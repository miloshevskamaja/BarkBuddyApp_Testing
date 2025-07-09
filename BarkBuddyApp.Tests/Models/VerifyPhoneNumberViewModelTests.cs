using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class VerifyPhoneNumberViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    [Fact]
    public void VerifyPhoneNumber_ShouldBeInvalid_WhenFieldsMissing()
    {
        var model = new VerifyPhoneNumberViewModel();
        var results = ValidateModel(model);
        results.Should().Contain(r => r.MemberNames.Contains("Code"));
        results.Should().Contain(r => r.MemberNames.Contains("PhoneNumber"));
    }

    [Fact]
    public void VerifyPhoneNumber_ShouldBeValid_WhenAllDataPresent()
    {
        var model = new VerifyPhoneNumberViewModel
        {
            Code = "123456",
            PhoneNumber = "+38970111222"
        };
        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }
}
