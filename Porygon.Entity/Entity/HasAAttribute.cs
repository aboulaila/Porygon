using Porygon.Entity.Manager;

namespace Porygon.Entity.Entity
{
    public class HasAAttribute : RelationshipAttribute
    {
        public HasAAttribute(Type? manager = null, bool isCascading = false) : base(manager, isCascading)
        {
        }
    }
}
