using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class LoginViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void LoginViewModel_ShouldBeInvalid_WhenEmailIsMissing()
    {
     
        var model = new LoginViewModel
        {
            Email = null,
            Password = "secret"
        };

    
        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginViewModel_ShouldBeInvalid_WhenPasswordIsMissing()
    {
        
        var model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = null
        };

        
        var results = ValidateModel(model);

       
        results.Should().ContainSingle(r => r.MemberNames.Contains("Password"));
    }

    [Fact]
    public void LoginViewModel_ShouldBeValid_WhenAllDataIsCorrect()
    {
      
        var model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = "StrongPassword"
        };

        var results = ValidateModel(model);

        
        results.Should().BeEmpty();
    }
}