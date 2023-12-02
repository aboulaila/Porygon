using Porygon.Entity.Manager;

namespace Porygon.Entity.Entity
{
    public class HasAAttribute : RelationshipAttribute
    {
        public HasAAttribute()
        {            
        }

        public HasAAttribute(Type manager) : base(manager)
        {
        }
    }
}
