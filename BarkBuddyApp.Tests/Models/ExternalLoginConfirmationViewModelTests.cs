using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ExternalLoginConfirmationViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
    [Fact]
    public void ExternalLoginConfirmation_ShouldBeInvalid_WhenEmailIsMissing()
    {
        var model = new ExternalLoginConfirmationViewModel { Email = null };
        var results = ValidateModel(model);
        results.Should().ContainSingle(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void ExternalLoginConfirmation_ShouldBeValid_WhenEmailIsProvided()
    {
        var model = new ExternalLoginConfirmationViewModel { Email = "user@example.com" };
        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }

}
