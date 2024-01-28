using System.Data;
using Porygon.Entity.Interfaces;

namespace Porygon.Entity.Utils
{
    public class TransactionExecutor : ITransactionExecutor
    {
        private readonly IDbConnectionProvider dbConnectionProvider;
        private IDbTransaction? transaction;
        private object l = new();
        public TransactionExecutor(IDbConnectionProvider dbConnectionProvider)
		{
            this.dbConnectionProvider = dbConnectionProvider;
		}

        public async Task<S> ExecuteInTransaction<S>(Func<IDbTransaction, Task<S>> execute)
        {
            BeginTransaction();
            try
            {
                var result = await execute(transaction!);
                transaction!.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction!.Rollback();
                throw;
            }
            finally
            {
                transaction!.Dispose();
            }
        }

        public IDbTransaction GetTransaction()
        {
            return transaction!;
        }

        private void BeginTransaction()
        {
            lock(l)
            {
                if (transaction == null)
                {
                    var connection = dbConnectionProvider.GetConnection();
                    transaction = connection.BeginTransaction();
                }
            }
        }

    }
}

