using Porygon.Entity.Entity;

namespace Porygon.Entity
{
    public interface IKeyEntity<TKey>
    {
        public TKey? Id { get; set; }
        public EntityStates State { get; set; }
        public TKey? LinkedItemId { get; set; }
        public void Enrich(bool isNew);
        public IKeyEntity<TKey> ToViewModel();
    }
}
