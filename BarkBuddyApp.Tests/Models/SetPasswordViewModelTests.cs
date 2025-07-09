using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class SetPasswordViewModelTests
{

    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    [Fact]
    public void SetPassword_ShouldBeInvalid_WhenPasswordIsTooShort()
    {
        var model = new SetPasswordViewModel
        {
            NewPassword = "123",
            ConfirmPassword = "123"
        };

        var results = ValidateModel(model);
        results.Should().Contain(r => r.MemberNames.Contains("NewPassword"));
    }

    [Fact]
    public void SetPassword_ShouldBeInvalid_WhenPasswordsDoNotMatch()
    {
        var model = new SetPasswordViewModel
        {
            NewPassword = "Password123!",
            ConfirmPassword = "Mismatch123!"
        };

        var results = ValidateModel(model);
        results.Should().Contain(r => r.MemberNames.Contains("ConfirmPassword"));
    }

    [Fact]
    public void SetPassword_ShouldBeValid_WhenPasswordsMatchAndValid()
    {
        var model = new SetPasswordViewModel
        {
            NewPassword = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }
}
