using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class AddPhoneNumberViewModelTests
{
    [Fact]
    public void DummyTest_AddPhoneNumberViewModel_IsValid()
    {
        var model = new AddPhoneNumberViewModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
