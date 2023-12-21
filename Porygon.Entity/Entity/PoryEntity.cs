using Porygon.Entity.Entity;

namespace Porygon.Entity
{
    public class PoryEntity<TKey>
    {
        public TKey? Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public TKey? LinkedItemId { get; set; }
        public DateTimeOffset? DateModified { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public int TotalRecords { get; set; }
        public EntityStates State { get; set; } = EntityStates.UNMODIFIED;

        public virtual void Enrich(bool isNew)
        {
            if (isNew)
            {
                DateCreated = DateTimeOffset.UtcNow;
            }
            DateModified = DateTimeOffset.UtcNow;
        }

        public virtual TModel ToViewModel<TModel>() where TModel : PoryEntity<TKey>
        {
            return (TModel)this;
        }
    }

    public class PoryEntity : PoryEntity<Guid>
    {
        public override void Enrich(bool isNew)
        {
            base.Enrich(isNew);
            if (isNew)
            {
                Id = Id.Equals(Guid.Empty) ? Guid.NewGuid() : Id;
            }
        }
    }
}
