using Moq;
using Porygon.Entity.Data;
using Porygon.Entity.Manager;
using Porygon.Entity;

namespace Porygon.Test.Invoice
{
    internal class InvoiceGetTest
    {
        private EntityManager<Invoice> InvoiceManager { get; set; }
        private EntityManager<Customer> CustomerManager { get; set; }
        private EntityManager<InvoiceItem> InvoiceItemManager { get; set; }
        private EntityManager<Product> ProductManager { get; set; }
        private EntityManager<ContactDetail> ContactDetailManager { get; set; }
        private EntityManager PaymentManager { get; set; }

        private Mock<IServiceProvider> ServiceProvider = new();
        private Mock<IEntityDataManager<Invoice>> InvoiceDataManager = new();
        private Mock<IEntityDataManager<Customer>> CustomerDataManager = new();
        private Mock<IEntityDataManager<Product>> ProductDataManager = new();
        private Mock<IEntityDataManager<ContactDetail>> ContactDetailDataManager = new();
        private Mock<IEntityDataManager<InvoiceItem>> InvoiceItemDataManager = new();
        private Mock<IEntityDataManager> PaymentDataManager = new();

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
                Type = "Candy"
            };

            Customer customer = new()
            {
                Id = Guid.Parse("311C8F27-406F-4C76-B248-3165EB92C327"),
                ContactDetailId = contactDetail.Id,
                Name = "John"
            };

            InvoiceItem invoiceItem = new()
            {
                Id = Guid.Parse("445F531D-0172-43D9-99B9-CDEDD6C48595"),
                ProductId = product.Id
            };

            PoryEntity payment = new() { Id = Guid.Parse("1821C8B8-379F-47C6-A155-4E892A9D2ED6") };
            PoryEntity payment1 = new() { Id = Guid.Parse("96E444A8-679E-453D-82A7-7789AD00F1D3") };

            Invoice invoice = new()
            {
                Id = Guid.Parse("7295F9D6-0989-4C4D-8EE3-88AF81E25F57"),
                Code = "1",
                CustomerId = customer.Id
            };

            TestUtils.SetupEntityGetter(contactDetail, ContactDetailDataManager);
            TestUtils.SetupEntityGetter(product, ProductDataManager);
            TestUtils.SetupEntityGetter(customer, CustomerDataManager);
            TestUtils.SetupEntityGetter(invoiceItem, InvoiceItemDataManager);
            TestUtils.SetupEntityGetter(payment, PaymentDataManager);
            TestUtils.SetupEntityGetter(payment1, PaymentDataManager);
            InvoiceDataManager.Setup(x => x.GetAsync(invoice.Id)).Returns(() => Task.FromResult(invoice.Clone()));

            var itemsList = new List<InvoiceItem>() { invoiceItem };
            var paymentList = new List<PoryEntity>() { payment, payment1 };

            InvoiceItemDataManager.Setup(x => x.Search(It.IsAny<EntityFilter>())).Returns(Task.FromResult(itemsList));
            PaymentDataManager.Setup(x => x.Search(It.IsAny<EntityFilter>())).Returns(Task.FromResult(paymentList));

            InvoiceManager = new EntityManager<Invoice>(InvoiceDataManager.Object, ServiceProvider.Object);
            CustomerManager = new EntityManager<Customer>(CustomerDataManager.Object, ServiceProvider.Object);
            InvoiceItemManager = new EntityManager<InvoiceItem>(InvoiceItemDataManager.Object, ServiceProvider.Object);
            ProductManager = new EntityManager<Product>(ProductDataManager.Object, ServiceProvider.Object);
            ContactDetailManager = new EntityManager<ContactDetail>(ContactDetailDataManager.Object, ServiceProvider.Object);
            PaymentManager = new EntityManager(PaymentDataManager.Object, ServiceProvider.Object);

            MockServiceProvider(ServiceProvider);

            Invoice? entity = await InvoiceManager.GetEnriched(Guid.Parse("7295F9D6-0989-4C4D-8EE3-88AF81E25F57"));

            Assert.Multiple(() =>
            {
                Assert.That(entity, Is.Not.Null);
                Assert.That(entity.Customer, Is.EqualTo(customer));
                Assert.That(entity.Customer.ContactDetail, Is.EqualTo(contactDetail));
            });

            Assert.Multiple(() =>
            {
                Assert.That(entity.Items, Is.Not.Null);
                Assert.That(entity.Items, Has.Count.EqualTo(1));
                Assert.That(entity.Items[0], Is.EqualTo(invoiceItem));
                Assert.That(entity.Items[0].Product, Is.EqualTo(product));
            });

            Assert.Multiple(() =>
            {
                Assert.That(entity.Payments, Is.Not.Null);
                Assert.That(entity.Payments, Has.Count.EqualTo(2));
                Assert.That(entity.Payments[0], Is.EqualTo(payment));
                Assert.That(entity.Payments[1], Is.EqualTo(payment1));
            });

            Invoice? entity1 = await InvoiceManager.Get(Guid.Parse("7295F9D6-0989-4C4D-8EE3-88AF81E25F57"));

            Assert.Multiple(() =>
            {
                Assert.That(entity1, Is.Not.Null);
                Assert.That(entity1.Customer, Is.Null);
                Assert.That(entity1.Items, Is.Null);
                Assert.That(entity1.Payments, Is.Null);
            });

            InvoiceDataManager.Setup(x => x.GetAll()).Returns(Task.FromResult(new List<Invoice>() { invoice.Clone(), invoice.Clone() }));

            var entities = await InvoiceManager.GetAllEnriched();

            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Has.Count.EqualTo(2));
            foreach (var e in entities)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Customer, Is.EqualTo(customer));
                    Assert.That(e.Customer.ContactDetail, Is.EqualTo(contactDetail));
                });

                Assert.Multiple(() =>
                {
                    Assert.That(e.Items, Is.Not.Null);
                    Assert.That(e.Items, Has.Count.EqualTo(1));
                    Assert.That(e.Items[0], Is.EqualTo(invoiceItem));
                    Assert.That(e.Items[0].Product, Is.EqualTo(product));
                });

                Assert.Multiple(() =>
                {
                    Assert.That(e.Payments, Is.Not.Null);
                    Assert.That(e.Payments, Has.Count.EqualTo(2));
                    Assert.That(e.Payments[0], Is.EqualTo(payment));
                    Assert.That(e.Payments[1], Is.EqualTo(payment1));
                });
            }
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
