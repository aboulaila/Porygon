using Porygon.Entity.Data;
using Porygon.Entity.Relationships;
using System.Transactions;

namespace Porygon.Entity.Manager
{
    public class EntityManager : EntityManager<PoryEntity, Guid, PoryEntity, EntityFilter, IEntityDataManager>
    {
        public EntityManager(IEntityDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T> : EntityManager<T, Guid, T, EntityFilter, IEntityDataManager<T, Guid, EntityFilter>>
           where T : PoryEntity<Guid>, new()
    {
        public EntityManager(IEntityDataManager<T, Guid, EntityFilter> dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TDataManager> : EntityManager<T, Guid, T, EntityFilter, TDataManager>
           where T : PoryEntity<Guid>, new()
           where TDataManager : IEntityDataManager<T, Guid, EntityFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TFilter, TDataManager> : EntityManager<T, Guid, T, TFilter, TDataManager>
           where T : PoryEntity<Guid>, new()
           where TFilter : EntityFilter, new()
           where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TModel, TFilter, TDataManager> : EntityManager<T, Guid, TModel, TFilter, TDataManager>
           where T : PoryEntity<Guid>, new()
           where TFilter : EntityFilter, new()
           where TModel : T
           where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public partial class EntityManager<T, TKey, TModel, TFilter, TDataManager> : IEntityManager<T, TKey, TFilter, TModel>
        where T : PoryEntity<TKey>, new()
        where TFilter : EntityFilter<TKey>, new()
        where TModel : T
        where TDataManager : IEntityDataManager<T, TKey, TFilter>
    {
        protected TDataManager DataManager;
        protected IServiceProvider ServiceProvider;

        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider)
        {
            DataManager = dataManager;
            ServiceProvider = serviceProvider;
        }

        #region Public Functions

        public async Task<T?> Create(TModel model)
        {
            return await CreateInternal(model, null);
        }

        public async Task<List<T>> CreateBulk(List<TModel> models)
        {
            var entities = new List<T>();

            foreach (var model in models)
            {
                var entity = await Create(model);
                entities.Add(entity);
            }

            return entities;
        }

        public async Task<T?> Update(TModel model)
        {
            return await UpdateInternal(model, null);
        }

        public async Task<int> Delete(TKey id)
        {
            return await DeleteInternal(id, null);
        }

        public async Task<TModel?> GetEnriched(TKey id)
        {
            return await GetInternal(id, true);
        }

        public async Task<TModel?> Get(TKey id)
        {
            return await GetInternal(id, false);
        }

        public async Task<List<TModel>> GetAll()
        {
            return await GetAllInternal(false);
        }

        public async Task<List<TModel>> GetAllEnriched()
        {
            return await GetAllInternal(true);
        }

        public async Task<List<TModel>> Search(TFilter filter)
        {
            return await SearchInternal(filter, false);
        }

        public async Task<List<TModel>> SearchEnriched(TFilter filter)
        {
            return await SearchInternal(filter, true);
        }
        #endregion

        #region Internal Functions

        protected async Task<T?> CreateInternal(TModel model, TransactionScope? scope)
        {
            PreEntityValidation(model);
            await PreCreateValidation(model);

            model.Enrich(true);

            var localScope = GetTransactionScope(scope);

            await PreCreation(model, localScope);
            DataManager.Insert(model);
            await PostCreation(model, localScope);

            CompleteTransactionScope(scope, localScope);

            return model;
        }

        protected async Task<T?> UpdateInternal(TModel model, TransactionScope? scope)
        {
            PreEntityValidation(model);
            await PreUpdateValidation(model);

            model.Enrich(false);

            var localScope = GetTransactionScope(scope);

            await PreUpdate(model, localScope);
            DataManager.Update(model);
            await PostUpdate(model, localScope);

            CompleteTransactionScope(scope, localScope);

            return model;
        }

        protected async Task<int> DeleteInternal(TKey id, TransactionScope? scope)
        {
            var entity = await GetInternal(id, true);
            PreEntityValidation(entity);
            await PreDeleteValidation(entity);

            var localScope = GetTransactionScope(scope);

            await PreDeletion(entity!, localScope);
            var result = DataManager.Delete(id);
            await PostDeletion(entity!, localScope);

            CompleteTransactionScope(scope, localScope);

            return result;
        }

        protected async Task<TModel?> GetInternal(TKey id, bool enriched)
        {
            if (EmptyId(id))
                return null;

            var entity = await DataManager.GetAsync(id);
            return await EnrichAndConvertToViewModel(entity, enriched);
        }

        protected async Task<List<TModel>> GetAllInternal(bool enriched)
        {
            var results = await DataManager.GetAll();
            return await ToViewModel(results, enriched);
        }

        protected async Task<List<TModel>> SearchInternal(TFilter filter, bool enriched)
        {
            var results = await DataManager.Search(filter);
            return await ToViewModel(results, enriched);
        }
        #endregion

        #region Interface Functions
        public async Task<object> Create(object model, TransactionScope scope)
        {
            return await CreateInternal((TModel)model, scope);
        }

        public async Task<object> Update(object model, TransactionScope scope)
        {
            return await UpdateInternal((TModel)model, scope);
        }

        public async Task<int> Delete(object id, TransactionScope scope)
        {
            return await DeleteInternal((TKey)id, scope);
        }

        public async Task<object> Get(object id)
        {
            return await GetInternal((TKey)id, true);
        }

        public async Task<IEnumerable<object?>> GetByLinkedItemId(object id)
        {
            return await SearchInternal(new TFilter() { LinkedItemId = (TKey)id }, true);
        } 
        #endregion

        #region Protected Functions

        protected virtual Task PreCreateValidation(TModel? model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreUpdateValidation(TModel? model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreDeleteValidation(TModel? model)
        {
            return Task.CompletedTask;
        }

        protected async Task PreCreation(TModel model, TransactionScope scope)
        {
            await VisitRelationships(model, scope, CheckRelatedEntityCreation, RelationshipType.HasA);
        }

        protected async Task PostCreation(TModel model, TransactionScope scope)
        {
            await VisitRelationships(model, scope, CheckRelatedEntityCreation, RelationshipType.HasMany);
        }

        protected async Task PreUpdate(TModel model, TransactionScope scope)
        {
            await VisitRelationships(model, scope, CheckRelatedEntityState, RelationshipType.HasA);
        }

        protected async Task PostUpdate(TModel model, TransactionScope scope)
        {
            await VisitRelationships(model, scope, CheckRelatedEntityState, RelationshipType.HasMany);
        }

        protected async Task PreDeletion(TModel model, TransactionScope scope)
        {
            await VisitRelationships(model, scope, CheckCascadingEntityDeletion, isCascading: true);
        }

        protected virtual Task PostDeletion(TModel model, TransactionScope scope)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task EnrichEntity(T entity)
        {
            var relationships = GetRelationships(entity);

            if (Extensions.IsNullOrEmpty(relationships))
                return;

            foreach (var relationship in relationships)
            {
                await EnrichRelatedEntities(entity, relationship);
            }
        }
        #endregion
    }

}
