namespace Porygon.Entity.Entity
{
    public class HasManyAttribute : RelationshipAttribute
    {
        public HasManyAttribute(string? entityIdProperty = null, Type? manager = null, bool isCascading = false) : base(entityIdProperty, manager, isCascading)
        {
        }
    }
}
