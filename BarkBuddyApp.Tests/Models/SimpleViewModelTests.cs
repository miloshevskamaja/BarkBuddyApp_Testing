using BarkBuddyApp.Models;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;


public class SimpleViewModelTests
{
    [Fact]
    public void IndexViewModel_ShouldAllowSettingAllProperties()
    {
        var model = new IndexViewModel
        {
            HasPassword = true,
            Logins = new List<UserLoginInfo>(),
            PhoneNumber = "070123456",
            TwoFactor = true,
            BrowserRemembered = false
        };

        model.HasPassword.Should().BeTrue();
        model.Logins.Should().NotBeNull();
        model.PhoneNumber.Should().Be("070123456");
        model.TwoFactor.Should().BeTrue();
        model.BrowserRemembered.Should().BeFalse();
    }

    [Fact]
    public void ManageLoginsViewModel_ShouldAllowSettingCollections()
    {
        var model = new ManageLoginsViewModel
        {
            CurrentLogins = new List<UserLoginInfo>(),
            OtherLogins = new List<AuthenticationDescription>()
        };

        model.CurrentLogins.Should().NotBeNull();
        model.OtherLogins.Should().NotBeNull();
    }

    [Fact]
    public void FactorViewModel_ShouldStorePurpose()
    {
        var model = new FactorViewModel { Purpose = "Testing" };
        model.Purpose.Should().Be("Testing");
    }

    [Fact]
    public void ConfigureTwoFactorViewModel_ShouldStoreSelectedProvider()
    {
        var model = new ConfigureTwoFactorViewModel
        {
            SelectedProvider = "Email",
            Providers = new List<System.Web.Mvc.SelectListItem>()
        };

        model.SelectedProvider.Should().Be("Email");
        model.Providers.Should().NotBeNull();
    }

    [Fact]
    public void AddToRoleModel_ShouldStoreEmailAndRoles()
    {
        var model = new AddToRoleModel
        {
            Email = "user@example.com",
            Roles = new List<string> { "Admin", "User" },
            selectedRole = "Admin"
        };

        model.Email.Should().Be("user@example.com");
        model.Roles.Should().Contain("Admin");
        model.selectedRole.Should().Be("Admin");
    }
}
