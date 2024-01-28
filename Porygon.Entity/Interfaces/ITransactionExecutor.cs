using System.Data;

namespace Porygon.Entity.Interfaces
{
    public interface ITransactionExecutor
	{
        public Task<S> ExecuteInTransaction<S>(Func<IDbTransaction, Task<S>> execute);
        public IDbTransaction GetTransaction();
    }
}

