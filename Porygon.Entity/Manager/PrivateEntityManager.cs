using Porygon.Entity.Data;
using Porygon.Entity.Entity;
using Porygon.Entity.Relationships;
using System.Collections;
using System.Reflection;
using System.Transactions;

namespace Porygon.Entity.Manager
{
    public partial class EntityManager<T, TKey, TModel, TFilter, TDataManager>
        where T : PoryEntity<TKey>, new()
        where TFilter : EntityFilter<TKey>, new()
        where TModel : T
        where TDataManager : IEntityDataManager<T, TKey, TFilter>
    {
        private async Task<List<TModel>> ToViewModel(List<T> results, bool enriched)
        {
            if (!Extensions.IsNullOrEmpty(results))
            {
                return await results.ParallelForEach(async (entity) => {                    
                    return await EnrichAndConvertToViewModel(entity, enriched);
                });
            }

            return new List<TModel>();
        }

        private async Task<TModel> EnrichAndConvertToViewModel(T entity, bool enriched)
        {
            if (enriched)
            {
                await EnrichEntity(entity);
            }

            return entity.ToViewModel<TModel>();
        }

        private static void PreEntityValidation(TModel? model)
        {
            if (model == null || model.Id == null)
                throw new ArgumentException("Id cannot be null");
        }

        private async Task VisitRelationships(TModel model, TransactionScope scope, Func<TModel, Relationship, PoryEntity<TKey>, TransactionScope, Task> relationshipHandler, RelationshipType? relationshipType = null, bool isCascading = false)
        {
            var relationships = GetRelationships(model, relationshipType, isCascading);

            if (Extensions.IsNullOrEmpty(relationships))
                return;

            foreach (var relationship in relationships)
            {
                if (relationship == null)
                    continue;
                await VisitRelationship(model, scope, relationship, relationshipHandler);
            }
        }

        private static async Task VisitRelationship(TModel model, TransactionScope scope, Relationship relationship, Func<TModel, Relationship, PoryEntity<TKey>, TransactionScope, Task> relationshipHandler)
        {
            var entityList = GetRelatedEntities(model, relationship);
            foreach (var entity in entityList)
            {
                await relationshipHandler(model, relationship, entity, scope);
            }
        }

        private static async Task CheckRelatedEntityState(TModel model, Relationship relationship, PoryEntity<TKey> entity, TransactionScope scope)
        {
            if (ShouldCreateEntity(entity))
            {
                await CreateRelatedEntity(model, relationship, entity, scope);
            }
            else if (Equals(entity.State, EntityStates.UPDATED))
            {
                await UpdateRelatedEntity(relationship, entity, scope);
            }
            else if (Equals(entity.State, EntityStates.DELETED))
            {
                await DeleteRelatedEntity(relationship, entity.Id!, scope);
            }
        }

        private static async Task CheckRelatedEntityCreation(TModel model, Relationship relationship, PoryEntity<TKey> entity, TransactionScope scope)
        {
            if (ShouldCreateEntity(entity))
            {
                await CreateRelatedEntity(model, relationship, entity, scope);
            }
        }

        private static async Task CheckCascadingEntityDeletion(TModel model, Relationship relationship, PoryEntity<TKey> entity, TransactionScope scope)
        {
            await DeleteRelatedEntity(relationship, entity.Id!, scope);
        }

        private List<Relationship> GetRelationships(T entity, RelationshipType? relationshipType = null, bool cascading = false)
        {
            IEnumerable<PropertyInfo> properties = GetPropertiesWithDesiredAttribute(entity, relationshipType);

            return properties.Select(property => ExtractRelationshipFromProperty(entity, property, cascading)).FilterNulls();
        }

        private Relationship? ExtractRelationshipFromProperty(T entity, PropertyInfo property, bool cascading)
        {
            RelationshipAttribute? attribute = property.GetCustomAttribute<RelationshipAttribute>();
            if (AttributeIsNullOrShouldBeCascadingButIsNot(cascading, attribute))
                return null;

            Relationship? relationship = CreateRelationshipInstance(entity, property, attribute!);
            if (relationship == null)
                return null;

            relationship.EntityManager = GetEntityManagerInstance(property, attribute!);
            return relationship;
        }

        private static bool AttributeIsNullOrShouldBeCascadingButIsNot(bool cascading, RelationshipAttribute? attribute)
        {
            return attribute == null || (cascading && !attribute.IsCascading);
        }

        private static Relationship? CreateRelationshipInstance(T entity, PropertyInfo property, RelationshipAttribute attribute)
        {
            object? relatedEntityInstance = GetRelatedEntityInstance(entity, property);
            object? relatedEntityId = attribute.IsHasA ? GetRelatedEntityId(entity, attribute, property.Name) : null;

            if (DoesntHaveRelatedEntity(attribute, relatedEntityInstance, relatedEntityId))
                return null;

            return new()
            {
                PropertyName = property.Name,
                PropertyIdName = attribute.EntityIdProperty,
                EntityId = relatedEntityId,
                EntityInstance = relatedEntityInstance,
                Entity = property.PropertyType,
                Type = attribute.IsHasMany ? RelationshipType.HasMany : RelationshipType.HasA
            };
        }

        private static IEnumerable<PropertyInfo> GetPropertiesWithDesiredAttribute(T entity, RelationshipType? relationshipType)
        {
            IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties()
                .Where(p => HasTheDesiredAttribute(p, relationshipType));

            if (Extensions.IsNullOrEmpty(properties))
                return new List<PropertyInfo>();
            return properties;
        }

        private static bool DoesntHaveRelatedEntity(RelationshipAttribute attribute, object? relatedEntityInstance, object? relatedEntityId)
        {
            return attribute.IsHasA && relatedEntityInstance == null && EmptyId(relatedEntityId);
        }

        private static List<PoryEntity<TKey>> GetRelatedEntities(TModel model, Relationship relationship)
        {
            if (relationship.IsHasA)
            {
                PoryEntity<TKey> entity = GetEntityInstance(model, relationship);
                return new List<PoryEntity<TKey>>() { entity };
            }
            else
            {
                return GetEntityListInstance(model, relationship);
            }
        }

        private static PoryEntity<TKey> GetEntityInstance(TModel model, Relationship relationship)
        {
            if (relationship.EntityInstance is not PoryEntity<TKey> entity)
                throw new InvalidCastException($"EntityInstance related to {model.GetType().Name}, with HasA relationship, is not of type PoryEntity");
            return entity;
        }

        private static List<PoryEntity<TKey>> GetEntityListInstance(TModel model, Relationship relationship)
        {
            if (relationship.EntityInstance is not IEnumerable<PoryEntity<TKey>> entityList)
                throw new InvalidCastException($"EntityInstance related to {model.GetType().Name}, with HasMany relationship, is not a list of PoryEntity");
            return entityList.FilterNulls();
        }

        private static async Task CreateRelatedEntity(TModel model, Relationship relationship, PoryEntity<TKey> entity, TransactionScope scope)
        {
            entity.LinkedItemId = model.Id;
            _ = await relationship.EntityManager!.Create(entity, scope) ?? throw new Exception($"Failed to create {entity.GetType()}");
        }

        private static async Task UpdateRelatedEntity(Relationship relationship, PoryEntity<TKey> entity, TransactionScope scope)
        {
            _ = await relationship.EntityManager!.Update(entity, scope) ?? throw new Exception($"Failed to update {entity.GetType()}");
        }

        private static async Task DeleteRelatedEntity(Relationship relationship, TKey id, TransactionScope scope)
        {
            bool isDeleted = await relationship.EntityManager!.Delete(id!, scope) > 0;
            if (!isDeleted)
                throw new Exception($"Failed to delete entity with ID: {id}");
        }

        private static bool HasTheDesiredAttribute(PropertyInfo property, RelationshipType? relationshipType)
        {
            if (relationshipType == null)
                return property.GetCustomAttribute<RelationshipAttribute>() != null;

            RelationshipAttribute? desiredAttribute = Equals(relationshipType, RelationshipType.HasA) ?
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

        private static async Task EnrichRelatedEntities(T entity, Relationship relationship)
        {
            if (relationship.IsHasMany)
            {
                await EnrichHasManyEntityList(entity, relationship);
            }
            else
            {
                await EnrichHasAEntity(entity, relationship);

            }
        }

        private static async Task EnrichHasManyEntityList(T entity, Relationship relationship)
        {
            var relatedEntityList = await relationship.EntityManager!.GetByLinkedItemId(entity.Id!);
            if (Extensions.IsNullOrEmpty(relatedEntityList))
                return;

            relationship.EntityInstance = relatedEntityList;
            entity.GetType().GetProperty(relationship.PropertyName!)?.SetValue(entity, relatedEntityList, null);
        }

        private static async Task EnrichHasAEntity(T entity, Relationship relationship)
        {
            object id = relationship.EntityId
                        ?? throw new ArgumentNullException("Property of entity id is null");

            var relatedEntity = await relationship.EntityManager!.Get(id);
            if (relatedEntity == null)
                return;

            relationship.EntityId = id;
            relationship.EntityInstance = relatedEntity;
            entity.GetType().GetProperty(relationship.PropertyName!)?.SetValue(entity, relatedEntity, null);
            entity.GetType().GetProperty(relationship.PropertyIdName!)?.SetValue(entity, id, null);
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

        private static object GetRelatedEntityId(T entity, RelationshipAttribute attribute, string propertyName)
        {
            if (string.IsNullOrEmpty(attribute.EntityIdProperty))
                attribute.EntityIdProperty = $"{propertyName}Id";

            var entityId = entity.GetType().GetProperty(attribute.EntityIdProperty)?.GetValue(entity);
            return entityId ?? throw new MissingMemberException($"Could not find Property {attribute.EntityIdProperty}");
        }

        private static bool ShouldCreateEntity(PoryEntity<TKey> entity)
        {
            return entity?.State == EntityStates.NEW || EmptyId(entity!.Id);
        }

        private static bool EmptyId(object? id)
        {
            if (id == null)
                return true;
            if (id is Guid guid)
                return guid.Equals(Guid.Empty);
            else if (id is object obj)
                return obj == default;
            return true;
        }

        private static TransactionScope GetTransactionScope(TransactionScope? scope)
        {
            return scope ?? new TransactionScope();
        }

        private static void CompleteTransactionScope(TransactionScope? scope, TransactionScope localScope)
        {
            if (scope == null)
                localScope.Complete();
        }
    }
}
