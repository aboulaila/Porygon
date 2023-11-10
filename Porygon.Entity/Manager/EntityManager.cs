using Porygon.Entity.Data;
using System.Transactions;

namespace Porygon.Entity.Manager
{
    public abstract class EntityManager<T, TDataManager> : EntityManager<T, Guid, T, EntityFilter, TDataManager>
           where T : PoryEntity<Guid>
           where TDataManager : IEntityDataManager<T, Guid, EntityFilter>
    {
        protected EntityManager(TDataManager dataManager) : base(dataManager)
        {
        }
    }

    public abstract class EntityManager<T, TFilter, TDataManager> : EntityManager<T, Guid, T, TFilter, TDataManager>
           where T : PoryEntity<Guid>
           where TFilter : EntityFilter
           where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        protected EntityManager(TDataManager dataManager) : base(dataManager)
        {
        }
    }

    public abstract class EntityManager<T, TModel, TFilter, TDataManager> : EntityManager<T, Guid, TModel, TFilter, TDataManager>
           where T : PoryEntity<Guid>
           where TFilter : EntityFilter
           where TModel : T
           where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        protected EntityManager(TDataManager dataManager) : base(dataManager)
        {
        }
    }

    public abstract class EntityManager<T, TKey, TModel, TFilter, TDataManager> : IEntityManager<T, TKey, TFilter, TModel>
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter
        where TModel : T
        where TDataManager : IEntityDataManager<T, TKey, TFilter>
    {
        protected TDataManager DataManager;

        public EntityManager(TDataManager dataManager)
        {
            DataManager = dataManager;
        }

        #region Public Methods
        public async Task<T> Create(TModel model)
        {
            await PreCreateValidation(model);

            var entity = ToEntity(model, true);

            using var scope = new TransactionScope();
            
            await PreCreation(model);
            DataManager.Insert(entity);
            await PostCreation(model);
            
            return entity;
        }

        public async Task<T> Update(TModel model)
        {
            await PreUpdateValidation(model);

            var entity = ToEntity(model, false);

            using var scope = new TransactionScope();

            await PreUpdate(model);
            DataManager.Update(entity);
            await PostCreation(model);

            return entity;
        }

        public async Task<int> Delete(TKey id)
        {
            if (id == null || id.Equals(default))
                return 0;

            using var scope = new TransactionScope();
            
            await PreDeletion(id);
            var result = DataManager.Delete(id);
            await PostDeletion(id);

            return result;
        }

        public async Task<TModel?> Get(TKey id)
        {
            if (id == null || id.Equals(default))
                return default;

            var entity = await DataManager.GetAsync(id);

            return await ToViewModel(entity);
        }

        public async Task<IEnumerable<TModel>> GetAll()
        {
            var results = await DataManager.GetAll();
            if (results != null && results.Any())
            {
                return await Task.WhenAll(results.Select(e => ToViewModel(e)));
            }

            return new List<TModel>();
        }

        public async Task<IEnumerable<TModel>> Search(TFilter filter)
        {
            var results = await DataManager.Search(filter);
            if (results != null && results.Any())
            {
                return await Task.WhenAll(results.Select(e => ToViewModel(e)));
            }

            return new List<TModel>();
        }
        #endregion

        #region Abstract/Virtual Methods
        protected virtual Task PreCreateValidation(TModel model)
        {
            if (model == null)
                throw new ArgumentException("Model cannot be null");
            return Task.CompletedTask;
        }

        protected virtual Task PreUpdateValidation(TModel model)
        {
            if (model == null)
                throw new ArgumentException("Model cannot be null");
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

        private Task PreDeletion(TKey id)
        {
            return Task.CompletedTask;
        }

        private Task PostDeletion(TKey id)
        {
            return Task.CompletedTask;
        }

        protected abstract T ToEntity(TModel model, bool isNew);

        protected abstract Task<TModel> ToViewModel(T entity);
        #endregion
    }

}
