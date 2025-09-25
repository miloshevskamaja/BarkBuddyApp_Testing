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

public class ProducersController_IntegrationTests : IDisposable
{
    private readonly AppDb _db;
    private readonly ProducersController _controller;

    public ProducersController_IntegrationTests()
    {
        
        DbConnection conn = DbConnectionFactory.CreateTransient();
        _db = new AppDb(conn, false);

  
        var p1 = new Producer { Name = "ACME", Logo = "https://ex.com/acme.png", Description = "Top brand" };
        var p2 = new Producer { Name = "Contoso", Logo = "https://ex.com/contoso.png", Description = "Alt brand" };

        _db.Producers.Add(p1);
        _db.Producers.Add(p2);
        _db.SaveChanges();

        _controller = new ProducersController(_db);
    }

    public void Dispose()
    {
        _controller?.Dispose();
        _db?.Dispose();
    }


    [Fact]
    public void Index_Returns_All_Producers()
    {
        var res = _controller.Index() as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsAssignableFrom<List<Producer>>(res.Model);
        Assert.Equal(2, model.Count);
        Assert.Contains(model, x => x.Name == "ACME");
        Assert.Contains(model, x => x.Name == "Contoso");
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
        var invalidId = (_db.Producers.Max(p => (int?)p.Id) ?? 0) + 1000;
        var res = _controller.Details(invalidId);
        Assert.IsType<HttpNotFoundResult>(res);
    }

    [Fact]
    public void Details_ValidId_Returns_View_With_Model()
    {
        var id = _db.Producers.Single(p => p.Name == "ACME").Id;

        var res = _controller.Details(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<Producer>(res.Model);
        Assert.Equal("ACME", model.Name);
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
        var toCreate = new Producer
        {
            Name = "Northwind",
            Logo = "https://ex.com/northwind.png",
            Description = "New"
        };

        var res = _controller.Create(toCreate) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        Assert.True(_db.Producers.Any(p => p.Name == "Northwind"));
    }

    [Fact]
    public void Create_Post_Invalid_Model_Returns_View()
    {
        var invalid = new Producer
        {
        
            Logo = "https://ex.com/invalid.png"
        };
        _controller.ModelState.AddModelError("Name", "required");

        var res = _controller.Create(invalid) as ViewResult;
        Assert.NotNull(res);
        Assert.Same(invalid, res.Model);
        Assert.False(_db.Producers.Any(p => p.Logo == "https://ex.com/invalid.png" && p.Name == null));
    }


    [Fact]
    public void Edit_Get_Valid_Returns_View_With_Model()
    {
        var id = _db.Producers.Single(p => p.Name == "Contoso").Id;

        var res = _controller.Edit(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<Producer>(res.Model);
        Assert.Equal("Contoso", model.Name);
    }

    [Fact]
    public void Edit_Post_Valid_Updates_And_Redirects()
    {
        var id = _db.Producers.Single(p => p.Name == "ACME").Id;

        var updated = new Producer
        {
            Id = id,
            Name = "ACME Updated",
            Logo = "https://ex.com/acme2.png",
            Description = "Updated"
        };

        var res = _controller.Edit(updated) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        var reloaded = _db.Producers.Find(id);
        Assert.Equal("ACME Updated", reloaded.Name);
        Assert.Equal("https://ex.com/acme2.png", reloaded.Logo);
        Assert.Equal("Updated", reloaded.Description);
    }

    [Fact]
    public void Edit_Post_Invalid_Model_Returns_View()
    {
        var id = _db.Producers.Single(p => p.Name == "Contoso").Id;

        var invalid = new Producer
        {
            Id = id,
            Name = "", 
            Logo = "https://ex.com/contoso2.png",
            Description = "X"
        };
        _controller.ModelState.AddModelError("Name", "required");

        var res = _controller.Edit(invalid) as ViewResult;
        Assert.NotNull(res);
        Assert.Same(invalid, res.Model);

        var still = _db.Producers.Find(id);
        Assert.Equal("Contoso", still.Name); 
    }

  
    [Fact]
    public void Delete_Get_Valid_Returns_View()
    {
        var id = _db.Producers.Single(p => p.Name == "Contoso").Id;

        var res = _controller.Delete(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<Producer>(res.Model);
        Assert.Equal("Contoso", model.Name);
    }

    [Fact]
    public void DeleteConfirmed_Removes_And_Redirects()
    {
        var idToDelete = _db.Producers.Single(p => p.Name == "Contoso").Id;

        var res = _controller.DeleteConfirmed(idToDelete) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        Assert.False(_db.Producers.Any(p => p.Id == idToDelete));
        Assert.Equal(1, _db.Producers.Count()); 
    }
}
