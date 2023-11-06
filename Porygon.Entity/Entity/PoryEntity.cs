namespace Porygon.Entity
{
    public class PoryEntity<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public int TotalRecords { get; set; }
    }

    public class PoryEntity : PoryEntity<Guid>
    {

    }
}
