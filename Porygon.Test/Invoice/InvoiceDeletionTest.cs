using Moq;
using Porygon.Entity;
using Porygon.Entity.Data;
using Porygon.Entity.Manager;

namespace Porygon.Test.Invoice
{
    public class InvoiceDeletionTest
    {
        private EntityManager<Invoice> InvoiceManager { get; set; }
        private EntityManager<Customer> CustomerManager { get; set; }
        private EntityManager<InvoiceItem> InvoiceItemManager { get; set; }
        private EntityManager<Product> ProductManager { get; set; }
        private EntityManager<ContactDetail> ContactDetailManager { get; set; }
        private EntityManager PaymentManager { get; set; }

        private Mock<IEntityDataManager<Invoice>> InvoiceDataManager = new();
        private Mock<IEntityDataManager<Customer>> CustomerDataManager = new();
        private Mock<IEntityDataManager<Product>> ProductDataManager = new();
        private Mock<IEntityDataManager<ContactDetail>> ContactDetailDataManager = new();
        private Mock<IEntityDataManager<InvoiceItem>> InvoiceItemDataManager = new();
        private Mock<IEntityDataManager> PaymentDataManager = new();

        [SetUp]
        public void Setup()
        {
            Mock<IServiceProvider> serviceProvider = new();

            InvoiceManager = new EntityManager<Invoice>(InvoiceDataManager.Object, serviceProvider.Object);
            CustomerManager = new EntityManager<Customer>(CustomerDataManager.Object, serviceProvider.Object);
            InvoiceItemManager = new EntityManager<InvoiceItem>(InvoiceItemDataManager.Object, serviceProvider.Object);
            ProductManager = new EntityManager<Product>(ProductDataManager.Object, serviceProvider.Object);
            ContactDetailManager = new EntityManager<ContactDetail>(ContactDetailDataManager.Object, serviceProvider.Object);
            PaymentManager = new EntityManager(PaymentDataManager.Object, serviceProvider.Object);

            MockServiceProvider(serviceProvider);
        }

        [Test]
        public async Task Test1()
        {
            ContactDetail contactDetail = new()
            {
                Id = Guid.Parse("70b810af-0d04-4e13-9fcc-c669d41df4c2"),
                PhoneNumber = "1"
            };

            Product product = new()
            {
                Id = Guid.Parse("951182D1-AA05-4C8E-B748-ED52AF6218FC"),
                Type = "Candy",
                State = Entity.Entity.EntityStates.NEW
            };

            Customer customer = new()
            {
                Id = Guid.Parse("311C8F27-406F-4C76-B248-3165EB92C327"),
                Name = "John",
                State = Entity.Entity.EntityStates.UPDATED
            };

            InvoiceItem invoiceItem = new()
            {
                Id = Guid.Parse("445F531D-0172-43D9-99B9-CDEDD6C48595"),
                Product = product,
                State = Entity.Entity.EntityStates.UPDATED
            };

            InvoiceItem invoiceItem2 = new()
            {
                Id = Guid.Parse("F518B860-3EE8-4451-A9AA-B8C31872FD17"),
                Product = product,
                State = Entity.Entity.EntityStates.DELETED
            };

            PoryEntity payment = new() { Id = Guid.Parse("1821C8B8-379F-47C6-A155-4E892A9D2ED6") };
            PoryEntity payment1 = new() { Id = Guid.Parse("96E444A8-679E-453D-82A7-7789AD00F1D3") };

            Invoice invoice = new()
            {
                Id = Guid.Parse("7295F9D6-0989-4C4D-8EE3-88AF81E25F57"),
                Code = "1",
                Customer = customer,
                Items = new List<InvoiceItem>
                {
                    invoiceItem, invoiceItem2
                },
                Payments = new List<PoryEntity> {
                    payment,
                    payment1
                }
            };

            TestUtils.SetupEntityGetter(contactDetail, ContactDetailDataManager);
            TestUtils.SetupEntityGetter(payment, PaymentDataManager);
            TestUtils.SetupEntityGetter(payment1, PaymentDataManager);
            TestUtils.SetupEntityGetter(invoiceItem, InvoiceItemDataManager);
            TestUtils.SetupEntityGetter(invoiceItem2, InvoiceItemDataManager);
            TestUtils.SetupEntityGetter(invoice, InvoiceDataManager);

            ContactDetailDataManager.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(1);
            InvoiceItemDataManager.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(1);
            InvoiceDataManager.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(1);
            PaymentDataManager.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(1);

            await InvoiceManager.Delete(invoice.Id);

            Mock.Get(InvoiceDataManager.Object).Verify(x => x.Insert(invoice), Times.Never);
            Mock.Get(InvoiceDataManager.Object).Verify(x => x.Update(invoice), Times.Never);
            Mock.Get(InvoiceDataManager.Object).Verify(x => x.Delete(invoice.Id), Times.Once);

            Mock.Get(ContactDetailDataManager.Object).Verify(x => x.Insert(contactDetail), Times.Never);
            Mock.Get(ContactDetailDataManager.Object).Verify(x => x.Update(contactDetail), Times.Never);
            Mock.Get(ContactDetailDataManager.Object).Verify(x => x.Delete(contactDetail.Id), Times.Never);

            Mock.Get(ProductDataManager.Object).Verify(x => x.Insert(product), Times.Never);
            Mock.Get(ProductDataManager.Object).Verify(x => x.Update(product), Times.Never);
            Mock.Get(ProductDataManager.Object).Verify(x => x.Delete(product.Id), Times.Never);

            Mock.Get(CustomerDataManager.Object).Verify(x => x.Insert(customer), Times.Never);
            Mock.Get(CustomerDataManager.Object).Verify(x => x.Update(customer), Times.Never);
            Mock.Get(CustomerDataManager.Object).Verify(x => x.Delete(customer.Id), Times.Never);

            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Insert(invoiceItem), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Update(invoiceItem), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Delete(invoiceItem.Id), Times.Once);

            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Insert(invoiceItem2), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Update(invoiceItem2), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Delete(invoiceItem2.Id), Times.Once);

            Mock.Get(PaymentDataManager.Object).Verify(x => x.Insert(It.IsAny<PoryEntity>()), Times.Never);
            Mock.Get(PaymentDataManager.Object).Verify(x => x.Update(It.IsAny<PoryEntity>()), Times.Never);
            Mock.Get(PaymentDataManager.Object).Verify(x => x.Delete(It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Test]
        public void Test2()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await InvoiceManager.Update(null));
        }


        private void MockServiceProvider(Mock<IServiceProvider> serviceProvider)
        {
            SetupMockGetService<EntityManager<Invoice>>(serviceProvider, InvoiceManager);
            SetupMockGetService<EntityManager<Customer>>(serviceProvider, CustomerManager);
            SetupMockGetService<EntityManager<InvoiceItem>>(serviceProvider, InvoiceItemManager);
            SetupMockGetService<EntityManager<Product>>(serviceProvider, ProductManager);
            SetupMockGetService<EntityManager<ContactDetail>>(serviceProvider, ContactDetailManager);
            SetupMockGetService<EntityManager<PoryEntity>>(serviceProvider, PaymentManager);
        }

        private void SetupMockGetService<T>(Mock<IServiceProvider> serviceProvider, IEntityManager manager)
        {
            serviceProvider.Setup(x =>
                            x.GetService(typeof(T)))
                            .Returns(manager);
        }
    }
}