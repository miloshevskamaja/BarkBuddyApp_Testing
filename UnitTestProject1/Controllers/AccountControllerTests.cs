using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;
using Moq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using BarkBuddyApp.Controllers;
using BarkBuddyApp.Models;
using BarkBuddyApp;
using Microsoft.Owin.Security;
using System.Web;
using UnitTestProject1.Conotrollers;


namespace UnitTestProject1.Controllers
{
    public class AccountControllerTests
    {

        [Fact]
        public void Register_Get_ReturnsView()
        {
            var controller = new AccountController();

            var result = controller.Register() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ForgotPassword_Get_ReturnsView()
        {
            var controller = new AccountController();

            var result = controller.ForgotPassword() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ResetPassword_Get_ReturnsView_WhenCodeExists()
        {
            var controller = new AccountController();

            var result = controller.ResetPassword("some-code") as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ExternalLoginFailure_ReturnsView()
        {
            var controller = new AccountController();

            var result = controller.ExternalLoginFailure() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void AddToRoleModel_Get_ReturnsRoles()
        {
            var controller = new AccountController();

            var result = controller.AddToRoleModel() as ViewResult;
            var model = result.Model as AddToRoleModel;

            Assert.Contains("Admin", model.Roles);
            Assert.Contains("User", model.Roles);
        }

        [Fact]
        public async Task Login_Post_ValidModel_SuccessRedirectsToLocal()
        {
      
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<ApplicationUserManager>(userStoreMock.Object);
            var authManagerMock = new Mock<IAuthenticationManager>();
            var signInManagerMock = new Mock<ApplicationSignInManager>(userManagerMock.Object, authManagerMock.Object);

            signInManagerMock
                .Setup(s => s.PasswordSignInAsync("test@test.com", "Password1!", true, false))
                .ReturnsAsync(SignInStatus.Success);

            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object);

            var httpContextMock = new Mock<HttpContextBase>();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);
            controller.Url = urlHelperMock.Object;

            var model = new LoginViewModel
            {
                Email = "test@test.com",
                Password = "Password1!",
                RememberMe = true
            };

           
            var result = await controller.Login(model, "/Home") as RedirectResult;

            
            Assert.NotNull(result);
            Assert.Equal("/Home", result.Url);
        }


        [Fact]
        public async Task Register_Post_ValidModel_CreatesUser()
        {
       
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var mockUserMgr = new Mock<ApplicationUserManager>(userStoreMock.Object);
            var authManagerMock = new Mock<IAuthenticationManager>();

            mockUserMgr
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), "Password1!"))
                .ReturnsAsync(IdentityResult.Success);

            var mockSignIn = new Mock<ApplicationSignInManager>(mockUserMgr.Object, authManagerMock.Object);
            mockSignIn
                .Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), false, false))
                .Returns(Task.CompletedTask);

            var controller = new AccountController(mockUserMgr.Object, mockSignIn.Object);

        
            var httpContextMock = new Mock<HttpContextBase>();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var model = new RegisterViewModel
            {
                Email = "new@test.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

     
            var result = await controller.Register(model) as RedirectToRouteResult;

           
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
            Assert.Equal("Home", result.RouteValues["controller"]);

           
            mockUserMgr.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), "Password1!"), Times.Once);
            mockSignIn.Verify(s => s.SignInAsync(It.IsAny<ApplicationUser>(), false, false), Times.Once);
        }


        [Fact]
        public void LogOff_RedirectsToHome()
        {
            var authManagerMock = new Mock<IAuthenticationManager>();
            authManagerMock.Setup(a => a.SignOut(It.IsAny<string[]>()));

            var controller = new TestableAccountController(authManagerMock.Object);

            var result = controller.LogOff() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
            Assert.Equal("Home", result.RouteValues["controller"]);

            authManagerMock.Verify(a => a.SignOut(DefaultAuthenticationTypes.ApplicationCookie), Times.Once);
        }








    }

}
