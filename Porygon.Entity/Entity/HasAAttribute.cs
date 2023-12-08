namespace Porygon.Entity.Entity
{
    public class HasAAttribute : RelationshipAttribute
    {
        public HasAAttribute(string? entityIdProperty = null, Type? manager = null, bool isCascading = false) : base(entityIdProperty, manager, isCascading)
        {
        }
    }
}
