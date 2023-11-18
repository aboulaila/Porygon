namespace Porygon.Entity
{
    public abstract class PoryEntity<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public int TotalRecords { get; set; }

        public abstract void Enrich(bool isNew);
    }

    public class PoryEntity : PoryEntity<Guid>
    {
        public override void Enrich(bool isNew)
        {
            if (isNew)
            {
                Id = Guid.NewGuid();
                DateCreated = DateTimeOffset.UtcNow;
            }
            DateModified = DateTimeOffset.UtcNow;
        }
    }
}
