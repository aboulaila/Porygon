using Porygon.Entity.Data;
using Porygon.Entity.Entity;
using Porygon.Entity.Relationships;
using System.Collections;
using System.Reflection;
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
           where T : PoryEntity<Guid>
    {
        public EntityManager(IEntityDataManager<T, Guid, EntityFilter> dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TDataManager> : EntityManager<T, Guid, T, EntityFilter, TDataManager>
           where T : PoryEntity<Guid>
           where TDataManager : IEntityDataManager<T, Guid, EntityFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TFilter, TDataManager> : EntityManager<T, Guid, T, TFilter, TDataManager>
           where T : PoryEntity<Guid>
           where TFilter : EntityFilter
           where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TModel, TFilter, TDataManager> : EntityManager<T, Guid, TModel, TFilter, TDataManager>
           where T : PoryEntity<Guid>
           where TFilter : EntityFilter
           where TModel : T
           where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        public EntityManager(TDataManager dataManager, IServiceProvider serviceProvider) : base(dataManager, serviceProvider)
        {
        }
    }

    public class EntityManager<T, TKey, TModel, TFilter, TDataManager> : IEntityManager<T, TKey, TFilter, TModel>
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter
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

        #region Public Methods
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

        public async Task<T?> Create(TModel model)
        {
            return await Create(model, null);
        }

        public async Task<T?> Create(TModel model, TransactionScope? scope)
        {
            bool isValid = await PreCreateValidation(model);
            if (!isValid)
                return null;

            model.Enrich(true);

            var localScope = scope ?? new TransactionScope();

            _ = await PreCreation(model, localScope);
            DataManager.Insert(model);
            _ = await PostCreation(model, localScope);


            if (scope == null)
                localScope.Complete();

            return model;
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
            return await Update(model, null);
        }

        public async Task<T?> Update(TModel model, TransactionScope? scope)
        {
            bool isValid = await PreUpdateValidation(model);
            if (!isValid)
                return null;

            model.Enrich(false);

            using var localScope = new TransactionScope();

            await PreUpdate(model);
            DataManager.Update(model);
            await PostUpdate(model);

            return model;
        }

        public async Task<int> Delete(TKey id)
        {
            return await Delete(id, null);
        }

        public async Task<int> Delete(TKey id, TransactionScope? scope)
        {
            bool isValid = await PreDeleteValidation(id);
            if (!isValid)
                return -1;

            using var localScope = new TransactionScope();

            await PreDeletion(id);
            var result = DataManager.Delete(id);
            await PostDeletion(id);

            return result;
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

        public async Task<object> Create(object model, TransactionScope scope)
        {
            return await Create((TModel)model, scope);
        }

        public async Task<object> Update(object model, TransactionScope scope)
        {
            return await Update((TModel)model, scope);
        }

        public async Task<int> Delete(object id, TransactionScope scope)
        {
            return await Delete((TKey)id, scope);
        }
        #endregion

        #region Abstract/Virtual Methods

        protected virtual Task<bool> PreCreateValidation(TModel model)
        {
            if (model == null)
                throw new ArgumentException("Model cannot be null");
            return Task.FromResult(true);
        }

        protected virtual Task<bool> PreUpdateValidation(TModel model)
        {
            if (model == null)
                throw new ArgumentException("Model cannot be null");
            return Task.FromResult(true);
        }

        protected virtual Task<bool> PreDeleteValidation(TKey id)
        {
            if (id == null || id.Equals(default))
                throw new ArgumentException("Id cannot be null");
            return Task.FromResult(true);
        }

        protected async Task<object?> PreCreation(TModel model, TransactionScope scope)
        {
            var relationships = GetRelationships(model, RelationshipType.HasA);

            if (Extensions.IsNullOrEmpty(relationships))
                return null;

            List<PoryEntity<TKey>> entities = new();
            foreach (var relationship in relationships)
            {
                if (relationship.EntityInstance is not PoryEntity<TKey> entity)
                    throw new InvalidCastException($"EntityInstance related to {model.GetType().Name}, with HasA relationship, is not of type PoryEntity");

                var created = await CreateRelatedEntity(model, relationship, entity, scope);
                if (created != null)
                    entities.Add(created);
            };
            return entities;
        }

        protected virtual async Task<object?> PostCreation(TModel model, TransactionScope scope)
        {
            var relationships = GetRelationships(model, RelationshipType.HasMany);

            if (Extensions.IsNullOrEmpty(relationships))
                return null;

            List<PoryEntity<TKey>> entities = new();
            foreach (var relationship in relationships)
            {
                if (relationship.EntityInstance is not IEnumerable<PoryEntity<TKey>> entityList)
                    throw new InvalidCastException($"EntityInstance related to {model.GetType().Name}, with HasMany relationship, is not a list of PoryEntity");

                foreach (var entity in entityList)
                {
                    var created = await CreateRelatedEntity(model, relationship, entity, scope);
                    if (created != null)
                        entities.Add(created);
                }
            }

            return entities;
        }

        protected virtual Task PreUpdate(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostUpdate(TModel model)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PreDeletion(TKey id)
        {
            return Task.CompletedTask;
        }

        protected virtual Task PostDeletion(TKey id)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<TModel> ToViewModel(T entity)
        {
            return Task.FromResult((TModel)entity);
        }
        #endregion

        private List<Relationship>? GetRelationships(T entity, RelationshipType relationshipType)
        {
            var properties = entity.GetType().GetProperties().Where(p => HasTheDesiredAttribute(p, relationshipType));
            if (Extensions.IsNullOrEmpty(properties))
                return null;

            var relationships = new List<Relationship>();
            foreach (var property in properties)
            {
                RelationshipAttribute attribute = property.GetCustomAttribute<RelationshipAttribute>();
                IEntityManager entityManager = GetEntityManagerInstance(property, attribute);
                object? relatedEntityInstance = GetRelatedEntityInstance(entity, property);
                if (relatedEntityInstance == null)
                    continue;

                if (attribute is HasAAttribute)
                {
                    relationships.Add(Relationship.HasA(relatedEntityInstance, property.PropertyType, entityManager));
                }
                else if (attribute is HasManyAttribute)
                {
                    relationships.Add(Relationship.HasMany(relatedEntityInstance, property.PropertyType, entityManager));
                }
            }

            return relationships;
        }

        private static async Task<PoryEntity<TKey>> CreateRelatedEntity(TModel model, Relationship relationship, PoryEntity<TKey> entity, TransactionScope scope)
        {
            if (HasNoId(entity.Id))
            {
                entity.LinkedItemId = model.Id;
                return (PoryEntity<TKey>)(await relationship.EntityManager.Create(entity, scope) ?? throw new Exception($"Failed to create {entity.GetType()}"));
            }
            return null;
        }

        private static bool HasTheDesiredAttribute(PropertyInfo property, RelationshipType relationshipType)
        {
            RelationshipAttribute? desiredAttribute = relationshipType.Equals(RelationshipType.HasA) ?
                property.GetCustomAttribute<HasAAttribute>() :
                property.GetCustomAttribute<HasManyAttribute>();
            return desiredAttribute != null;
        }

        private IEntityManager GetEntityManagerInstance(PropertyInfo property, RelationshipAttribute attribute)
        {
            return attribute.EntityManager != null ?
                                        GetEntityManager(attribute.EntityManager) :
                                        GetDefaultEntityManager(property);

        }

        private IEntityManager GetDefaultEntityManager(PropertyInfo property)
        {
            if (property.PropertyType.IsAssignableTo(typeof(ICollection)))
            {
                Type genericType = property.PropertyType.GetGenericArguments()[0];
                if (genericType != null)
                    return GetEntityManagerByEntityType(genericType);
            }

            return GetEntityManagerByEntityType(property.PropertyType);
        }

        private IEntityManager GetEntityManagerByEntityType(Type? entityType)
        {
            if (entityType != null && entityType.IsAssignableTo(typeof(PoryEntity)))
                return GetEntityManager(typeof(EntityManager<>).MakeGenericType(entityType));

            throw new MissingMemberException($"Could not find EntityManager of type {entityType}");
        }

        private IEntityManager GetEntityManager(Type entityManagerType)
        {
            var manager = ServiceProvider.GetService(entityManagerType);
            if (manager != null && manager is IEntityManager castedManager)
                return castedManager;

            throw new MissingMemberException($"Could not find EntityManager of type {entityManagerType.FullName}");
        }

        private static object? GetRelatedEntityInstance(T entity, PropertyInfo property)
        {
            return property.GetValue(entity);
        }

        private static bool HasNoId(TKey id)
        {
            if (id is Guid guid)
                return guid.Equals(Guid.Empty);
            else if (id is object obj)
                return obj == default;
            return true;
        }
    }

}
