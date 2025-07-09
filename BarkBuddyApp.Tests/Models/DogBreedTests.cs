using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class DogBreedTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void DogBreed_ShouldBeValid_WhenEverythingIsOk()
    {
        var model = new DogBreed
        {
            Id = 1,
            ImageURL = "https://content.lyka.com.au/f/1016262/1104x676/f7ee8782a7/chihuahua.webp/m/640x427/smart",
            Name = "Chihuahua",
            Description = "This is a dog breed",
            Products = new HashSet<Product>()
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void DogBreed_ShouldBeInvalid_WhenIdIsLessOrEqualToZero()
    {
        var model = new DogBreed
        {
            Id = -1,
            ImageURL = "https://content.lyka.com.au/f/1016262/1104x676/f7ee8782a7/chihuahua.webp/m/640x427/smart",
            Name = "Chihuahua",
            Description = "This is a dog breed",
            Products = new HashSet<Product>()
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Id must be positive."));
    }

    [Fact]
    public void DogBreed_ShouldBeInvalid_WhenNameIsShorterThanThreeChars()
    {
        var model = new DogBreed
        {
            Id = 1,
            ImageURL = "https://content.lyka.com.au/f/1016262/1104x676/f7ee8782a7/chihuahua.webp/m/640x427/smart",
            Name = "?",
            Description = "This is a dog breed",
            Products = new HashSet<Product>()
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Name must be at least 3 characters long."));
    }
}
