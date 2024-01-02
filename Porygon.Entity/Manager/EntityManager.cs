using System.Data;
using Porygon.Entity.Data;
using Porygon.Entity.Relationships;

namespace Porygon.Entity.Manager
{
    public class EntityManager : EntityManager<PoryEntity, Guid, PoryEntity, EntityFilter, IEntityDataManager>
    {
        public EntityManager(IEntityDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T> : EntityManager<T, Guid, T, EntityFilter, IEntityDataManager<T>>
           where T : PoryEntity, new()
    {
        public EntityManager(IEntityDataManager<T> dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T, TDataManager> : EntityManager<T, Guid, T, EntityFilter, TDataManager>
           where T : PoryEntity, new()
           where TDataManager : IEntityDataManager<T>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T, TFilter, TDataManager> : EntityManager<T, Guid, T, TFilter, TDataManager>
           where T : PoryEntity, new()
           where TFilter : EntityFilter, new()
           where TDataManager : IEntityDataManager<T, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
        {
        }
    }

    public class EntityManager<T, TModel, TFilter, TDataManager> : EntityManager<T, Guid, TModel, TFilter, TDataManager>
           where T : PoryEntity, new()
           where TFilter : EntityFilter, new()
           where TModel : T
           where TDataManager : IEntityDataManager<T, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider) : base(dataManager, serviceProvider, dbConnectionProvider)
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
        protected IDbConnectionProvider DbConnectionProvider;

        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider, IDbConnectionProvider dbConnectionProvider)
        {
            DataManager = dataManager;
            ServiceProvider = serviceProvider;
            DbConnectionProvider = dbConnectionProvider;
        }

        #region Public Functions

        public async Task<T?> Create(TModel model)
        {
            return await ExecuteInTransaction(model, CreateInternal);
        }

        public async Task<List<T>?> CreateBulk(List<TModel> models)
        {
            return await ExecuteInTransaction(models, CreateBulkInternal);
        }

        public async Task<T?> Update(TModel model)
        {
            return await ExecuteInTransaction(model, UpdateInternal);
        }

        public async Task<int> Delete(TKey id)
        {
            return await ExecuteInTransaction(id, DeleteInternal);
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

        protected async Task<T?> CreateInternal(TModel model, IDbTransaction transaction)
        {
            PreEntityValidation(model);
            await PreCreateValidation(model);

            model.Enrich(true);

            await VisitRelationships(model, CheckRelatedEntityCreation, RelationshipType.HasA);
            await PreCreation(model);
            DataManager.Insert(model, transaction);
            await VisitRelationships(model, CheckRelatedEntityCreation, RelationshipType.HasMany);
            await PostCreation(model);

            return model;
        }

        protected async Task<T?> UpdateInternal(TModel model, IDbTransaction transaction)
        {
            PreEntityValidation(model);
            await PreUpdateValidation(model);

            model.Enrich(false);

            await VisitRelationships(model, ManipulateRelatedEntity, RelationshipType.HasA);
            await PreUpdate(model);
            DataManager.Update(model, transaction);
            await VisitRelationships(model, ManipulateRelatedEntity, RelationshipType.HasMany);
            await PostUpdate(model);
            return model;
        }

        protected async Task<int> DeleteInternal(TKey id, IDbTransaction transaction)
        {
            var entity = await GetInternal(id, true);
            PreEntityValidation(entity);
            await PreDeleteValidation(entity);

            await VisitRelationships(entity!, CheckCascadingEntityDeletion, isCascading: true);
            await PreDeletion(entity!);
            var result = DataManager.Delete(id, transaction);
            await PostDeletion(entity!);

            return result;
        }

        protected async Task<List<T>?> CreateBulkInternal(List<TModel> models, IDbTransaction transaction)
        {
            var entities = new List<T>();
            foreach (var model in models)
            {
                var entity = await CreateInternal(model, transaction);
                entities.Add(entity!);
            }
            return entities;
        }

        protected async Task<TModel?> GetInternal(TKey id, bool enriched)
        {
            if (EmptyId(id))
                return null;

            T? entity = await DataManager.GetAsync(id);
            if (entity == null)
                return null;

            return await EnrichAndConvertToViewModel(entity!, enriched);
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
        public async Task<object> Create(object model, IDbTransaction transaction)
        {
            return await CreateInternal((TModel)model, transaction);
        }

        public async Task<object> Update(object model, IDbTransaction transaction)
        {
            return await UpdateInternal((TModel)model, transaction);
        }

        public async Task<int> Delete(object id, IDbTransaction transaction)
        {
            return await DeleteInternal((TKey)id, transaction);
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

        protected virtual Task PreCreation(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostCreation(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreUpdate(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostUpdate(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreDeletion(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostDeletion(TModel model)
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
