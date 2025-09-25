using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class OrderViewModelTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void OrderViewModel_ShouldBeValid_WhenAllFieldsSet()
    {
        var model = new OrderViewModel
        {
            Id = 1,
            Buyer = new Buyer { Name = "John Doe", Email = "john@example.com" }, 
            ShoppingCartItems = new List<ShoppingCartItem>
            {
                new ShoppingCartItem { ProductId = 1, Quantity = 2 }
            }
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty(); 
    }
    [Fact]
    public void OrderViewModel_ShouldBeInvalid_WhenBuyerIsNull()
    {
        var model = new OrderViewModel
        {
            Id = 2,
            Buyer = null,
            ShoppingCartItems = new List<ShoppingCartItem>()
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.MemberNames.Contains("Buyer"));
    }

    [Fact]
    public void OrderViewModel_ShouldBeInvalid_WhenBuyerIsMissing()
    {
        var model = new OrderViewModel
        {
            Id = 2,
            Buyer = null,
            ShoppingCartItems = new List<ShoppingCartItem>()
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.MemberNames.Contains("Buyer")); 
    }

}
