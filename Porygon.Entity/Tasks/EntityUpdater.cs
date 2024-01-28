using System.Data;
using Porygon.Entity.Entity;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Relationships;
using Porygon.Entity.Utils;

namespace Porygon.Entity.Tasks
{
    public class EntityUpdater : EntityManipulator
    {
		public EntityUpdater(IEntityManager entityManager, IServiceProvider serviceProvider) : base(entityManager, serviceProvider)
        {
        }

        protected override void Manipulate(IDbTransaction? transaction)
        {
            if(transaction == null)
                dataManager.Update(entity!);
            else
                dataManager.Update(entity!, transaction);
        }

        protected override async Task Post(IDbTransaction? transaction)
        {
            await new RelationshipVisitor(entity!)
                .Handler(ManipulateRelatedEntity)
                .Type(RelationshipType.HasMany)
                .ServiceProvider(serviceProvider!)
                .Visit();
        }

        protected override async Task Pre(IDbTransaction? transaction)
        {
            await new RelationshipVisitor(entity!)
                .Handler(ManipulateRelatedEntity)
                .Type(RelationshipType.HasA)
                .ServiceProvider(serviceProvider!)
                .Visit();
        }

        private async Task ManipulateRelatedEntity(Relationship relationship, IKeyEntity<Guid> entity)
        {
            if (relationship.IsHasMany)
                entity.LinkedItemId = base.entity!.Id;

            Guid relatedEntityId = await ManipulateRelatedEntityIfNeeded(relationship, entity);

            if (relationship.IsHasA && !Equals(relatedEntityId, default))
                base.entity!.SetObjectProperty(relationship.PropertyIdName!, relatedEntityId!);
        }

        private async Task<Guid> ManipulateRelatedEntityIfNeeded(Relationship relationship, IKeyEntity<Guid> entity)
        {
            if (entity.ShouldCreateEntity<Guid>())
            {
                return await CreateRelatedEntity(relationship, entity);
            }
            else if (Equals(entity.State, EntityStates.UPDATED))
            {
                entity.LinkedItemId = base.entity!.Id;
                return await UpdateRelatedEntity(relationship, entity);
            }
            else if (Equals(entity.State, EntityStates.DELETED))
            {
                return await DeleteRelatedEntity(relationship, entity);
            }
            return entity.Id;
        }
    }
}

