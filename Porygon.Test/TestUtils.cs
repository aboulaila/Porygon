using Moq;
using Porygon.Entity.Data;
using Porygon.Entity;

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
            dataManager.Setup(x => x.GetAsync(entity.Id)).Returns(Task.FromResult(entity));
        }

        public static void SetupEntityGetter(PoryEntity entity, Mock<IEntityDataManager> dataManager)
        {
            dataManager.Setup(x => x.GetAsync(entity.Id)).Returns(Task.FromResult(entity));
        }
    }
}
