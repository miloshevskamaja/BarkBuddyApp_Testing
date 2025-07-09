using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class GroomingTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Grooming_ShouldBeValid_WhenEverythingIsOk()
    {
        var model = new Grooming
        {
            Id = 1,
            ReservationDateTime = DateTime.Now,
            DogBreed = "Chihuahua",
            DogAge = 7,
            Details = "My dog"
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Grooming_ShouldBeInvalid_WhenIdIsLessOrEqualToZero()
    {
        var model = new Grooming
        {
            Id = -1,
            ReservationDateTime = DateTime.Now,
            DogBreed = "Chihuahua",
            DogAge = 7,
            Details = "My dog"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Id must be positive."));
    }

    [Fact]
    public void Grooming_ShouldBeInvalid_WhenNameIsShorterThanThreeChars()
    {
        var model = new Grooming
        {
            Id = -1,
            ReservationDateTime = DateTime.Now,
            DogBreed = "C",
            DogAge = 7,
            Details = "My dog"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Name must be at least 3 characters long."));
    }

    [Fact]
    public void Grooming_ShouldBeInvalid_WhenAgeLessThanZero()
    {
        var model = new Grooming
        {
            Id = 1,
            ReservationDateTime = DateTime.Now,
            DogBreed = "Chihuahua",
            DogAge = -5,
            Details = "My dog"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Age should be >= 0."));
    }
}
