using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ForgotViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
    [Fact]
    public void ForgotViewModel_ShouldBeInvalid_WhenEmailMissing()
    {
        var model = new ForgotViewModel { Email = null };
        var results = ValidateModel(model);
        results.Should().ContainSingle(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void ForgotViewModel_ShouldBeValid_WhenEmailIsProvided()
    {
        var model = new ForgotViewModel { Email = "test@example.com" };
        var results = ValidateModel(model);
        results.Should().BeEmpty();
    }

}
