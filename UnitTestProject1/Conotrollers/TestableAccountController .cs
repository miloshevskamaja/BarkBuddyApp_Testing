using BarkBuddyApp.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UnitTestProject1.Conotrollers
{
    public class TestableAccountController : AccountController
    {
        private readonly IAuthenticationManager _authManager;

        public TestableAccountController(IAuthenticationManager authManager)
        {
            _authManager = authManager;
        }

        // нов public/protected property за тестирање
        public IAuthenticationManager TestAuthenticationManager => _authManager;

        // override LogOff да користи TestAuthenticationManager наместо private AuthenticationManager
        public override ActionResult LogOff()
        {
            TestAuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }


}
