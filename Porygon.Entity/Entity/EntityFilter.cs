using Porygon.Entity.Interfaces;

namespace Porygon.Entity
{
    public class EntityFilter<TKey> : IEntityFilter<TKey>
    {
        public TKey LinkedItemId { get; set; }
        public string Criteria { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class EntityFilter : EntityFilter<Guid>
    {
    }
}
