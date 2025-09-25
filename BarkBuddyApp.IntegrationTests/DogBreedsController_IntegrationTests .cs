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

public class DogBreedsController_IntegrationTests : IDisposable
{
    private readonly AppDb _db;
    private readonly DogBreedsController _controller;

    public DogBreedsController_IntegrationTests()
    {
        
        DbConnection conn = DbConnectionFactory.CreateTransient();
        _db = new AppDb(conn, false);

   
        _db.Configuration.ValidateOnSaveEnabled = false;

        _db.DogBreeds.Add(new DogBreed { Id = 1, Name = "Husky", ImageURL = "husky.jpg", Description = "Snow" });
        _db.DogBreeds.Add(new DogBreed { Id = 2, Name = "Beagle", ImageURL = "beagle.jpg", Description = "Hunter" });
        _db.SaveChanges();

        _controller = new DogBreedsController(_db);
    }

    public void Dispose()
    {
        _controller?.Dispose();
        _db?.Dispose();
    }


    [Fact]
    public void Index_Returns_All_DogBreeds()
    {
        var res = _controller.Index() as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsAssignableFrom<List<DogBreed>>(res.Model);
        Assert.Equal(2, model.Count);
        Assert.Contains(model, d => d.Name == "Husky");
        Assert.Contains(model, d => d.Name == "Beagle");
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
        var invalidId = (_db.DogBreeds.Max(d => (int?)d.Id) ?? 0) + 1000;
        var res = _controller.Details(invalidId);
        Assert.IsType<HttpNotFoundResult>(res);
    }

    [Fact]
    public void Details_ValidId_Returns_View_With_Model()
    {
        var id = _db.DogBreeds.Single(d => d.Name == "Husky").Id;

        var res = _controller.Details(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<DogBreed>(res.Model);
        Assert.Equal("Husky", model.Name);
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
        var nextId = (_db.DogBreeds.Max(d => (int?)d.Id) ?? 0) + 1; 
        var toCreate = new DogBreed
        {
            Id = nextId,
            Name = "Labrador",
            ImageURL = "lab.jpg",
            Description = "Friendly"
        };

        var res = _controller.Create(toCreate) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);
        Assert.True(_db.DogBreeds.Any(d => d.Name == "Labrador"));
    }

    [Fact]
    public void Create_Post_Invalid_Model_Returns_View()
    {
        var nextId = (_db.DogBreeds.Max(d => (int?)d.Id) ?? 0) + 1;
        var invalid = new DogBreed
        {
            Id = nextId,
            Name = "AB", 
            ImageURL = "ab.jpg"
        };
        _controller.ModelState.AddModelError("Name", "min 3");

        var res = _controller.Create(invalid) as ViewResult;
        Assert.NotNull(res);
        Assert.Same(invalid, res.Model);
        Assert.False(_db.DogBreeds.Any(d => d.Id == nextId && d.Name == "AB"));
    }

    [Fact]
    public void Edit_Get_Valid_Returns_View_With_Model()
    {
        var id = _db.DogBreeds.Single(d => d.Name == "Beagle").Id;

        var res = _controller.Edit(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<DogBreed>(res.Model);
        Assert.Equal("Beagle", model.Name);
    }

    [Fact]
    public void Edit_Post_Valid_Updates_And_Redirects()
    {
        var id = _db.DogBreeds.Single(d => d.Name == "Husky").Id;

        var updated = new DogBreed
        {
            Id = id,
            Name = "Husky Updated",
            ImageURL = "husky2.jpg",
            Description = "Snow king"
        };

        var res = _controller.Edit(updated) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        var reloaded = _db.DogBreeds.Find(id);
        Assert.Equal("Husky Updated", reloaded.Name);
        Assert.Equal("husky2.jpg", reloaded.ImageURL);
        Assert.Equal("Snow king", reloaded.Description);
    }

    [Fact]
    public void Edit_Post_Invalid_Model_Returns_View()
    {
        var id = _db.DogBreeds.Single(d => d.Name == "Beagle").Id;

        var invalid = new DogBreed
        {
            Id = id,
            Name = "AB", 
            ImageURL = "b2.jpg",
            Description = "X"
        };
        _controller.ModelState.AddModelError("Name", "min 3");

        var res = _controller.Edit(invalid) as ViewResult;
        Assert.NotNull(res);
        Assert.Same(invalid, res.Model);

        var still = _db.DogBreeds.Find(id);
        Assert.Equal("Beagle", still.Name); 
    }

 
    [Fact]
    public void Delete_Get_Valid_Returns_View()
    {
        var id = _db.DogBreeds.Single(d => d.Name == "Beagle").Id;

        var res = _controller.Delete(id) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<DogBreed>(res.Model);
        Assert.Equal("Beagle", model.Name);
    }

    [Fact]
    public void DeleteConfirmed_Removes_And_Redirects()
    {
        var idToDelete = _db.DogBreeds.Single(d => d.Name == "Beagle").Id;

        var res = _controller.DeleteConfirmed(idToDelete) as RedirectToRouteResult;
        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);

        Assert.False(_db.DogBreeds.Any(d => d.Id == idToDelete));

    }
}
