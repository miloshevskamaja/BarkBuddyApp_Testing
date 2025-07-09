using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class BuyerTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Buyer_ShouldBeValid_WhenIdIsIntAndElseIsString()
    {
        var model = new Buyer
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            ContactNumber = "075111222",
            Email = "john@yahoo.com",
            Address = "Taki Daskalo 20",
            PostalCode = "7000",
            City = "Bitola",
            Country = "Macedonia"
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Buyer_ShouldBeInvalid_WhenIdIsInvalid()
    {
        var model = new Buyer
        {
            Id = 0,
            Name = "John",
            Surname = "Doe",
            ContactNumber = "075111222",
            Email = "john@yahoo.com",
            Address = "Taki Daskalo 20",
            PostalCode = "7000",
            City = "Bitola",
            Country = "Macedonia"
        };

        var results = ValidateModel(model);

        results.Should().ContainSingle(r => r.ErrorMessage.Contains("Id must be positive number"));
    }
}
