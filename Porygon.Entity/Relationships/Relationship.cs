using Porygon.Entity.Manager;

namespace Porygon.Entity.Relationships
{
    public class Relationship
    {
        public Type? Entity { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyIdName { get; set; }
        public object? EntityId { get; set; }
        public object? EntityInstance { get; set; }
        public IEntityManager? EntityManager { get; set; }
        public RelationshipType? Type { get; set; }
        public bool IsHasA => Equals(Type, RelationshipType.HasA);
        public bool IsHasMany => Equals(Type, RelationshipType.HasMany);
    }

    public enum RelationshipType
    {
        HasA,
        HasMany
    }
}
