using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;

namespace UnitTestProject1.Conotrollers
{
    public class ProducersControllerTests
    {
        private Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(s => sourceList.Add(s));
            dbSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>(s => sourceList.Remove(s));

            return dbSet;
        }

        private ProducersController GetController(List<Producer> data)
        {
            var mockSet = GetQueryableMockDbSet(data);
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids =>
            {
                var id = (int)ids[0];
                return data.FirstOrDefault(d => d.Id == id);
            });

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Producers).Returns(mockSet.Object);
       

            var controller = new ProducersController(mockContext.Object);
            controller.ControllerContext = new ControllerContext();

            return controller;
        }

        [Fact]
        public void Index_ReturnsViewWithAllProducers()
        {
            var data = new List<Producer>
            {
                new Producer { Id = 1, Name = "P1", Logo="http://logo1.com" },
                new Producer { Id = 2, Name = "P2", Logo="http://logo2.com" }
            };

            var controller = GetController(data);

            var result = controller.Index() as ViewResult;
            var model = Assert.IsAssignableFrom<List<Producer>>(result.Model);

            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Details_IdIsNull_ReturnsBadRequest()
        {
            var controller = GetController(new List<Producer>());
            var result = controller.Details(null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Details_IdNotFound_ReturnsNotFound()
        {
            var controller = GetController(new List<Producer>());
            var result = controller.Details(5);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void Details_IdValid_ReturnsProducer()
        {
            var data = new List<Producer> { new Producer { Id = 1, Name = "P1", Logo = "http://logo.com" } };
            var controller = GetController(data);

            var result = controller.Details(1) as ViewResult;
            var model = Assert.IsType<Producer>(result.Model);

            Assert.Equal("P1", model.Name);
        }

        [Fact]
        public void Create_Post_ValidModel_AddsProducer()
        {
            var data = new List<Producer>();
            var controller = GetController(data);

            var producer = new Producer { Id = 1, Name = "P1", Logo = "http://logo.com" };

            var result = controller.Create(producer) as RedirectToRouteResult;

            Assert.Single(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_IdIsNull_ReturnsBadRequest()
        {
            var controller = GetController(new List<Producer>());
            var result = controller.Edit((int?)null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Edit_IdNotFound_ReturnsNotFound()
        {
            var controller = GetController(new List<Producer>());
            var result = controller.Edit(99);

            Assert.IsType<HttpNotFoundResult>(result);
        }


        [Fact]
        public void Edit_Post_ValidModel_UpdatesProducer()
        {
            var data = new List<Producer> { new Producer { Id = 1, Name = "Old", Logo = "http://logo.com" } };
            var controller = GetController(data);

            var updated = new Producer { Id = 1, Name = "New", Logo = "http://logo.com" };

            var result = controller.Edit(updated) as RedirectToRouteResult;

            Assert.Equal("New", data.First().Name);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_IdIsNull_ReturnsBadRequest()
        {
            var controller = GetController(new List<Producer>());
            var result = controller.Delete((int?)null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Delete_IdNotFound_ReturnsNotFound()
        {
            var controller = GetController(new List<Producer>());
            var result = controller.Delete(99);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void DeleteConfirmed_RemovesProducer()
        {
            var data = new List<Producer> { new Producer { Id = 1, Name = "P1", Logo = "http://logo.com" } };
            var controller = GetController(data);

            var result = controller.DeleteConfirmed(1) as RedirectToRouteResult;

            Assert.Empty(data);
            Assert.Equal("Index", result.RouteValues["action"]);
        }
    }
}

