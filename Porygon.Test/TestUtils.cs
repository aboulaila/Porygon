using Moq;
using Porygon.Entity.Data;
using Porygon.Entity;

namespace Porygon.Test
{
    public static class TestUtils
    {
        public static void AssertEntityIsInserted<T>(T entity, IEntityDataManager<T> dataManager) where T : PoryEntity
        {
            Mock.Get(dataManager).Verify(x => x.Insert(entity), Times.Once);
        }
        public static void AssertEntityIsNotInserted<T>(T entity, IEntityDataManager<T> dataManager) where T : PoryEntity
        {
            Mock.Get(dataManager).Verify(x => x.Insert(entity), Times.Never);
        }

        public static void AssertIdNotEmpty<T>(T entity) where T : PoryEntity
        {
            Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty), () => $"{entity.GetType().Name}'s Id is empty");
        }
    }
}
