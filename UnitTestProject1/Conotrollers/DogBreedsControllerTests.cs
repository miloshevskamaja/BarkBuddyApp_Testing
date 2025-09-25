using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using Moq;
using Xunit;

namespace UnitTestProject1.Controllers
{
    public class DogBreedsControllerTests
    {
        private Mock<DbSet<DogBreed>> GetQueryableMockDbSet(List<DogBreed> sourceList)
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<DogBreed>>();
            dbSet.As<IQueryable<DogBreed>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<DogBreed>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<DogBreed>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<DogBreed>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // важно за Find()
            dbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids =>
            {
                var id = (int)ids[0];
                return sourceList.FirstOrDefault(d => d.Id == id);
            });

            dbSet.Setup(m => m.Add(It.IsAny<DogBreed>())).Callback<DogBreed>(sourceList.Add);
            dbSet.Setup(m => m.Remove(It.IsAny<DogBreed>())).Callback<DogBreed>(d => sourceList.Remove(d));

            return dbSet;
        }

        private DogBreedsController GetController(List<DogBreed> data)
        {
            var mockSet = GetQueryableMockDbSet(data);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.DogBreeds).Returns(mockSet.Object);
            var controller = new DogBreedsController(mockContext.Object);
            controller.ControllerContext = new ControllerContext();

            return controller;
        }

        [Fact]
        public void Index_ReturnsAllBreeds()
        {
            var data = new List<DogBreed> { new DogBreed { Id = 1, Name = "Beagle" } };
            var controller = GetController(data);

            var result = controller.Index() as ViewResult;

            var model = Assert.IsAssignableFrom<IEnumerable<DogBreed>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Details_ValidId_ReturnsBreed()
        {
            var data = new List<DogBreed> { new DogBreed { Id = 1, Name = "Husky" } };
            var controller = GetController(data);

            var result = controller.Details(1) as ViewResult;

            var model = Assert.IsType<DogBreed>(result.Model);
            Assert.Equal("Husky", model.Name);
        }

        [Fact]
        public void Details_NullId_ReturnsBadRequest()
        {
            var controller = GetController(new List<DogBreed>());

            var result = controller.Details(null);

            Assert.IsType<HttpStatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Fact]
        public void Details_NotFound_ReturnsHttpNotFound()
        {
            var controller = GetController(new List<DogBreed>());

            var result = controller.Details(99);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void Create_Post_ValidModel_AddsBreed()
        {
            var data = new List<DogBreed>();
            var controller = GetController(data);

            var breed = new DogBreed { Id = 1, Name = "Collie" };

            var result = controller.Create(breed) as RedirectToRouteResult;

            Assert.Single(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_ValidId_ReturnsBreed()
        {
            var data = new List<DogBreed> { new DogBreed { Id = 1, Name = "Bulldog" } };
            var controller = GetController(data);

            var result = controller.Edit(1) as ViewResult;

            var model = Assert.IsType<DogBreed>(result.Model);
            Assert.Equal("Bulldog", model.Name);
        }

        [Fact]
        public void Edit_Post_ValidModel_UpdatesBreed()
        {
            var data = new List<DogBreed> { new DogBreed { Id = 1, Name = "OldName" } };
            var controller = GetController(data);

            var updated = new DogBreed { Id = 1, Name = "NewName" };

            var result = controller.Edit(updated) as RedirectToRouteResult;

            Assert.Equal("NewName", data.First().Name);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void DeleteConfirmed_RemovesBreed()
        {
            var data = new List<DogBreed> { new DogBreed { Id = 1, Name = "Retriever" } };
            var controller = GetController(data);

            var result = controller.DeleteConfirmed(1) as RedirectToRouteResult;

            Assert.Empty(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }
    }
}
