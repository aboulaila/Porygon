using System.Data;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Relationships;
using Porygon.Entity.Utils;

namespace Porygon.Entity.Tasks
{
    public abstract class EntityManipulator : EntityTask
    {
        protected IKeyEntity<Guid>? entity;
        protected IEntityEventsHook? entityEventsHook;
        protected ITransactionExecutor? transactionExecutor;

        public EntityManipulator(IEntityManager entityManager, IServiceProvider serviceProvider) : base(entityManager, serviceProvider)
        {
        }

        public EntityManipulator Entity(IKeyEntity<Guid> entity)
        {
            this.entity = entity;
            return this;
        }

        public EntityManipulator TransactionExecutor(ITransactionExecutor? transactionExecutor)
        {
            this.transactionExecutor = transactionExecutor;
            return this;
        }

        public async Task<IKeyEntity<Guid>?> Execute()
        {
            return await Execute<IKeyEntity<Guid>>();
        }

        public async Task<T?> Execute<T>()
            where T : IKeyEntity<Guid>
        {
            if (entity == null)
                throw new ArgumentNullException("Entity cannot be null");

            if (serviceProvider == null)
                throw new ArgumentException("ServiceProvider must be set");

            if (dataManager == null)
                throw new ArgumentException("DataManager must be specified");

            entityEventsHook ??= new EntityEventHooksBuilder().PreValidation(PreValidation).Build();
            await entityEventsHook.InvokePreValidation(entity);

            return transactionExecutor != null ?
                await transactionExecutor.ExecuteInTransaction(ManipulateInternal<T>) :
                await ManipulateInternal<T>(null);
        }

        private Task PreValidation(object t)
        {
            if (entity == null || EntityHelper.EmptyId(entity.Id))
                throw new ArgumentException("Id cannot be null");
            return Task.CompletedTask;
        }

        private async Task<T?> ManipulateInternal<T>(IDbTransaction? transaction)
            where T : IKeyEntity<Guid>
        {
            entity!.Enrich(true);
            await Pre(transaction);
            await entityEventsHook!.InvokePre(entity);
            Manipulate(transaction);
            await Post(transaction);
            await entityEventsHook.InvokePost(entity);
            return (T)entity;
        }

        protected abstract Task Pre(IDbTransaction? transaction);
        protected abstract void Manipulate(IDbTransaction? transaction);
        protected abstract Task Post(IDbTransaction? transaction);

        protected async Task<Guid> CreateRelatedEntityIfNeeded(Relationship relationship, IKeyEntity<Guid> entity)
        {
            if (entity.ShouldCreateEntity<Guid>())
                return await CreateRelatedEntity(relationship, entity);
            return entity.Id;
        }

        protected async Task<Guid> CreateRelatedEntity(Relationship relationship, IKeyEntity<Guid> relatedEntity)
        {
            var newEntity = await new EntityCreator(relationship.EntityManager!, serviceProvider)
                .Entity(relatedEntity)
                .TransactionExecutor(transactionExecutor)
                .Execute() ?? throw new Exception($"Failed to create {relatedEntity.GetType()}");

            return newEntity.Id;
        }

        protected async Task<Guid> UpdateRelatedEntity(Relationship relationship, IKeyEntity<Guid> relatedEntity)
        {
            var newEntity = await new EntityUpdater(relationship.EntityManager!, serviceProvider)
                .Entity(relatedEntity)
                .TransactionExecutor(transactionExecutor)
                .Execute() ?? throw new Exception($"Failed to update {relatedEntity.GetType()}");

            return newEntity.Id;
        }

        protected async Task<Guid> DeleteRelatedEntity(Relationship relationship, IKeyEntity<Guid> relatedEntity)
        {
            var newEntity = await new EntityDeleter(relationship.EntityManager!, serviceProvider)
                .Entity(relatedEntity)
                .TransactionExecutor(transactionExecutor)
                .Execute() ?? throw new Exception($"Failed to delete entity with ID: {relatedEntity.Id}");

            return newEntity.Id;
        }
    }
}

