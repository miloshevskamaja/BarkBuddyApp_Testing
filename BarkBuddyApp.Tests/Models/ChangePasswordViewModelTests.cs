using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ChangePasswordViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void ChangePassword_ShouldBeInvalid_WhenOldPasswordIsMissing()
    {
        var model = new ChangePasswordViewModel
        {
            OldPassword = null,
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.MemberNames.Contains("OldPassword"));
    }

    [Fact]
    public void ChangePassword_ShouldBeInvalid_WhenNewPasswordIsMissing()
    {
        var model = new ChangePasswordViewModel
        {
            OldPassword = "OldPassword123!",
            NewPassword = null,
            ConfirmPassword = null
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.MemberNames.Contains("NewPassword"));
    }

    [Fact]
    public void ChangePassword_ShouldBeInvalid_WhenNewPasswordIsTooShort()
    {
        var model = new ChangePasswordViewModel
        {
            OldPassword = "OldPassword123!",
            NewPassword = "123",
            ConfirmPassword = "123"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.MemberNames.Contains("NewPassword")
                                          && r.ErrorMessage.Contains("at least"));
    }

    [Fact]
    public void ChangePassword_ShouldBeInvalid_WhenPasswordsDoNotMatch()
    {
        var model = new ChangePasswordViewModel
        {
            OldPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.MemberNames.Contains("ConfirmPassword")
                                          && r.ErrorMessage.Contains("do not match"));
    }

    [Fact]
    public void ChangePassword_ShouldBeValid_WhenAllFieldsAreCorrect()
    {
        var model = new ChangePasswordViewModel
        {
            OldPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }
}
