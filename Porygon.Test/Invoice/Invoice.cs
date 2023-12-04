using Porygon.Entity;
using Porygon.Entity.Entity;
using Porygon.Entity.Manager;

namespace Porygon.Test.Invoice
{
    public class Invoice : PoryEntity
    {
        public string Code { get; set; }

        [HasA(typeof(EntityManager<Customer>))]
        public Customer Customer { get; set; }

        [HasMany(typeof(EntityManager<InvoiceItem>), true)]
        public List<InvoiceItem> Items { get; set; }

        [HasMany(isCascading: true)]
        public List<PoryEntity> Payments { get; set; }
    }

    public class Customer : PoryEntity
    {
        public string Name { get; set; }
        [HasA(isCascading: true)]
        public ContactDetail ContactDetail { get; set; }
    }

    public class ContactDetail : PoryEntity
    {
        public string PhoneNumber { get; set; }
    }

    public class InvoiceItem : PoryEntity
    {
        [HasA]
        public Product Product { get; set; }
    }

    public class Product : PoryEntity
    {
        public string Type { get; set; }
    }
}
