using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class GroomingDogTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void GroomingDog_ShouldBeValid_WhenEverythingIsOk()
    {
        var model = new GroomingDog
        {
            Id = 1,
            Name = "Simba",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/93/Bichon_Fris%C3%A9_-_studdogbichon.jpg/1200px-Bichon_Fris%C3%A9_-_studdogbichon.jpg",
            PriceForGrooming = 20.0
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void GroomingDog_ShouldBeInvalid_WhenIdIsLessOrEqualToZero()
    {
        var model = new GroomingDog
        {
            Id = 0,
            Name = "Simba",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/93/Bichon_Fris%C3%A9_-_studdogbichon.jpg/1200px-Bichon_Fris%C3%A9_-_studdogbichon.jpg",
            PriceForGrooming = 20.0
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Id must be positive."));
    }

    [Fact]
    public void GroomingDog_ShouldBeInvalid_WhenNameIsShorterThanThreeChars()
    {
        var model = new GroomingDog
        {
            Id = 1,
            Name = "Si",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/93/Bichon_Fris%C3%A9_-_studdogbichon.jpg/1200px-Bichon_Fris%C3%A9_-_studdogbichon.jpg",
            PriceForGrooming = 20.0
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Name must be at least 3 characters long."));
    }

    [Fact]
    public void GroomingDog_ShouldBeInvalid_WhenPriceLessThanZero()
    {
        var model = new GroomingDog
        {
            Id = 1,
            Name = "Simba",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/93/Bichon_Fris%C3%A9_-_studdogbichon.jpg/1200px-Bichon_Fris%C3%A9_-_studdogbichon.jpg",
            PriceForGrooming = -5.0
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Price should be >= 0."));
    }
}
