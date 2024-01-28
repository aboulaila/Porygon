using System.Collections;
using System.Reflection;
using Porygon.Entity.Entity;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Manager;
using Porygon.Entity.Relationships;

namespace Porygon.Entity.Utils
{
    public class RelationshipVisitor
    {
        private readonly IKeyEntity<Guid> model;
        private Func<Relationship, IKeyEntity<Guid>, Task>? handler;
        private RelationshipType? relationshipType = null;
        private bool isCascading = false;
        private IServiceProvider? serviceProvider;

        public RelationshipVisitor(IKeyEntity<Guid> model)
        {
            this.model = model;
        }

        #region Public Methods

        public RelationshipVisitor Handler(Func<Relationship, IKeyEntity<Guid>, Task> handler)
        {
            this.handler = handler;
            return this;
        }

        public RelationshipVisitor Type(RelationshipType relationshipType)
        {
            this.relationshipType = relationshipType;
            return this;
        }

        public RelationshipVisitor ServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            return this;
        }

        public RelationshipVisitor IsCascading()
        {
            isCascading = true;
            return this;
        }

        public async Task Visit()
        {
            if (serviceProvider == null)
                throw new ArgumentException("ServiceProvider must be provided");
            if (handler == null)
                throw new ArgumentException($"Handler is not specified for {nameof(RelationshipVisitor)}");
            var relationships = GetRelationships();
            await relationships.ParallelForEach(VisitRelationship);
        }

        public List<Relationship> GetRelationships()
        {
            var attributeType = GetRelationshipAttributeType();
            IEnumerable<PropertyInfo> properties = model.GetPropertiesByAttribute(attributeType);
            return properties.SelectNonNull(ExtractRelationshipFromProperty);
        }
        #endregion

        #region Private Methods

        private async Task VisitRelationship(Relationship relationship)
        {
            var entityList = model.GetEntityInstances<IKeyEntity<Guid>, IKeyEntity<Guid>>(relationship.EntityInstance);
            await entityList.ParallelForEach(async (entity) => await handler!(relationship, entity));
        }

        private Type GetRelationshipAttributeType()
        {
            return relationshipType == null ? typeof(RelationshipAttribute) :
                Equals(relationshipType, RelationshipType.HasA) ?
                    typeof(HasAAttribute) :
                    typeof(HasManyAttribute);
        }

        private Relationship? ExtractRelationshipFromProperty(PropertyInfo property)
        {
            RelationshipAttribute? attribute = property.GetCustomAttribute<RelationshipAttribute>();
            if (attribute == null || (isCascading && !attribute.IsCascading))
                return null;

            Relationship? relationship = CreateRelationshipInstance(model, property, attribute!);
            if (relationship == null)
                return null;

            relationship.EntityManager = GetEntityManagerInstance(property, attribute!);
            return relationship;
        }

        private static Relationship? CreateRelationshipInstance(IKeyEntity<Guid> entity, PropertyInfo property, RelationshipAttribute attribute)
        {
            object? relatedEntityInstance = property.GetValue(entity);
            Guid? relatedEntityId = attribute.IsHasA ? GetRelatedEntityId(entity, attribute, property.Name) : null;

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

        private static Guid? GetRelatedEntityId(IKeyEntity<Guid> entity, RelationshipAttribute attribute, string propertyName)
        {
            if (string.IsNullOrEmpty(attribute.EntityIdProperty))
                attribute.EntityIdProperty = $"{propertyName}Id";

            var entityId = (Guid?) entity.GetType().GetProperty(attribute.EntityIdProperty)?.GetValue(entity);
            return entityId ?? throw new MissingMemberException($"Could not find Property {attribute.EntityIdProperty}");
        }

        private static bool DoesntHaveRelatedEntity(RelationshipAttribute attribute, object? relatedEntityInstance, object? relatedEntityId)
        {
            return attribute.IsHasA && relatedEntityInstance == null && EntityHelper.EmptyId(relatedEntityId);
        }

        private IEntityManager GetEntityManagerInstance(PropertyInfo property, RelationshipAttribute attribute)
        {
            return attribute.EntityManager != null ?
                                        GetEntityManager(attribute.EntityManager) :
                                        GetDefaultEntityManager(property);

        }

        private IEntityManager GetEntityManager(Type entityManagerType)
        {
            try
            {
                var manager = serviceProvider!.GetService(entityManagerType);
                if (manager != null && manager is IEntityManager castedManager)
                    return castedManager;
                throw new Exception();
            }
            catch (Exception)
            {
                throw new MissingMemberException($"Could not find EntityManager of type {entityManagerType.FullName}");
            }
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
        #endregion
    }
}

