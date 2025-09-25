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
    public class GroomingDogsControllerTests
    {
        private Mock<DbSet<GroomingDog>> GetQueryableMockDbSet(List<GroomingDog> sourceList)
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<GroomingDog>>();
            dbSet.As<IQueryable<GroomingDog>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<GroomingDog>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<GroomingDog>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<GroomingDog>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

    
            dbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids =>
            {
                var id = (int)ids[0];
                return sourceList.FirstOrDefault(d => d.Id == id);
            });

            dbSet.Setup(m => m.Add(It.IsAny<GroomingDog>())).Callback<GroomingDog>(sourceList.Add);
            dbSet.Setup(m => m.Remove(It.IsAny<GroomingDog>())).Callback<GroomingDog>(d => sourceList.Remove(d));

            return dbSet;
        }

        private GroomingDogsController GetController(List<GroomingDog> data)
        {
            var mockSet = GetQueryableMockDbSet(data);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.GroomingDogs).Returns(mockSet.Object);
            var controller = new GroomingDogsController(mockContext.Object);
            controller.ControllerContext = new ControllerContext();

            return controller;
        }

        [Fact]
        public void Index_ReturnsAllDogs()
        {
            var data = new List<GroomingDog> { new GroomingDog { Id = 1, Name = "Cut", PriceForGrooming = 20 } };
            var controller = GetController(data);

            var result = controller.Index() as ViewResult;

            var model = Assert.IsAssignableFrom<IEnumerable<GroomingDog>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Details_ValidId_ReturnsDog()
        {
            var data = new List<GroomingDog> { new GroomingDog { Id = 1, Name = "Wash", PriceForGrooming = 15 } };
            var controller = GetController(data);

            var result = controller.Details(1) as ViewResult;

            var model = Assert.IsType<GroomingDog>(result.Model);
            Assert.Equal("Wash", model.Name);
        }

        [Fact]
        public void Details_NullId_ReturnsBadRequest()
        {
            var controller = GetController(new List<GroomingDog>());

            var result = controller.Details(null);

            Assert.IsType<HttpStatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Fact]
        public void Details_NotFound_ReturnsHttpNotFound()
        {
            var controller = GetController(new List<GroomingDog>());

            var result = controller.Details(99);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void Create_Post_ValidModel_AddsDog()
        {
            var data = new List<GroomingDog>();
            var controller = GetController(data);

            var dog = new GroomingDog { Id = 1, Name = "Trim", PriceForGrooming = 30 };

            var result = controller.Create(dog) as RedirectToRouteResult;

            Assert.Single(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_ValidId_ReturnsDog()
        {
            var data = new List<GroomingDog> { new GroomingDog { Id = 1, Name = "Shave", PriceForGrooming = 25 } };
            var controller = GetController(data);

            var result = controller.Edit(1) as ViewResult;

            var model = Assert.IsType<GroomingDog>(result.Model);
            Assert.Equal("Shave", model.Name);
        }

        [Fact]
        public void Edit_Post_ValidModel_UpdatesDog()
        {
            var data = new List<GroomingDog> { new GroomingDog { Id = 1, Name = "Shave", PriceForGrooming = 25 } };
            var controller = GetController(data);

            var updated = new GroomingDog { Id = 1, Name = "New Style", PriceForGrooming = 40 };

            var result = controller.Edit(updated) as RedirectToRouteResult;

            Assert.Equal("New Style", data.First().Name);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void DeleteConfirmed_RemovesDog()
        {
            var data = new List<GroomingDog> { new GroomingDog { Id = 1, Name = "Bath", PriceForGrooming = 10 } };
            var controller = GetController(data);

            var result = controller.DeleteConfirmed(1) as RedirectToRouteResult;

            Assert.Empty(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }
    }
}
