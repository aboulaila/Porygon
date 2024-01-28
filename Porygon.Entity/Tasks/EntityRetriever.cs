using Porygon.Entity.Interfaces;
using Porygon.Entity.Relationships;
using Porygon.Entity.Utils;

namespace Porygon.Entity.Tasks
{
    public class EntityRetriever<T, TModel> : EntityTask
        where T : class, IKeyEntity<Guid>
	{
        private bool shouldEnrich = false;
        public EntityRetriever(IEntityManager entityManager, IServiceProvider serviceProvider) : base(entityManager, serviceProvider)
        {
        }

        public EntityRetriever<T, TModel> ShouldEnrich()
        {
            shouldEnrich = true;
            return this;
        }

        public async Task<TModel?> Get(Guid id)
        {
            if (EntityHelper.EmptyId(id))
                return default;

            T? entity = await dataManager.GetAsync<T>(id);
            if (entity == null)
                return default;

            return await EnrichAndConvertToViewModel(entity!);
        }

        public async Task<List<TModel>> Get()
        {
            var results = await dataManager.GetAll<T>();
            return await ToViewModel(results);
        }

        protected async Task<List<TModel>> ToViewModel(List<T> results)
        {
            if (!Extensions.IsNullOrEmpty(results))
            {
                return await results.ParallelForEach(EnrichAndConvertToViewModel);
            }

            return new List<TModel>();
        }

        private async Task<TModel?> EnrichAndConvertToViewModel(T entity)
        {
            if (shouldEnrich)
            {
                await EnrichEntity(entity);
            }

            return (TModel)entity.ToViewModel();
        }

        private async Task EnrichEntity(T entity)
        {
            var relationships = new RelationshipVisitor(entity)
                .ServiceProvider(serviceProvider!)
                .GetRelationships();

            if (Extensions.IsNullOrEmpty(relationships))
                return;

            foreach (var relationship in relationships)
            {
                await EnrichRelatedEntities(entity, relationship);
            }
        }

        private async Task EnrichRelatedEntities(IKeyEntity<Guid> entity, Relationship relationship)
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
        private async Task EnrichHasManyEntityList(IKeyEntity<Guid> entity, Relationship relationship)
        {
            var relatedEntityList = await relationship.EntityManager!.GetLinkedItems(entity.Id!);
            if (Extensions.IsNullOrEmpty(relatedEntityList))
                return;

            relationship.EntityInstance = relatedEntityList;
            entity.GetType().GetProperty(relationship.PropertyName!)?.SetValue(entity, relatedEntityList, null);
        }

        private async Task EnrichHasAEntity(IKeyEntity<Guid> entity, Relationship relationship)
        {
            Guid id = relationship.EntityId
                        ?? throw new ArgumentNullException("Property of entity id is null");

            var relatedEntity = await relationship.EntityManager!.GetSingle(id);
            if (relatedEntity == null)
                return;

            relationship.EntityId = id;
            relationship.EntityInstance = relatedEntity;
            entity.GetType().GetProperty(relationship.PropertyName!)?.SetValue(entity, relatedEntity, null);
            entity.GetType().GetProperty(relationship.PropertyIdName!)?.SetValue(entity, id, null);
        }
    }
}

