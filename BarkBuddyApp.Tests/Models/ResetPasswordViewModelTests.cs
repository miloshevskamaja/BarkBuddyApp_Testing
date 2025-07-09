using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ResetPasswordViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
    [Fact]
    public void ResetPasswordViewModel_ShouldBeInvalid_WhenFieldsMissing()
    {
        var model = new ResetPasswordViewModel();
        var results = ValidateModel(model);

        results.Should().Contain(r => r.MemberNames.Contains("Email"));
        results.Should().Contain(r => r.MemberNames.Contains("Password"));
    }

    [Fact]
    public void ResetPasswordViewModel_ShouldBeInvalid_WhenPasswordsDoNotMatch()
    {
        var model = new ResetPasswordViewModel
        {
            Email = "test@example.com",
            Password = "Secret123",
            ConfirmPassword = "Other123"
        };

        var results = ValidateModel(model);
        results.Should().ContainSingle(r => r.MemberNames.Contains("ConfirmPassword"));
    }

    [Fact]
    public void ResetPasswordViewModel_ShouldBeValid_WithValidData()
    {
        var model = new ResetPasswordViewModel
        {
            Email = "test@example.com",
            Password = "StrongPassword1!",
            ConfirmPassword = "StrongPassword1!",
            Code = "abc123"
        };

        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }

}
