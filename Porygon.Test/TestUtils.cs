using Moq;
using Porygon.Entity;
using System.Data;
using Porygon.Entity.Interfaces;

namespace Porygon.Test
{
    public static class TestUtils
    {
        public static void AssertIdNotEmpty<T>(T entity) where T : PoryEntity
        {
            Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty), () => $"{entity.GetType().Name}'s Id is empty");
        }

        public static void SetupEntityGetter<T>(T entity, Mock<IEntityDataManager<T>> dataManager) where T : PoryEntity
        {
            dataManager.Setup(x => x.GetAsync<T>(entity.Id)).Returns(Task.FromResult(entity));
        }

        public static void SetupEntityGetter(PoryEntity entity, Mock<IEntityDataManager> dataManager)
        {
            dataManager.Setup(x => x.GetAsync<PoryEntity>(entity.Id)).Returns(Task.FromResult(entity));
        }

        public static Mock<IDbConnectionProvider> SetupDbConnectionProvider()
        {
            Mock<IDbConnection> connection = new();
            Mock<IDbTransaction> transaction = new();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            Mock<IDbConnectionProvider> dbConnectionProvider = new();
            dbConnectionProvider.Setup(x => x.GetConnection()).Returns(connection.Object);
            return dbConnectionProvider;
        }
    }
}
