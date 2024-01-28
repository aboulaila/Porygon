namespace Porygon.Entity.Interfaces
{
    public interface IEntityFilter<TKey>
	{
        public TKey LinkedItemId { get; set; }
        public string Criteria { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}

