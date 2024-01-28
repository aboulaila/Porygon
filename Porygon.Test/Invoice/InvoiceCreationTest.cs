using System.Data;
using Moq;
using Porygon.Entity;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Manager;

namespace Porygon.Test.Invoice
{
    public class InvoiceCreationTest
    {
        private EntityManager<Invoice> InvoiceManager { get; set; }
        private EntityManager<Customer> CustomerManager { get; set; }
        private EntityManager<InvoiceItem> InvoiceItemManager { get; set; }
        private EntityManager<Product> ProductManager { get; set; }
        private EntityManager<ContactDetail> ContactDetailManager { get; set; }
        private EntityManager PaymentManager { get; set; }

        private IEntityDataManager<Invoice> InvoiceDataManager = Mock.Of<IEntityDataManager<Invoice>>();
        private IEntityDataManager<Customer> CustomerDataManager = Mock.Of<IEntityDataManager<Customer>>();
        private IEntityDataManager<InvoiceItem> InvoiceItemDataManager = Mock.Of<IEntityDataManager<InvoiceItem>>();
        private IEntityDataManager<Product> ProductDataManager = Mock.Of<IEntityDataManager<Product>>();
        private IEntityDataManager<ContactDetail> ContactDetailDataManager = Mock.Of<IEntityDataManager<ContactDetail>>();
        private IEntityDataManager PaymentDataManager = Mock.Of<IEntityDataManager>();

        [SetUp]
        public void Setup()
        {
            Mock<IServiceProvider> serviceProvider = new();
            Mock<IDbConnectionProvider> dbConnectionProvider = TestUtils.SetupDbConnectionProvider();

            InvoiceManager = new EntityManager<Invoice>(InvoiceDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            CustomerManager = new EntityManager<Customer>(CustomerDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            InvoiceItemManager = new EntityManager<InvoiceItem>(InvoiceItemDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            ProductManager = new EntityManager<Product>(ProductDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            ContactDetailManager = new EntityManager<ContactDetail>(ContactDetailDataManager, serviceProvider.Object, dbConnectionProvider.Object);
            PaymentManager = new EntityManager(PaymentDataManager, serviceProvider.Object, dbConnectionProvider.Object);

            MockServiceProvider(serviceProvider);
        }

        [Test]
        public async Task Test1()
        {
            ContactDetail contactDetail = new()
            {
                PhoneNumber = "1"
            };

            Product product = new()
            {
                Id = Guid.NewGuid(),
                Type = "Candy"
            };

            Customer customer = new()
            {
                Name = "John"
            };

            InvoiceItem invoiceItem = new()
            {
                Product = product
            };

            Invoice invoice = new()
            {
                Code = "1",
                Customer = customer,
                Items = new List<InvoiceItem>
                {
                    invoiceItem
                },
                Payments = new List<PoryEntity> { new PoryEntity(), new PoryEntity(), new PoryEntity() }
            };

            Invoice? createdInvoice = await InvoiceManager.Create(invoice);

            Assert.Multiple(() =>
            {
                TestUtils.AssertIdNotEmpty(invoice);
                TestUtils.AssertIdNotEmpty(customer);
                TestUtils.AssertIdNotEmpty(product);
                TestUtils.AssertIdNotEmpty(invoiceItem);
            });

            Assert.Multiple(() =>
            {
                Assert.That(createdInvoice, Is.Not.Null);
                Assert.That(createdInvoice!.CustomerId, Is.EqualTo(customer.Id));
                Assert.That(customer.ContactDetailId, Is.EqualTo(Guid.Empty));
                Assert.That(invoiceItem.ProductId, Is.EqualTo(product.Id));
                Assert.That(invoiceItem.LinkedItemId, Is.EqualTo(invoice.Id));
                Assert.That(invoice.Payments[0].LinkedItemId, Is.EqualTo(invoice.Id));
                Assert.That(invoice.Payments[1].LinkedItemId, Is.EqualTo(invoice.Id));
                Assert.That(invoice.Payments[2].LinkedItemId, Is.EqualTo(invoice.Id));
            });

            Mock.Get(PaymentDataManager).Verify(x => x.Insert(It.IsAny<PoryEntity>(), It.IsAny<IDbTransaction>()), Times.Exactly(3));
            Mock.Get(InvoiceDataManager).Verify(x => x.Insert(invoice, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(CustomerDataManager).Verify(x => x.Insert(customer, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(InvoiceItemDataManager).Verify(x => x.Insert(invoiceItem, It.IsAny<IDbTransaction>()), Times.Once);
            Mock.Get(ContactDetailDataManager).Verify(x => x.Insert(contactDetail, It.IsAny<IDbTransaction>()), Times.Never);
            Mock.Get(ProductDataManager).Verify(x => x.Insert(product, It.IsAny<IDbTransaction>()), Times.Never);
        }

        [Test]
        public async Task Test2()
        {
            var payments = new List<ContactDetail> { new ContactDetail(), new ContactDetail(), new ContactDetail() };
            await ContactDetailManager.CreateBulk(payments);
            Mock.Get(ContactDetailDataManager).Verify(x => x.Insert(It.IsAny<ContactDetail>(), It.IsAny<IDbTransaction>()), Times.Exactly(3));
        }

        [Test]
        public void Test3()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await InvoiceManager.Create(null));
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