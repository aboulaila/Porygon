using System.Data;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Utils;

namespace Porygon.Entity.Tasks
{
    public class EntityDeleter : EntityManipulator
    {
		public EntityDeleter(IEntityManager entityManager, IServiceProvider serviceProvider) : base(entityManager, serviceProvider)
        {
        }

        protected override void Manipulate(IDbTransaction? transaction)
        {
            if (transaction == null)
                dataManager.Delete(entity!.Id!);
            else
                dataManager.Delete(entity!.Id!, transaction);
        }

        protected override Task Post(IDbTransaction? transaction)
        {
            return Task.CompletedTask;
        }

        protected override async Task Pre(IDbTransaction? transaction)
        {
            await new RelationshipVisitor(entity!)
                .Handler(DeleteRelatedEntity)
                .IsCascading()
                .ServiceProvider(serviceProvider!)
                .Visit();
        }
    }
}

