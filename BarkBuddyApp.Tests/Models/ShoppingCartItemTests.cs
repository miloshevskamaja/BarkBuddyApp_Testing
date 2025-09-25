using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ShoppingCartItemTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void ShoppingCartItem_ShouldBeValid_WhenEverythingIsOk()
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = "Dog Toy",
            Price = 10.5,
            Quantity = 2,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ShoppingCartItem_ShouldBeInvalid_WhenIdIsLessOrEqualToZero(int invalidId)
    {
        var model = new ShoppingCartItem
        {
            Id = invalidId,
            ProductName = "Dog Toy",
            Price = 10.5,
            Quantity = 2,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Id must be positive."));
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShoppingCartItem_ShouldBeInvalid_WhenProductNameIsNullOrEmpty(string invalidName)
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = invalidName,
            Price = 10.5,
            Quantity = 2,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("ProductName is required"));
    }

    [Fact]
    public void ShoppingCartItem_ShouldBeInvalid_WhenProductNameIsTooShort()
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = "a",  
            Price = 10.5,
            Quantity = 2,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("ProductName must be between 2 and 100 characters"));
    }

    [Fact]
    public void ShoppingCartItem_ShouldBeValid_WhenProductNameIsExactlyMinimumLength()
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = "ab",  
            Price = 10.5,
            Quantity = 2,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void ShoppingCartItem_ShouldBeInvalid_WhenPriceIsNotPositive(double invalidPrice)
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = "Dog Toy",
            Price = invalidPrice,
            Quantity = 2,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Price must be positive"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ShoppingCartItem_ShouldBeInvalid_WhenQuantityIsLessThanOne(int invalidQuantity)
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = "Dog Toy",
            Price = 10.5,
            Quantity = invalidQuantity,
            OrderId = 1,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Quantity must be at least 1"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ShoppingCartItem_ShouldBeInvalid_WhenOrderIdIsLessOrEqualToZero(int invalidOrderId)
    {
        var model = new ShoppingCartItem
        {
            Id = 1,
            ProductName = "Dog Toy",
            Price = 10.5,
            Quantity = 2,
            OrderId = invalidOrderId,
            ProductId = 1
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("OrderId must be positive"));
    }
}
