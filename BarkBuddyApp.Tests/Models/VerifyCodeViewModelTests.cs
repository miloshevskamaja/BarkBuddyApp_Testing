using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class VerifyCodeViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
    [Fact]
    public void VerifyCodeViewModel_ShouldBeInvalid_WhenProviderOrCodeMissing()
    {
        var model = new VerifyCodeViewModel();
        var results = ValidateModel(model);

        results.Should().Contain(r => r.MemberNames.Contains("Provider"));
        results.Should().Contain(r => r.MemberNames.Contains("Code"));
    }

    [Fact]
    public void VerifyCodeViewModel_ShouldBeValid_WithAllRequiredFields()
    {
        var model = new VerifyCodeViewModel
        {
            Provider = "Email",
            Code = "123456",
            ReturnUrl = "/return",
            RememberBrowser = true,
            RememberMe = false
        };

        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }

}
