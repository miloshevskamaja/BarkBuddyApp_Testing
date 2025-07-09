using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ProductTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Product_ShouldBeInvalid_WhenNameIsMissing()
    {
        var product = new Product
        {
            Name = null,
            Price = 10.0,
            Price2 = 100.0,
            ProducerId = 1
        };

        var results = ValidateModel(product);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Product_ShouldBeInvalid_WhenPriceIsZero()
    {
        var product = new Product
        {
            Name = "Dog Food",
            Price = 0.0,
            Price2 = 100.0,
            ProducerId = 1
        };

        var results = ValidateModel(product);

        results.Should().Contain(r => r.MemberNames.Contains("Price"));
    }

    [Fact]
    public void Product_ShouldBeInvalid_WhenPrice2IsNegative()
    {
        var product = new Product
        {
            Name = "Dog Food",
            Price = 10.0,
            Price2 = -5.0,
            ProducerId = 1
        };

        var results = ValidateModel(product);

        results.Should().Contain(r => r.MemberNames.Contains("Price2"));
    }

    [Fact]
    public void Product_ShouldBeInvalid_WhenProducerIdIsMissing()
    {
        var product = new Product
        {
            Name = "Dog Food",
            Price = 10.0,
            Price2 = 100.0,
            ProducerId = 0 
        };

        var results = ValidateModel(product);

        results.Should().Contain(r => r.MemberNames.Contains("ProducerId"));
    }

    [Fact]
    public void Product_ShouldBeValid_WhenAllFieldsCorrect()
    {
        var product = new Product
        {
            Name = "Dog Food",
            Price = 10.0,
            Price2 = 100.0,
            ImageUrl = "https://example.com/image.jpg",
            ProducerId = 1
        };

        var results = ValidateModel(product);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Product_ShouldInitialize_DogBreedsCollection()
    {
        var product = new Product();

        product.DogBreeds.Should().NotBeNull();
        product.DogBreeds.Should().BeEmpty();
    }

    [Fact]
    public void Product_ShouldAllow_AssigningProducerAndDogBreeds()
    {
        var producer = new Producer { Id = 1, Name = "ACME" };
        var dogBreed = new DogBreed { Id = 1, Name = "Labrador" };

        var product = new Product
        {
            Name = "Test",
            Price = 20,
            Price2 = 200,
            ProducerId = 1,
            Producer = producer
        };

        product.DogBreeds.Add(dogBreed);

        product.Producer.Should().Be(producer);
        product.DogBreeds.Should().Contain(dogBreed);
    }
}
