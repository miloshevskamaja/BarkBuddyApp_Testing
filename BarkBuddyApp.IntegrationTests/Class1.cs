using System.Linq;
using System.Net;
using System.Web.Mvc;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using Effort; // работи само во net48 со Effort.EF6
using FluentAssertions;
using Xunit;

namespace BarkBuddyApp.Tests.Integration
{
    public class TestDbContext : ApplicationDbContext
    {
        public TestDbContext(System.Data.Common.DbConnection conn) : base(conn, true) { }
    }

    public class DogBreedsControllerTests : System.IDisposable
    {
        private readonly DogBreedsController _controller;
        private readonly TestDbContext _context;

        public DogBreedsControllerTests()
        {
            var conn = DbConnectionFactory.CreateTransient();
            _context = new TestDbContext(conn);

            _context.DogBreeds.Add(new DogBreed { Id = 1, Name = "Husky", Description = "Snow dog", ImageURL = "husky.jpg" });
            _context.DogBreeds.Add(new DogBreed { Id = 2, Name = "Beagle", Description = "Hunter dog", ImageURL = "beagle.jpg" });
            _context.SaveChanges();

            _controller = new DogBreedsController(_context);
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _context?.Dispose();
        }

        [Fact]
        public void Index_ReturnsViewWithAllDogBreeds()
        {
            var result = _controller.Index() as ViewResult;
            result.Should().NotBeNull();

            var model = result.Model as System.Collections.Generic.IEnumerable<DogBreed>;
            model.Should().NotBeNull();
            model.Count().Should().Be(2);
            model.Any(d => d.Name == "Husky").Should().BeTrue();
        }

        [Fact]
        public void Details_NullId_ReturnsBadRequest()
        {
            var result = _controller.Details(null) as HttpStatusCodeResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Details_WithValidId_ReturnsCorrectDogBreed()
        {
            var result = _controller.Details(1) as ViewResult;
            result.Should().NotBeNull();
            var dog = result.Model as DogBreed;
            dog.Should().NotBeNull();
            dog.Name.Should().Be("Husky");
        }

        [Fact]
        public void Details_WithInvalidId_ReturnsNotFound()
        {
            var result = _controller.Details(99);
            result.Should().BeOfType<HttpNotFoundResult>();
        }

        [Fact]
        public void Create_PostValidDogBreed_AddsToDbAndRedirects()
        {
            var newDog = new DogBreed { Id = 3, Name = "Labrador", Description = "Friendly dog", ImageURL = "lab.jpg" };

            var result = _controller.Create(newDog) as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            _context.DogBreeds.Count().Should().Be(3);
        }

        [Fact]
        public void Edit_PostValidDogBreed_UpdatesAndRedirects()
        {
            var updatedDog = new DogBreed { Id = 1, Name = "Husky Updated", Description = "Snow dog", ImageURL = "husky.jpg" };

            var result = _controller.Edit(updatedDog) as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            _context.DogBreeds.Find(1).Name.Should().Be("Husky Updated");
        }

        [Fact]
        public void DeleteConfirmed_RemovesDogBreedAndRedirects()
        {
            var result = _controller.DeleteConfirmed(1) as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            _context.DogBreeds.Count().Should().Be(1);
        }
    }
}
