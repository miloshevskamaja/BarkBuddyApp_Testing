using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Effort;
using Moq;
using Xunit;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using AppDb = BarkBuddyApp.Models.ApplicationDbContext;

public class ProductsController_IntegrationTests_Short : IDisposable
{
    private readonly AppDb _db;
    private readonly ProductsController _controller;

    public ProductsController_IntegrationTests_Short()
    {
       
        DbConnection conn = DbConnectionFactory.CreateTransient();
        _db = new AppDb(conn, false);

       
        _db.Configuration.ValidateOnSaveEnabled = false;

        var prod1 = new Producer { Id = 1, Name = "ACME", Logo = "https://ex.com/acme.png", Description = "Top brand" };
        var prod2 = new Producer { Id = 2, Name = "Contoso", Logo = "https://ex.com/contoso.png", Description = "Alt brand" };

        var b1 = new DogBreed { Id = 10, Name = "Husky", Description = "Snow", ImageURL = "husky.jpg" };
        var b2 = new DogBreed { Id = 11, Name = "Beagle", Description = "Hunter", ImageURL = "beagle.jpg" };

        var p1 = new Product { Id = 100, Name = "Dog Food", Description = "Tasty", Price = 10, Price2 = 90, ImageUrl = "dog.jpg", ProducerId = 1, Producer = prod1, DogBreeds = new List<DogBreed> { b1 } };
        var p2 = new Product { Id = 101, Name = "Cat Food", Description = "Yum", Price = 8, Price2 = 80, ImageUrl = "cat.jpg", ProducerId = 2, Producer = prod2, DogBreeds = new List<DogBreed> { b2 } };

        var toy = new Toys { Id = 200, Name = "Squeaky", Price = 15 };

        _db.Producers.Add(prod1);
        _db.Producers.Add(prod2);
        _db.DogBreeds.Add(b1);
        _db.DogBreeds.Add(b2);
        _db.Products.Add(p1);
        _db.Products.Add(p2);
        _db.Toys.Add(toy);
        _db.SaveChanges();


        _controller = BuildControllerWithSession(_db);
    }

    private ProductsController BuildControllerWithSession(AppDb db)
    {
        var controller = new ProductsController(db);

        var backing = new Dictionary<string, object>();
        var session = new Mock<HttpSessionStateBase>();
        session.SetupGet(s => s["ShoppingCart"])
               .Returns(() => backing.ContainsKey("ShoppingCart") ? backing["ShoppingCart"] : null);
        session.SetupSet(s => s["ShoppingCart"] = It.IsAny<object>())
               .Callback<string, object>((k, v) => backing[k] = v);

        var http = new Mock<HttpContextBase>();
        http.SetupGet(h => h.Session).Returns(session.Object);

        controller.ControllerContext = new ControllerContext(http.Object, new System.Web.Routing.RouteData(), controller);
        return controller;
    }

    public void Dispose()
    {
        _controller?.Dispose();
        _db?.Dispose();
    }

 
    [Fact]
    public void Index_NoSearch_ReturnsAll()
    {
        var res = _controller.Index(null) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsAssignableFrom<List<Product>>(res.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public void Index_Search_Filters()
    {
        var res = _controller.Index("Dog") as ViewResult;
        var model = Assert.IsAssignableFrom<List<Product>>(res.Model);
        Assert.Single(model);
        Assert.Equal("Dog Food", model[0].Name);
    }

    [Fact]
    public void DeleteConfirmed_Removes_And_Redirects()
    {
        var idToDelete = _db.Products.Single(p => p.Name == "Cat Food").Id;

        var res = _controller.DeleteConfirmed(idToDelete) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        Assert.False(_db.Products.Any(p => p.Id == idToDelete));
        Assert.Equal(1, _db.Products.Count());
    }


    [Fact]
    public void AddToCart_Uses_Price()
    {
        var dogFoodId = _db.Products.Single(p => p.Name == "Dog Food").Id;

        var res = _controller.AddToCart(dogFoodId, 2) as RedirectToRouteResult;
        Assert.NotNull(res);

        var cart = (ShoppingCart)_controller.Session["ShoppingCart"];
        Assert.NotNull(cart);
        Assert.Single(cart.BuyingProducts);
        Assert.Equal("Dog Food", cart.BuyingProducts[0]);
        Assert.Equal(10, cart.Prices[0]);
        Assert.Equal(2, cart.Quantities[0]);
    }

    [Fact]
    public void AddToCart2_Uses_Price2()
    {
        var dogFoodId = _db.Products.Single(p => p.Name == "Dog Food").Id;

        var res = _controller.AddToCart2(dogFoodId, 3) as RedirectToRouteResult;
        Assert.NotNull(res);

        var cart = (ShoppingCart)_controller.Session["ShoppingCart"];
        Assert.NotNull(cart);
        Assert.Equal(90, cart.Prices[0]);  
        Assert.Equal(3, cart.Quantities[0]);
    }

    [Fact]
    public void AddToCart3_Toy_Adds_And_Aggregates()
    {
        var toyId = _db.Toys.Single(t => t.Name == "Squeaky").Id;

        _controller.AddToCart3(toyId, 1);
        var res = _controller.AddToCart3(toyId, 4) as RedirectToRouteResult;
        Assert.NotNull(res);

        var cart = (ShoppingCart)_controller.Session["ShoppingCart"];
        Assert.NotNull(cart);
        Assert.Single(cart.BuyingProducts);
        Assert.Equal("Squeaky", cart.BuyingProducts[0]);
        Assert.Equal(15, cart.Prices[0]);
        Assert.Equal(5, cart.Quantities[0]); 
    }


    [Fact]
    public void Groomings_Returns_List()
    {
       
        _db.Groomings.Add(new Grooming
        {
            Id = 401,
            ReservationDateTime = new DateTime(2030, 1, 2, 10, 0, 0),
            DogBreed = "Beagle",
            DogAge = 5,
            Details = "Full"
        });
        _db.SaveChanges();

        var res = _controller.Groomings() as ViewResult;
        Assert.NotNull(res);
        var list = Assert.IsAssignableFrom<List<Grooming>>(res.Model);
        Assert.Single(list); 
    }

    [Fact]
    public void ScheduleGrooming_Success_Redirects_To_Succ()
    {
        var ok = new Grooming
        {
            Id = 402,
            ReservationDateTime = new DateTime(2030, 1, 3, 9, 0, 0),
            DogBreed = "Husky",
            DogAge = 2,
            Details = "Light"
        };
        var res = _controller.ScheduleGrooming(ok) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Succ", res.RouteValues["action"]);
        Assert.NotNull(_db.Groomings.FirstOrDefault(g => g.ReservationDateTime == ok.ReservationDateTime));
    }

    [Fact]
    public void ScheduleGrooming_Conflict_AddsModelError()
    {

        var reserved = new Grooming
        {
            Id = 500,
            ReservationDateTime = new DateTime(2030, 1, 4, 10, 0, 0),
            DogBreed = "Husky",
            DogAge = 3
        };
        _db.Groomings.Add(reserved);
        _db.SaveChanges();

      
        var conflict = new Grooming
        {
            Id = 501,
            ReservationDateTime = new DateTime(2030, 1, 4, 10, 0, 0),
            DogBreed = "Husky",
            DogAge = 4
        };
        var res = _controller.ScheduleGrooming(conflict) as ViewResult;
        Assert.NotNull(res);
        Assert.False(_controller.ModelState.IsValid);
        Assert.True(_controller.ModelState[string.Empty].Errors.Any());
        Assert.Same(conflict, res.Model);
    }
}
