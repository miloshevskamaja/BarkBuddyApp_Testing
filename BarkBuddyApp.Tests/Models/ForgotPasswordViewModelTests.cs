using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ForgotPasswordViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void ForgotPasswordViewModel_ShouldBeInvalid_WhenEmailIsMissing()
    {
        var model = new ForgotPasswordViewModel
        {
            Email = null
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void ForgotPasswordViewModel_ShouldBeInvalid_WhenEmailIsNotValid()
    {
        var model = new ForgotPasswordViewModel
        {
            Email = "not-an-email"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void ForgotPasswordViewModel_ShouldBeValid_WhenEmailIsCorrect()
    {
        var model = new ForgotPasswordViewModel
        {
            Email = "user@example.com"
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty(); 
    }
}
