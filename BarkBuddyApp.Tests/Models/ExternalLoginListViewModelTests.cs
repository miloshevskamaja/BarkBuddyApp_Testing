using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ExternalLoginListViewModelTests
{
    [Fact]
    public void DummyTest_ExternalLoginListViewModel_IsValid()
    {
        var model = new ExternalLoginListViewModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
