using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class VerifyPhoneNumberViewModelTests
{
    [Fact]
    public void DummyTest_VerifyPhoneNumberViewModel_IsValid()
    {
        var model = new VerifyPhoneNumberViewModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
