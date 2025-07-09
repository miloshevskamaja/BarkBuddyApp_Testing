using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ShoppingCartTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }


    [Fact]
    public void AddProduct_ShouldAddProductToLists()
    {
        // Arrange
        var cart = new ShoppingCart();

        // Act
        cart.AddProduct("Dog Food", 15.99, 3);

        // Assert
        cart.BuyingProducts.Should().ContainSingle().Which.Should().Be("Dog Food");
        cart.Prices.Should().ContainSingle().Which.Should().Be(15.99);
        cart.Quantities.Should().ContainSingle().Which.Should().Be(3);
    }

    [Fact]
    public void AddMultipleProducts_ShouldAddAllProductsCorrectly()
    {
        // Arrange
        var cart = new ShoppingCart();

        // Act
        cart.AddProduct("Dog Food", 15.99, 3);
        cart.AddProduct("Dog Toy", 9.99, 1);

        // Assert
        cart.BuyingProducts.Should().HaveCount(2);
        cart.Prices.Should().HaveCount(2);
        cart.Quantities.Should().HaveCount(2);

        cart.BuyingProducts.Should().ContainInOrder("Dog Food", "Dog Toy");
        cart.Prices.Should().ContainInOrder(15.99, 9.99);
        cart.Quantities.Should().ContainInOrder(3, 1);
    }

    [Fact]
    public void Constructor_ShouldInitializeEmptyLists()
    {
        // Arrange & Act
        var cart = new ShoppingCart();

        // Assert
        cart.BuyingProducts.Should().NotBeNull().And.BeEmpty();
        cart.Prices.Should().NotBeNull().And.BeEmpty();
        cart.Quantities.Should().NotBeNull().And.BeEmpty();
    }
}

