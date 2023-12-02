using Porygon.Entity.Manager;

namespace Porygon.Entity.Entity
{
    public class HasManyAttribute : RelationshipAttribute
    {
        public HasManyAttribute()
        {            
        }

        public HasManyAttribute(Type manager) : base(manager)
        {
        }
    }
}
