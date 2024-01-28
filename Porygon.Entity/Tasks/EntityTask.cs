using Porygon.Entity.Interfaces;

namespace Porygon.Entity.Tasks
{
    public abstract class EntityTask : IEntityTask
    {
        protected readonly IEntityManager entityManager;
        protected readonly IDataManager dataManager;
        protected IServiceProvider serviceProvider;

        public EntityTask(IEntityManager entityManager, IServiceProvider serviceProvider)
        {
            this.entityManager = entityManager;
            this.serviceProvider = serviceProvider;
            dataManager = entityManager.GetDataManager();
        }
    }
}

