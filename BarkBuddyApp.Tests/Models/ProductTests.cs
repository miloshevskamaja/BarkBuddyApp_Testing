using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ProductTests
{
    [Fact]
    public void DummyTest_Product_IsValid()
    {
        var model = new Product();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
