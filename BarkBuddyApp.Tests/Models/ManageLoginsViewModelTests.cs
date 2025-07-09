using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ManageLoginsViewModelTests
{
    [Fact]
    public void DummyTest_ManageLoginsViewModel_IsValid()
    {
        var model = new ManageLoginsViewModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
