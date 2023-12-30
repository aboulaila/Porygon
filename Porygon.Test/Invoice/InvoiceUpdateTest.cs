using Moq;
using Porygon.Entity;
using Porygon.Entity.Data;
using Porygon.Entity.Manager;
using System.Data;

namespace Porygon.Test.Invoice
{
    public class InvoiceUpdateTest
    {
        private EntityManager<Invoice> InvoiceManager { get; set; }
        private EntityManager<Customer> CustomerManager { get; set; }
        private EntityManager<InvoiceItem> InvoiceItemManager { get; set; }
        private EntityManager<Product> ProductManager { get; set; }
        private EntityManager<ContactDetail> ContactDetailManager { get; set; }
        private EntityManager PaymentManager { get; set; }

        private IEntityDataManager<Invoice> InvoiceDataManager = Mock.Of<IEntityDataManager<Invoice>>();
        private IEntityDataManager<Customer> CustomerDataManager = Mock.Of<IEntityDataManager<Customer>>();
        private IEntityDataManager<Product> ProductDataManager = Mock.Of<IEntityDataManager<Product>>();
        private IEntityDataManager<ContactDetail> ContactDetailDataManager = Mock.Of<IEntityDataManager<ContactDetail>>();
        private Mock<IEntityDataManager<InvoiceItem>> InvoiceItemDataManager = new();
        private Mock<IEntityDataManager> PaymentDataManager = new();

        [SetUp]
        public void Setup()
        {
            Mock<IServiceProvider> serviceProvider = new();
            InvoiceItemDataManager.Setup(x => x.Delete(It.IsAny<Guid>(), It.IsAny<IDbTransaction>())).Returns(1);
            PaymentDataManager.Setup(x => x.Delete(It.IsAny<Guid>(), It.IsAny<IDbTransaction>())).Returns(1);

            Mock<IDbConnectionProvider> dbConnectionProvider = TestUtils.SetupDbConnectionProvider();
            InvoiceManager = new EntityManager<Invoice>(InvoiceDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            CustomerManager = new EntityManager<Customer>(CustomerDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            InvoiceItemManager = new EntityManager<InvoiceItem>(InvoiceItemDataManager.Object, serviceProvider.Object, dbConnectionProvider.Object);
            ProductManager = new EntityManager<Product>(ProductDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            ContactDetailManager = new EntityManager<ContactDetail>(ContactDetailDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            PaymentManager = new EntityManager(PaymentDataManager.Object, serviceProvider.Object, dbConnectionProvider.Object);

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

            var payment = new PoryEntity() { Id = Guid.Parse("1821C8B8-379F-47C6-A155-4E892A9D2ED6"), State = Entity.Entity.EntityStates.DELETED };

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
                    new PoryEntity() { Id = Guid.Parse("96E444A8-679E-453D-82A7-7789AD00F1D3"), State = Entity.Entity.EntityStates.UPDATED },
                    new PoryEntity(),
                    payment
                }
            };

            InvoiceItemDataManager.Setup(x => x.GetAsync(Guid.Parse("F518B860-3EE8-4451-A9AA-B8C31872FD17"))).Returns(Task.FromResult(invoiceItem2));
            PaymentDataManager.Setup(x => x.GetAsync(Guid.Parse("1821C8B8-379F-47C6-A155-4E892A9D2ED6"))).Returns(Task.FromResult(payment));

            await InvoiceManager.Update(invoice);

            Assert.Multiple(() =>
            {
                TestUtils.AssertIdNotEmpty(invoice);
                TestUtils.AssertIdNotEmpty(customer);
                TestUtils.AssertIdNotEmpty(product);
                TestUtils.AssertIdNotEmpty(invoiceItem);
            });

            Mock.Get(InvoiceDataManager).Verify(x => x.Insert(invoice, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(InvoiceDataManager).Verify(x => x.Update(invoice, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(InvoiceDataManager).Verify(x => x.Delete(invoice.Id, It.IsAny<IDbTransaction>()), Times.Never);

            Mock.Get(ContactDetailDataManager).Verify(x => x.Insert(contactDetail, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(ContactDetailDataManager).Verify(x => x.Update(contactDetail, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(ContactDetailDataManager).Verify(x => x.Delete(contactDetail.Id, It.IsAny<IDbTransaction>()), Times.Never);

            Mock.Get(ProductDataManager).Verify(x => x.Insert(product, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(ProductDataManager).Verify(x => x.Update(product, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(ProductDataManager).Verify(x => x.Delete(product.Id, It.IsAny<IDbTransaction>()), Times.Never);

            Mock.Get(CustomerDataManager).Verify(x => x.Insert(customer, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(CustomerDataManager).Verify(x => x.Update(customer, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(CustomerDataManager).Verify(x => x.Delete(customer.Id, It.IsAny<IDbTransaction>()), Times.Never);

            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Insert(invoiceItem, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Update(invoiceItem, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Delete(invoiceItem.Id, It.IsAny<IDbTransaction>()), Times.Never);

            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Insert(invoiceItem2, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Update(invoiceItem2, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(InvoiceItemDataManager.Object).Verify(x => x.Delete(invoiceItem2.Id, It.IsAny<IDbTransaction>()), Times.Once);

            Mock.Get(PaymentDataManager.Object).Verify(x => x.Insert(It.IsAny<PoryEntity>(), It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(PaymentDataManager.Object).Verify(x => x.Update(It.IsAny<PoryEntity>(), It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(PaymentDataManager.Object).Verify(x => x.Delete(It.IsAny<Guid>(), It.IsAny<IDbTransaction>()), Times.Once);
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