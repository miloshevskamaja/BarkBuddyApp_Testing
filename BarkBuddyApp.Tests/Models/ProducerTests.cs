using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ProducerTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Producer_ShouldBeInvalid_WhenNameIsMissing()
    {
        var producer = new Producer
        {
            Name = null,
            Logo = "https://example.com/logo.png",
            Description = "Test description"
        };

        var results = ValidateModel(producer);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Producer_ShouldBeInvalid_WhenLogoIsMissing()
    {
        var producer = new Producer
        {
            Name = "Producer Name",
            Logo = null,
            Description = "Test description"
        };

        var results = ValidateModel(producer);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Logo"));
    }

    [Fact]
    public void Producer_ShouldBeInvalid_WhenLogoIsNotAValidUrl()
    {
        var producer = new Producer
        {
            Name = "Producer Name",
            Logo = "invalid-url",
            Description = "Test description"
        };

        var results = ValidateModel(producer);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Logo"));
    }

    [Fact]
    public void Producer_ShouldBeInvalid_WhenNameTooLong()
    {
        var producer = new Producer
        {
            Name = new string('A', 101), // 101 chars
            Logo = "https://example.com/logo.png",
            Description = "Test"
        };

        var results = ValidateModel(producer);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Producer_ShouldBeInvalid_WhenDescriptionTooLong()
    {
        var producer = new Producer
        {
            Name = "Producer Name",
            Logo = "https://example.com/logo.png",
            Description = new string('B', 501) // 501 chars
        };

        var results = ValidateModel(producer);

        results.Should().ContainSingle(r => r.MemberNames.Contains("Description"));
    }

    [Fact]
    public void Producer_ShouldBeValid_WhenAllFieldsAreCorrect()
    {
        var producer = new Producer
        {
            Name = "Happy Dog Foods",
            Logo = "https://example.com/logo.png",
            Description = "High-quality dog food products."
        };

        var results = ValidateModel(producer);

        results.Should().BeEmpty(); // no validation errors
    }
}
