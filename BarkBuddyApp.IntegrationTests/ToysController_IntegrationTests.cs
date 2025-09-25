using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Effort;
using Xunit;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using AppDb = BarkBuddyApp.Models.ApplicationDbContext;

public class ToysController_IntegrationTests : IDisposable
{
    private readonly AppDb _db;
    private readonly ToysController _controller;

    public ToysController_IntegrationTests()
    {
   
        DbConnection conn = DbConnectionFactory.CreateTransient();
        _db = new AppDb(conn, false);

      
        _db.Configuration.ValidateOnSaveEnabled = false;


        _db.Toys.Add(new Toys { Name = "Squeaky", ImageUrl = "https://example.com/s.jpg", Description = "Noise", Price = 15 });
        _db.Toys.Add(new Toys { Name = "Chewy", ImageUrl = "https://example.com/c.jpg", Description = "Chew", Price = 7 });
        _db.SaveChanges();

        _controller = new ToysController(_db);
    }

    public void Dispose()
    {
        _controller?.Dispose();
        _db?.Dispose();
    }

    [Fact]
    public void Index_NoSearch_Returns_All()
    {
        var res = _controller.Index(null) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsAssignableFrom<List<Toys>>(res.Model);
        Assert.Equal(2, model.Count);
        Assert.Contains(model, t => t.Name == "Squeaky");
        Assert.Contains(model, t => t.Name == "Chewy");
    }

    [Fact]
    public void Index_Search_Filters_By_Name()
    {
        var res = _controller.Index("Squea") as ViewResult;
        var model = Assert.IsAssignableFrom<List<Toys>>(res.Model);
        Assert.Single(model);
        Assert.Equal("Squeaky", model[0].Name);
    }

 
    [Fact]
    public void Details_NullId_Returns_BadRequest()
    {
        var res = _controller.Details(null) as HttpStatusCodeResult;
        Assert.NotNull(res);
        Assert.Equal((int)HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public void Details_InvalidId_Returns_NotFound()
    {
        var invalidId = (_db.Toys.Max(t => (int?)t.Id) ?? 0) + 1000;
        var res = _controller.Details(invalidId);
        Assert.IsType<HttpNotFoundResult>(res);
    }

    [Fact]
    public void Details_ValidId_Returns_View_With_Model()
    {
        var id = _db.Toys.Single(t => t.Name == "Squeaky").Id;

        var res = _controller.Details(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<Toys>(res.Model);
        Assert.Equal("Squeaky", model.Name);
    }

 
    [Fact]
    public void Create_Get_Returns_View()
    {
        var res = _controller.Create() as ViewResult;
        Assert.NotNull(res);
    }

    [Fact]
    public void Create_Post_Valid_Adds_And_Redirects()
    {
        var toCreate = new Toys { Name = "Rope", ImageUrl = "https://example.com/r.jpg", Description = "Pull", Price = 9 };

        var res = _controller.Create(toCreate) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);
        Assert.True(_db.Toys.Any(t => t.Name == "Rope"));
    }


    [Fact]
    public void Edit_Get_Valid_Returns_View_With_Model()
    {
        var id = _db.Toys.Single(t => t.Name == "Chewy").Id;

        var res = _controller.Edit(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<Toys>(res.Model);
        Assert.Equal("Chewy", model.Name);
    }

    [Fact]
    public void Edit_Post_Valid_Updates_And_Redirects()
    {
        var id = _db.Toys.Single(t => t.Name == "Squeaky").Id;

        var updated = new Toys { Id = id, Name = "Squeaky Pro", ImageUrl = "https://example.com/s2.jpg", Description = "Louder", Price = 18 };

        var res = _controller.Edit(updated) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        var reloaded = _db.Toys.Find(id);
        Assert.Equal("Squeaky Pro", reloaded.Name);
        Assert.Equal("https://example.com/s2.jpg", reloaded.ImageUrl);
        Assert.Equal(18, reloaded.Price);
        Assert.Equal("Louder", reloaded.Description);
    }


    [Fact]
    public void Delete_Get_Valid_Returns_View()
    {
        var id = _db.Toys.Single(t => t.Name == "Chewy").Id;

        var res = _controller.Delete(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<Toys>(res.Model);
        Assert.Equal("Chewy", model.Name);
    }

    [Fact]
    public void DeleteConfirmed_Removes_And_Redirects()
    {
        var idToDelete = _db.Toys.Single(t => t.Name == "Chewy").Id;

        var res = _controller.DeleteConfirmed(idToDelete) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        Assert.False(_db.Toys.Any(t => t.Id == idToDelete));
    }
}
