namespace Porygon.Entity
{
    public class EntityFilter
    {
        public Guid LinkedItemId { get; set; }
        public string Criteria { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
