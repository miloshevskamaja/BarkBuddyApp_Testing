using System.Linq;
using System.Net;
using System.Web.Mvc;
using Xunit;
using Effort;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using AppDb = BarkBuddyApp.Models.ApplicationDbContext;
using System.Data.Common; 

public class GroomingDogsControllerTests : System.IDisposable
{
    private readonly AppDb _db;       
    private readonly GroomingDogsController _controller;

    public GroomingDogsControllerTests()
    {
  
        var conn = DbConnectionFactory.CreateTransient();


        _db = new AppDb(conn, false);     


        _db.GroomingDogs.Add(new GroomingDog { Id = 1, Name = "Small Dog", ImageUrl = "s.jpg", PriceForGrooming = 25 });
        _db.GroomingDogs.Add(new GroomingDog { Id = 2, Name = "Big Dog", ImageUrl = "b.jpg", PriceForGrooming = 40 });
        _db.SaveChanges();

        _controller = new GroomingDogsController(_db);
    }

    public void Dispose()
    {
        _controller?.Dispose();
        _db?.Dispose();
    }

    [Fact]
    public void Index_Returns_All_GroomingDogs()
    {
        var result = _controller.Index() as ViewResult;

        Assert.NotNull(result);
        var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<GroomingDog>>(result.Model);
        Assert.Equal(2, model.Count());
        Assert.Contains(model, x => x.Name == "Small Dog");
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
        var res = _controller.Details(999);
        Assert.IsType<HttpNotFoundResult>(res);
    }

    [Fact]
    public void Details_ValidId_Returns_View_With_Model()
    {
        var res = _controller.Details(1) as ViewResult;
        Assert.NotNull(res);
        var model = Assert.IsType<GroomingDog>(res.Model);
        Assert.Equal("Small Dog", model.Name);
    }

    [Fact]
    public void Create_Valid_Adds_And_Redirects()
    {
        var newItem = new GroomingDog { Id = 3, Name = "Medium Dog", ImageUrl = "m.jpg", PriceForGrooming = 30 };

        var res = _controller.Create(newItem) as RedirectToRouteResult;

        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);
        Assert.Equal(3, _db.GroomingDogs.Count());
        Assert.NotNull(_db.GroomingDogs.Find(3));
    }

    [Fact]
    public void Edit_Valid_Updates_And_Redirects()
    {
        var updated = new GroomingDog { Id = 1, Name = "Small Dog UPDATED", ImageUrl = "s.jpg", PriceForGrooming = 27 };

        var res = _controller.Edit(updated) as RedirectToRouteResult;

        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);
        Assert.Equal("Small Dog UPDATED", _db.GroomingDogs.Find(1).Name);
        Assert.Equal(27, _db.GroomingDogs.Find(1).PriceForGrooming);
    }

    [Fact]
    public void DeleteConfirmed_Removes_And_Redirects()
    {
        var res = _controller.DeleteConfirmed(1) as RedirectToRouteResult;

        Assert.NotNull(res);
        Assert.Equal("Index", res.RouteValues["action"]);
        Assert.Equal(1, _db.GroomingDogs.Count()); 
        Assert.Null(_db.GroomingDogs.Find(1));
    }
}
