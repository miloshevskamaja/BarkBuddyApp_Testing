using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ExternalLoginListViewModelTests
{
    [Fact]
    public void ExternalLoginListViewModel_ShouldSetReturnUrl()
    {
        var model = new ExternalLoginListViewModel { ReturnUrl = "/return" };
        model.ReturnUrl.Should().Be("/return");
    }

}
