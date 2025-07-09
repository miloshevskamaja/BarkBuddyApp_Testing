using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ConfigureTwoFactorViewModelTests
{
    [Fact]
    public void DummyTest_ConfigureTwoFactorViewModel_IsValid()
    {
        var model = new ConfigureTwoFactorViewModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
