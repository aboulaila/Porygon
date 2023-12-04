using Porygon.Entity.Manager;

namespace Porygon.Entity.Entity
{
    public class HasManyAttribute : RelationshipAttribute
    {
        public HasManyAttribute(Type? manager = null, bool isCascading = false) : base(manager, isCascading)
        {
        }
    }
}
