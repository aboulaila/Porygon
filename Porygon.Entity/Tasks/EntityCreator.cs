using System.Data;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Relationships;
using Porygon.Entity.Utils;

namespace Porygon.Entity.Tasks
{
    public class EntityCreator : EntityManipulator
    { 

        public EntityCreator(IEntityManager entityManager, IServiceProvider serviceProvider) : base (entityManager, serviceProvider)
		{
            entityEventsHook = entityManager.GetCreationHook();
        }

        protected override async Task Pre(IDbTransaction? transaction)
        {
            await new RelationshipVisitor(entity!)
                .Handler(CheckRelatedEntityCreation)
                .Type(RelationshipType.HasA)
                .ServiceProvider(serviceProvider!)
                .Visit();
        }

        protected override void Manipulate(IDbTransaction? transaction)
        {
            if (transaction == null)
                dataManager.Insert(entity!);
            else
                dataManager.Insert(entity!, transaction);
        }

        protected override async Task Post(IDbTransaction? transaction)
        {
            await new RelationshipVisitor(entity!)
                .Handler(CheckRelatedEntityCreation)
                .Type(RelationshipType.HasMany)
                .ServiceProvider(serviceProvider!)
                .Visit();
        }

        private async Task CheckRelatedEntityCreation(Relationship relationship, IKeyEntity<Guid> entity)
        {
            if (relationship.IsHasMany)
                entity.LinkedItemId = base.entity!.Id;

            Guid relatedEntityId = await CreateRelatedEntityIfNeeded(relationship, entity);

            if (relationship.IsHasA && !Equals(relatedEntityId, default))
                base.entity!.SetObjectProperty(relationship.PropertyIdName!, relatedEntityId!);
        }
    }
}

