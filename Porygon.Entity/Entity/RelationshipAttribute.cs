using Porygon.Entity.Manager;

namespace Porygon.Entity.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class RelationshipAttribute : Attribute
    {
        public Type? EntityManager { get; set; }
        public bool IsCascading { get; set; }        

        public RelationshipAttribute(Type? manager, bool isCascading)
        {
            EntityManager = manager;
            IsCascading = isCascading;
        }
    }
}
