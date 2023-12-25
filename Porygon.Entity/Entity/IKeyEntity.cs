namespace Porygon.Entity
{
    public interface IKeyEntity<TKey>
    {
        public TKey? Id { get; set; }
    }
}
