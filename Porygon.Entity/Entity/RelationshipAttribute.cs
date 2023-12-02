using Porygon.Entity.Manager;

namespace Porygon.Entity.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class RelationshipAttribute : Attribute
    {
        public Type? EntityManager { get; set; }

        public RelationshipAttribute()
        {            
        }
        public RelationshipAttribute(Type manager)
        {
            EntityManager = manager;
        }
    }
}
