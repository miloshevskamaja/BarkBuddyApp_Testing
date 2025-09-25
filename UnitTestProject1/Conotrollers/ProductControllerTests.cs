using Moq;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xunit;
using System;

public class ProductsControllerTests
{
    private ApplicationDbContext GetDbContextMock(List<Product> products, List<Producer> producers)
    {
        // Mock DbSet за Products
        var mockSetProducts = new Mock<DbSet<Product>>();
        var queryableProducts = products.AsQueryable();
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(queryableProducts.Provider);
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(queryableProducts.Expression);
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(queryableProducts.ElementType);
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(queryableProducts.GetEnumerator());

        // Mock DbSet за Producers
        var mockSetProducers = new Mock<DbSet<Producer>>();
        var queryableProducers = producers.AsQueryable();
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.Provider).Returns(queryableProducers.Provider);
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.Expression).Returns(queryableProducers.Expression);
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.ElementType).Returns(queryableProducers.ElementType);
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.GetEnumerator()).Returns(queryableProducers.GetEnumerator());

        // Mock ApplicationDbContext
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(c => c.Products).Returns(mockSetProducts.Object);
        mockContext.Setup(c => c.Producers).Returns(mockSetProducers.Object);

        return mockContext.Object;
    }



    private ProductsController SetupControllerWithSession(ApplicationDbContext db, List<Product> products = null, List<Toys> toys = null)
    {
        var controller = new ProductsController(db);

        // Mock session
        var mockSession = new Mock<HttpSessionStateBase>();
        var sessionItems = new Dictionary<string, object>();
        mockSession.Setup(s => s["ShoppingCart"]).Returns((string key) =>
            sessionItems.ContainsKey(key) ? sessionItems[key] : null
        );
        mockSession.SetupSet(s => s["ShoppingCart"] = It.IsAny<object>())
                   .Callback<string, object>((key, value) => sessionItems[key] = value);

        // Mock HttpContext
        var mockContext = new Mock<HttpContextBase>();
        mockContext.Setup(c => c.Session).Returns(mockSession.Object);

        // ControllerContext
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockContext.Object
        };

        // Mock Find for DbSet<Product>
        if (products != null)
        {
            var mockSetProducts = Mock.Get(db.Products);
            mockSetProducts.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => products.FirstOrDefault(p => p.Id == (int)ids[0]));
        }

        // Mock Find for DbSet<Toys>
        if (toys != null)
        {
            var mockSetToys = Mock.Get(db.Toys);
            mockSetToys.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => toys.FirstOrDefault(t => t.Id == (int)ids[0]));
        }

        return controller;
    }


    [Fact]
    public void Index_ShouldReturnAllProducts_WhenNoSearchString()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Dog Food", ProducerId = 1, Price = 10, Price2 = 90 },
            new Product { Id = 2, Name = "Dog Toy", ProducerId = 1, Price = 5, Price2 = 45 }
        };
        var producers = new List<Producer> { new Producer { Id = 1, Name = "ACME" } };

        var mockDb = GetDbContextMock(products, producers);
        var controller = new ProductsController(mockDb);

        var result = controller.Index(null) as ViewResult;
        var model = Assert.IsAssignableFrom<List<Product>>(result.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public void Index_ShouldReturnFilteredProducts_WhenSearchStringProvided()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Dog Food", ProducerId = 1, Price = 10, Price2 = 90 },
            new Product { Id = 2, Name = "Cat Food", ProducerId = 1, Price = 8, Price2 = 80 }
        };
        var producers = new List<Producer> { new Producer { Id = 1, Name = "ACME" } };

        var mockDb = GetDbContextMock(products, producers);
        var controller = new ProductsController(mockDb);

        var result = controller.Index("Dog") as ViewResult;
        var model = Assert.IsAssignableFrom<List<Product>>(result.Model);
        Assert.Single(model);
        Assert.Equal("Dog Food", model[0].Name);
    }

    [Fact]
    public void Details_ShouldReturnProduct_WhenIdIsValid()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Dog Food", ProducerId = 1, Price = 10, Price2 = 90, Producer = new Producer { Id = 1, Name = "ACME" } }
        };
        var producers = new List<Producer> { new Producer { Id = 1, Name = "ACME" } };

        var mockDb = GetDbContextMock(products, producers);
        var controller = new ProductsController(mockDb);

        var result = controller.Details(1) as ViewResult;
        var model = Assert.IsType<Product>(result.Model);
        Assert.Equal("Dog Food", model.Name);
    }

    [Fact]
    public void AddToCart_ShouldAddProductToCart()
    {
        var products = new List<Product>
    {
        new Product { Id = 1, Name = "Dog Food", Price = 10, Price2 = 90 }
    };
        var mockDb = GetDbContextMock(products, new List<Producer>());

        var controller = SetupControllerWithSession(mockDb, products);

        var result = controller.AddToCart(1, 2) as RedirectToRouteResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.RouteValues["action"]);

        var cart = controller.Session["ShoppingCart"] as ShoppingCart;
        Assert.NotNull(cart);
        Assert.Single(cart.BuyingProducts);
        Assert.Equal(2, cart.Quantities[0]);
        Assert.Equal(10, cart.Prices[0]);
    }

    [Fact]
    public void AddToCart2_ShouldAddProductToCartWithPrice2()
    {
        var products = new List<Product>
    {
        new Product { Id = 1, Name = "Dog Food", Price = 10, Price2 = 90 }
    };
        var mockDb = GetDbContextMock(products, new List<Producer>());

        var controller = SetupControllerWithSession(mockDb, products);

        var result = controller.AddToCart2(1, 3) as RedirectToRouteResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.RouteValues["action"]);

        var cart = controller.Session["ShoppingCart"] as ShoppingCart;
        Assert.NotNull(cart);
        Assert.Single(cart.BuyingProducts);
        Assert.Equal(3, cart.Quantities[0]);
        Assert.Equal(90, cart.Prices[0]); // Price2
    }

    [Fact]
    public void AddToCart3_ShouldAddToyToCart()
    {
        var toys = new List<Toys>
    {
        new Toys { Id = 1, Name = "Squeaky Toy", Price = 15 }
    };
        var mockDb = new Mock<ApplicationDbContext>();
        var mockSetToys = new Mock<DbSet<Toys>>();
        var queryableToys = toys.AsQueryable();

        mockSetToys.As<IQueryable<Toys>>().Setup(m => m.Provider).Returns(queryableToys.Provider);
        mockSetToys.As<IQueryable<Toys>>().Setup(m => m.Expression).Returns(queryableToys.Expression);
        mockSetToys.As<IQueryable<Toys>>().Setup(m => m.ElementType).Returns(queryableToys.ElementType);
        mockSetToys.As<IQueryable<Toys>>().Setup(m => m.GetEnumerator()).Returns(queryableToys.GetEnumerator());

        mockDb.Setup(d => d.Toys).Returns(mockSetToys.Object);

        var controller = SetupControllerWithSession(mockDb.Object, toys: toys);

        var result = controller.AddToCart3(1, 4) as RedirectToRouteResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.RouteValues["action"]);

        var cart = controller.Session["ShoppingCart"] as ShoppingCart;
        Assert.NotNull(cart);
        Assert.Single(cart.BuyingProducts);
        Assert.Equal(4, cart.Quantities[0]);
        Assert.Equal(15, cart.Prices[0]);
    }

    [Fact]
    public void Create_ShouldAddProduct_WhenModelIsValid()
    {
        var products = new List<Product>();
        var producers = new List<Producer> { new Producer { Id = 1, Name = "ACME" } };

        var mockDb = GetDbContextMock(products, producers);
        var controller = new ProductsController(mockDb);

        var newProduct = new Product { Id = 1, Name = "Dog Food", ProducerId = 1, Price = 10, Price2 = 90 };

        var result = controller.Create(newProduct, null) as RedirectToRouteResult;
        Assert.Equal("Index", result.RouteValues["action"]);
    }

    private ApplicationDbContext GetDbContextMock(
        List<Product> products,
        List<Producer> producers,
        List<DogBreed> dogBreeds,
        List<OrderViewModel> orders,
        List<Grooming> groomings)
    {
        // ------------------- Products -------------------
        var mockSetProducts = new Mock<DbSet<Product>>();
        var queryableProducts = products.AsQueryable();

        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(queryableProducts.Provider);
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(queryableProducts.Expression);
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(queryableProducts.ElementType);
        mockSetProducts.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(queryableProducts.GetEnumerator());

        // Populate DogBreeds for each product
        foreach (var p in products)
        {
            p.DogBreeds = new List<DogBreed>();
            foreach (var b in dogBreeds)
            {
                p.DogBreeds.Add(b);
            }
        }

        // ------------------- Producers -------------------
        var mockSetProducers = new Mock<DbSet<Producer>>();
        var queryableProducers = producers.AsQueryable();
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.Provider).Returns(queryableProducers.Provider);
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.Expression).Returns(queryableProducers.Expression);
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.ElementType).Returns(queryableProducers.ElementType);
        mockSetProducers.As<IQueryable<Producer>>().Setup(m => m.GetEnumerator()).Returns(queryableProducers.GetEnumerator());

        // ------------------- DogBreeds -------------------
        var mockSetDogBreeds = new Mock<DbSet<DogBreed>>();
        var queryableDogBreeds = dogBreeds.AsQueryable();
        mockSetDogBreeds.As<IQueryable<DogBreed>>().Setup(m => m.Provider).Returns(queryableDogBreeds.Provider);
        mockSetDogBreeds.As<IQueryable<DogBreed>>().Setup(m => m.Expression).Returns(queryableDogBreeds.Expression);
        mockSetDogBreeds.As<IQueryable<DogBreed>>().Setup(m => m.ElementType).Returns(queryableDogBreeds.ElementType);
        mockSetDogBreeds.As<IQueryable<DogBreed>>().Setup(m => m.GetEnumerator()).Returns(queryableDogBreeds.GetEnumerator());

        // ------------------- Orders -------------------
        var mockSetOrders = new Mock<DbSet<OrderViewModel>>();
        var queryableOrders = orders.AsQueryable();
        mockSetOrders.As<IQueryable<OrderViewModel>>().Setup(m => m.Provider).Returns(queryableOrders.Provider);
        mockSetOrders.As<IQueryable<OrderViewModel>>().Setup(m => m.Expression).Returns(queryableOrders.Expression);
        mockSetOrders.As<IQueryable<OrderViewModel>>().Setup(m => m.ElementType).Returns(queryableOrders.ElementType);
        mockSetOrders.As<IQueryable<OrderViewModel>>().Setup(m => m.GetEnumerator()).Returns(queryableOrders.GetEnumerator());
        mockSetOrders.Setup(m => m.Find(It.IsAny<int>())).Returns((object id) => orders.FirstOrDefault(o => o.Id == (int)id));

        // ------------------- Groomings -------------------
        var mockSetGroomings = new Mock<DbSet<Grooming>>();
        var queryableGroomings = groomings.AsQueryable();
        mockSetGroomings.As<IQueryable<Grooming>>().Setup(m => m.Provider).Returns(queryableGroomings.Provider);
        mockSetGroomings.As<IQueryable<Grooming>>().Setup(m => m.Expression).Returns(queryableGroomings.Expression);
        mockSetGroomings.As<IQueryable<Grooming>>().Setup(m => m.ElementType).Returns(queryableGroomings.ElementType);
        mockSetGroomings.As<IQueryable<Grooming>>().Setup(m => m.GetEnumerator()).Returns(queryableGroomings.GetEnumerator());

        // ------------------- Mock DbContext -------------------
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(c => c.Products).Returns(mockSetProducts.Object);
        mockContext.Setup(c => c.Producers).Returns(mockSetProducers.Object);
        mockContext.Setup(c => c.DogBreeds).Returns(mockSetDogBreeds.Object);
        mockContext.Setup(c => c.Orders).Returns(mockSetOrders.Object);
        mockContext.Setup(c => c.Groomings).Returns(mockSetGroomings.Object);

        return mockContext.Object;
    }

    // ------------------- Controller Setup with Session -------------------
    private ProductsController SetupControllerWithSession(ApplicationDbContext db)
    {
        var controller = new ProductsController(db);
        var httpContext = new Mock<HttpContextBase>();
        var session = new Mock<HttpSessionStateBase>();
        session.Setup(s => s["ShoppingCart"]).Returns(new ShoppingCart());
        httpContext.Setup(ctx => ctx.Session).Returns(session.Object);
        controller.ControllerContext = new ControllerContext(httpContext.Object, new System.Web.Routing.RouteData(), controller);
        return controller;
    }







    [Fact]
    public void Edit_Get_ShouldReturnProductView()
    {
        var products = new List<Product> { new Product { Id = 1, Name = "Dog Food" } };
        var producers = new List<Producer> { new Producer { Id = 1, Name = "ACME" } };
        var dogBreeds = new List<DogBreed> { new DogBreed { Id = 1, Name = "Bulldog" } };
        var orders = new List<OrderViewModel>();
        var groomings = new List<Grooming>();

        var mockDb = GetDbContextMock(products, producers, dogBreeds, orders, groomings);

        var controller = SetupControllerWithSession(mockDb);
        var result = controller.Edit(1) as ViewResult;

        var model = Assert.IsType<Product>(result.Model);
        Assert.Equal("Dog Food", model.Name);
    }

    [Fact]
    public void Delete_Get_ShouldReturnProductView()
    {
        var products = new List<Product> { new Product { Id = 1, Name = "Dog Food" } };
        var producers = new List<Producer>();
        var dogBreeds = new List<DogBreed>();
        var orders = new List<OrderViewModel>();
        var groomings = new List<Grooming>();

        var mockDb = GetDbContextMock(products, producers, dogBreeds, orders, groomings);

        var controller = SetupControllerWithSession(mockDb);
        var result = controller.Delete(1) as ViewResult;

        var model = Assert.IsType<Product>(result.Model);
        Assert.Equal("Dog Food", model.Name);
    }

    [Fact]
    public void Orders_ShouldReturnOrdersList()
    {
        var products = new List<Product>();
        var producers = new List<Producer>();
        var dogBreeds = new List<DogBreed>();
        var orders = new List<OrderViewModel> { new OrderViewModel { Id = 1, Buyer = new Buyer { Name = "John" } } };
        var groomings = new List<Grooming>();

        var mockDb = GetDbContextMock(products, producers, dogBreeds, orders, groomings);

        var controller = SetupControllerWithSession(mockDb);
        var result = controller.Orders() as ViewResult;

        var model = Assert.IsAssignableFrom<List<OrderViewModel>>(result.Model);
        Assert.Single(model);
        Assert.Equal("John", model[0].Buyer.Name);
    }

    [Fact]
    public void ShoppingCart_ShouldReturnCart()
    {
        var products = new List<Product>();
        var producers = new List<Producer>();
        var dogBreeds = new List<DogBreed>();
        var orders = new List<OrderViewModel>();
        var groomings = new List<Grooming>();

        var mockDb = GetDbContextMock(products, producers, dogBreeds, orders, groomings);
        var controller = SetupControllerWithSession(mockDb);

        var result = controller.ShoppingCart() as ViewResult;
        var model = Assert.IsType<ShoppingCart>(result.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public void Groomings_ShouldReturnGroomingsList()
    {
        var products = new List<Product>();
        var producers = new List<Producer>();
        var dogBreeds = new List<DogBreed>();
        var orders = new List<OrderViewModel>();
        var groomings = new List<Grooming> { new Grooming { Id = 1 } };

        var mockDb = GetDbContextMock(products, producers, dogBreeds, orders, groomings);
        var controller = SetupControllerWithSession(mockDb);
        var result = controller.Groomings() as ViewResult;

        var model = Assert.IsAssignableFrom<List<Grooming>>(result.Model);
        Assert.Single(model);
    }

    [Fact]
    public void Succ_ShouldReturnView()
    {
        var products = new List<Product>();
        var producers = new List<Producer>();
        var dogBreeds = new List<DogBreed>();
        var orders = new List<OrderViewModel>();
        var groomings = new List<Grooming>();

        var mockDb = GetDbContextMock(products, producers, dogBreeds, orders, groomings);
        var controller = SetupControllerWithSession(mockDb);

        var result = controller.Succ() as ViewResult;
        Assert.NotNull(result);
    }



}
