using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class SendCodeViewModelTests
{
    [Fact]
    public void SendCodeViewModel_ShouldSetProperties()
    {
        var model = new SendCodeViewModel
        {
            SelectedProvider = "Email",
            Providers = new List<System.Web.Mvc.SelectListItem>(),
            ReturnUrl = "/return",
            RememberMe = true
        };

        model.SelectedProvider.Should().Be("Email");
        model.Providers.Should().NotBeNull();
        model.ReturnUrl.Should().Be("/return");
        model.RememberMe.Should().BeTrue();
    }
}
