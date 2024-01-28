using Porygon.Entity.Interfaces;
using Porygon.Entity.Tasks;
using Porygon.Entity.Utils;

namespace Porygon.Entity.Manager
{
    public class EntityManager : EntityManager<PoryEntity, PoryEntity, EntityFilter, IEntityDataManager>
    {
        public EntityManager(IEntityDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T> : EntityManager<T, T, EntityFilter, IEntityDataManager<T>>
           where T : PoryEntity, new()
    {
        public EntityManager(IEntityDataManager<T> dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T, TDataManager> : EntityManager<T, T, EntityFilter, TDataManager>
           where T : PoryEntity, new()
           where TDataManager : IEntityDataManager<T>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T, TFilter, TDataManager> : EntityManager<T, T, TFilter, TDataManager>
           where T : PoryEntity, new()
           where TFilter : EntityFilter, new()
           where TDataManager : IEntityDataManager<T, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T, TModel, TFilter, TDataManager> : IEntityManager<T, TFilter, TModel>
        where T : class, IKeyEntity<Guid>, new()
        where TFilter : EntityFilter, new()
        where TModel : T
        where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        protected readonly TDataManager dataManager;
        private readonly IServiceProvider serviceProvider;
        private readonly ITransactionExecutor transactionExecutor;
        private readonly EntityEventsHook createHook;
        private readonly EntityEventsHook updateHook;
        private readonly EntityEventsHook deleteHook;

        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider)
        {
            this.dataManager = dataManager;
            this.serviceProvider = serviceProvider;
            transactionExecutor = new TransactionExecutor(dbConnectionProvider);
            createHook = new EntityEventHooksBuilder()
                .PreValidation(PreCreateValidation)
                .Pre(PreCreation)
                .Post(PostCreation)
                .Build();
            updateHook = new EntityEventHooksBuilder()
                .PreValidation(PreUpdateValidation)
                .Pre(PreUpdate)
                .Post(PostUpdate)
                .Build();
            deleteHook = new EntityEventHooksBuilder()
                .PreValidation(PreDeleteValidation)
                .Pre(PreDeletion)
                .Post(PostDeletion)
                .Build();
        }

        #region Public Functions

        public async Task<T?> Create(TModel model)
        {
            return await new EntityCreator(this, serviceProvider)
                .Entity(model)
                .TransactionExecutor(transactionExecutor)
                .Execute<T>();
        }

        public async Task<T?> Update(TModel model)
        {
            return await new EntityUpdater(this, serviceProvider)
                .Entity(model)
                .TransactionExecutor(transactionExecutor)
                .Execute<T>();
        }

        public async Task<int> Delete(Guid id)
        {
            var entity = await GetEnriched(id);
            return await new EntityDeleter(this, serviceProvider)
                .Entity(entity!)
                .TransactionExecutor(transactionExecutor)
                .Execute<T>() != null ? 1 : 0;
        }

        public async Task<List<T?>?> CreateBulk(List<TModel> models)
        {
            return await models.ParallelForEach(Create);
        }

        public async Task<TModel?> GetEnriched(Guid id)
        {
            return await new EntityRetriever<T, TModel>(this, serviceProvider)
                .ShouldEnrich()
                .Get(id);
        }

        public async Task<TModel?> Get(Guid id)
        {
            return await new EntityRetriever<T, TModel>(this, serviceProvider)
                .Get(id);
        }

        public async Task<List<TModel>> GetAll()
        {
            return await new EntityRetriever<T, TModel>(this, serviceProvider)
                .Get();
        }

        public async Task<List<TModel>> GetAllEnriched()
        {
            return await new EntityRetriever<T, TModel>(this, serviceProvider)
                .ShouldEnrich()
                .Get();
        }

        public async Task<List<TModel>> Search(TFilter filter)
        {
            return await new EntitySearcher<T, TModel, TFilter, TDataManager>(this, dataManager, serviceProvider)
                .Filter(filter)
                .Search();
        }

        public async Task<List<TModel>> SearchEnriched(TFilter filter)
        {
            return await new EntitySearcher<T, TModel, TFilter, TDataManager>(this, dataManager, serviceProvider)
                .Filter(filter)
                .ShouldEnrich()
                .Search();
        }

        public async Task<List<TModel>> GetByLinkedItemId(Guid id)
        {
            return await SearchEnriched(new TFilter()
            {
                LinkedItemId = id
            });
        }

        public async Task<IEnumerable<object?>> GetLinkedItems(Guid id)
        {
            return (IEnumerable<object?>) await new EntitySearcher<T, TModel, TFilter, TDataManager>(this, dataManager, serviceProvider)
                .Filter(new TFilter() { LinkedItemId = id })
                .ShouldEnrich()
                .Search();
        }

        public async Task<object?> GetSingle(Guid id)
        {
            return await new EntityRetriever<T, TModel>(this, serviceProvider)
                .ShouldEnrich()
                .Get(id);
        }

        public IDataManager GetDataManager()
        {
            return dataManager;
        }

        public IEntityEventsHook GetCreationHook()
        {
            return createHook;
        }

        public IEntityEventsHook GetUpdateHook()
        {
            return updateHook;
        }

        public IEntityEventsHook GetDeletionHook()
        {
            return deleteHook;
        }
        #endregion
        
        #region Protected Functions

        protected virtual Task PreCreateValidation(object? model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreUpdateValidation(object? model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreDeleteValidation(object? model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreCreation(object model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostCreation(object model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreUpdate(object model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostUpdate(object model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreDeletion(object model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostDeletion(object model)
        {
            return Task.CompletedTask;
        }
        #endregion
    }

}
