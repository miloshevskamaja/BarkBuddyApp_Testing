using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ToysTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Toys_ShouldBeValid_WhenEverythingIsOk()
    {
        var model = new Toys
        {
            Id = 1,
            Name = "Ball",
            ImageUrl = "https://m.media-amazon.com/images/I/71ebo8wSSrL.jpg",
            Description = "This is a dog ball.",
            Price = 10.5
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Toys_ShouldBeInvalid_WhenIdIsLessOrEqualToZero()
    {
        var model = new Toys
        {
            Id = 0,
            Name = "Ball",
            ImageUrl = "https://m.media-amazon.com/images/I/71ebo8wSSrL.jpg",
            Description = "This is a dog ball.",
            Price = 10.5
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Id must be positive."));
    }

    [Fact]
    public void Toys_ShouldBeInvalid_WhenNameIsShorterThanThreeChars()
    {
        var model = new Toys
        {
            Id = 1,
            Name = "Ba",
            ImageUrl = "https://m.media-amazon.com/images/I/71ebo8wSSrL.jpg",
            Description = "This is a dog ball.",
            Price = 10.5
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Name must be at least 3 characters long."));
    }

    [Fact]
    public void Toys_ShouldBeInvalid_WhenPriceIsLessThanZero()
    {
        var model = new Toys
        {
            Id = 1,
            Name = "Ball",
            ImageUrl = "https://m.media-amazon.com/images/I/71ebo8wSSrL.jpg",
            Description = "This is a dog ball.",
            Price = -3.0
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Price should be >= 0."));
    }
}
