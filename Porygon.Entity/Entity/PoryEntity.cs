using FreeSql.DataAnnotations;
using Porygon.Entity.Entity;

namespace Porygon.Entity
{
    public class PoryEntity<TKey> : IKeyEntity<TKey>
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public TKey? Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public TKey? LinkedItemId { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateCreated { get; set; }
        public int TotalRecords { get; set; }
        public EntityStates State { get; set; } = EntityStates.UNMODIFIED;

        public virtual void Enrich(bool isNew)
        {
            if (isNew)
            {
                DateCreated = DateTime.UtcNow;
            }
            DateModified = DateTime.UtcNow;
            State = EntityStates.UNMODIFIED;
        }

        public virtual IKeyEntity<TKey> ToViewModel()
        {
            return this;
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
