using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ShoppingCartItemTests
{
    [Fact]
    public void DummyTest_ShoppingCartItem_IsValid()
    {
        var model = new ShoppingCartItem();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
