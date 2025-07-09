using Xunit;
using System.ComponentModel.DataAnnotations;
using BarkBuddyApp.Models;
using FluentAssertions;
using System.Collections.Generic;

public class ProducerTests
{
    [Fact]
    public void DummyTest_Producer_IsValid()
    {
        var model = new Producer();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        results.Should().NotBeNull();
    }
}
