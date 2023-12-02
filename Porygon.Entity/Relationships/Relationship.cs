using Porygon.Entity.Manager;

namespace Porygon.Entity.Relationships
{
    public class Relationship
    {
        public Type Entity { get; set; }
        public object EntityInstance { get; set; }
        public IEntityManager EntityManager { get; set; }
        public RelationshipType Type { get; set; }
        public bool IsHasA => Equals(Type, RelationshipType.HasA);
        public bool IsHasMany => Equals(Type, RelationshipType.HasMany);

        private Relationship(RelationshipType type, object entityInstance, Type entity, IEntityManager entityManager)
        {
            EntityInstance = entityInstance;
            Entity = entity;
            EntityManager = entityManager;
            Type = type;
        }

        public static Relationship HasA(object entityInstance, Type entity, IEntityManager entityManager)
        {
            return new Relationship(RelationshipType.HasA, entityInstance, entity, entityManager);
        }

        public static Relationship HasMany(object entityInstance, Type entity, IEntityManager entityManager)
        {
            return new Relationship(RelationshipType.HasMany, entityInstance, entity, entityManager);
        }
    }

    public enum RelationshipType
    {
        HasA,
        HasMany
    }
}
