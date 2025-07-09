using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class RegisterViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void RegisterViewModel_ShouldBeInvalid_WhenEmailIsEmpty()
    {
       
        var model = new RegisterViewModel
        {
            Email = "",
            Password = "StrongPassword123!",
            ConfirmPassword = "StrongPassword123!"
        };

       
        var results = ValidateModel(model);

      
        results.Should().ContainSingle(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void RegisterViewModel_ShouldBeInvalid_WhenPasswordsDoNotMatch()
    {
      
        var model = new RegisterViewModel
        {
            Email = "test@example.com",
            Password = "Password1!",
            ConfirmPassword = "Password2!"
        };

       
        var results = ValidateModel(model);

       
        results.Should().ContainSingle(r => r.MemberNames.Contains("ConfirmPassword")
                                            && r.ErrorMessage.Contains("do not match", System.StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RegisterViewModel_ShouldBeInvalid_WhenPasswordTooShort()
    {
      
        var model = new RegisterViewModel
        {
            Email = "test@example.com",
            Password = "123",
            ConfirmPassword = "123"
        };

       
        var results = ValidateModel(model);

        
        results.Should().Contain(r => r.MemberNames.Contains("Password")
                                   && r.ErrorMessage.Contains("at least", System.StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RegisterViewModel_ShouldBeValid_WhenCorrectInput()
    {
        var model = new RegisterViewModel
        {
            Email = "valid@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

  
        var results = ValidateModel(model);

  
        results.Should().BeEmpty();
    }
}
