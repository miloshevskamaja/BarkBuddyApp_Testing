using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class AddPhoneNumberViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    [Fact]
    public void AddPhoneNumber_ShouldBeInvalid_WhenNumberMissing()
    {
        var model = new AddPhoneNumberViewModel { Number = null };
        var results = ValidateModel(model);
        results.Should().ContainSingle(r => r.MemberNames.Contains("Number"));
    }

    [Fact]
    public void AddPhoneNumber_ShouldBeValid_WithProperPhoneNumber()
    {
        var model = new AddPhoneNumberViewModel { Number = "+38970111222" };
        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }
}
