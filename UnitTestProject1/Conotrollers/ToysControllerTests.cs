using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Mvc;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using Moq;
using Xunit;

namespace UnitTestProject1.Conotrollers
{
    public class ToysControllerTests
    {
        private Mock<DbSet<Toys>> GetQueryableMockDbSet(List<Toys> sourceList)
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<Toys>>();

            dbSet.As<IQueryable<Toys>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<Toys>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<Toys>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<Toys>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            dbSet.Setup(d => d.Add(It.IsAny<Toys>())).Callback<Toys>(s => sourceList.Add(s));
            dbSet.Setup(d => d.Remove(It.IsAny<Toys>())).Callback<Toys>(s => sourceList.Remove(s));
            dbSet.Setup(d => d.Find(It.IsAny<object[]>())).Returns<object[]>(ids => sourceList.FirstOrDefault(d => d.Id == (int)ids[0]));

            return dbSet;
        }

        private ToysController GetController(List<Toys> data)
        {
            var mockSet = GetQueryableMockDbSet(data);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Toys).Returns(mockSet.Object);

      
            var controller = new ToysController(mockContext.Object);
            controller.ControllerContext = new ControllerContext();

            return controller;
        }




        [Fact]
        public void Index_NoSearch_ReturnsAllToys()
        {
            var data = new List<Toys> {
                new Toys { Id = 1, Name = "Ball" },
                new Toys { Id = 2, Name = "Bone" }
            };
            var controller = GetController(data);

            var result = controller.Index(null) as ViewResult;

            var model = Assert.IsAssignableFrom<IEnumerable<Toys>>(result.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Index_WithSearch_FiltersResults()
        {
            var data = new List<Toys> {
        new Toys { Id = 1, Name = "Ball" },
        new Toys { Id = 2, Name = "Bone" }
    };
            var controller = GetController(data);

            var result = controller.Index("Ball") as ViewResult;

            var model = Assert.IsAssignableFrom<IEnumerable<Toys>>(result.Model);
            Assert.Single(model);
            Assert.Equal("Ball", model.First().Name);
        }

        [Fact]
        public void Details_ValidId_ReturnsToy()
        {
            var data = new List<Toys> { new Toys { Id = 1, Name = "Ball" } };
            var controller = GetController(data);

            var result = controller.Details(1) as ViewResult;

            var model = Assert.IsType<Toys>(result.Model);
            Assert.Equal("Ball", model.Name);
        }

        [Fact]
        public void Details_InvalidId_ReturnsHttpNotFound()
        {
            var data = new List<Toys>();
            var controller = GetController(data);

            var result = controller.Details(5);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void Create_Post_ValidModel_AddsToy()
        {
            var data = new List<Toys>();
            var controller = GetController(data);

            var toy = new Toys { Id = 1, Name = "Ball", Price = 5.0 };

            var result = controller.Create(toy) as RedirectToRouteResult;

            Assert.Single(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_ValidId_ReturnsToy()
        {
            var data = new List<Toys> { new Toys { Id = 1, Name = "Ball" } };
            var controller = GetController(data);

            var result = controller.Edit(1) as ViewResult;

            var model = Assert.IsType<Toys>(result.Model);
            Assert.Equal("Ball", model.Name);
        }

        [Fact]
        public void Edit_Post_ValidModel_UpdatesToy()
        {
            var data = new List<Toys> { new Toys { Id = 1, Name = "Ball" } };
            var controller = GetController(data);

            var updatedToy = new Toys { Id = 1, Name = "Bone" };

            var result = controller.Edit(updatedToy) as RedirectToRouteResult;

            Assert.Equal("Bone", data.First().Name);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_ValidId_ReturnsToy()
        {
            var data = new List<Toys> { new Toys { Id = 1, Name = "Ball" } };
            var controller = GetController(data);

            var result = controller.Delete(1) as ViewResult;

            var model = Assert.IsType<Toys>(result.Model);
            Assert.Equal("Ball", model.Name);
        }

        [Fact]
        public void DeleteConfirmed_RemovesToy()
        {
            var data = new List<Toys> { new Toys { Id = 1, Name = "Ball" } };
            var controller = GetController(data);

            var result = controller.DeleteConfirmed(1) as RedirectToRouteResult;

            Assert.Empty(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }
    }
}
