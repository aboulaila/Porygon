namespace Porygon.Entity
{
    public class EntityFilter<TKey>
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
