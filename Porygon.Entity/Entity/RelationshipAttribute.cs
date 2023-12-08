namespace Porygon.Entity.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class RelationshipAttribute : Attribute
    {
        public string? EntityIdProperty { get; set; }
        public Type? EntityManager { get; set; }
        public bool IsCascading { get; set; }
        public bool IsHasA => this is HasAAttribute;
        public bool IsHasMany => this is HasManyAttribute;

        public RelationshipAttribute(string? entityIdProperty, Type? manager, bool isCascading)
        {
            EntityIdProperty = entityIdProperty;
            EntityManager = manager;
            IsCascading = isCascading;
        }
    }
}
