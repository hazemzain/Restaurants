using FluentValidation.TestHelper;
using Xunit;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant.Tests;

public class CreateRestaurantCommandValidatorTests
{
    [Fact()]
    public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
    {
        // arrange

        var command = new CreateRestaurantCommand()
        {
            Name = "Test",
            Category = "Italian",
            ContactEmail = "test@test.com",
            PostalCode = "12-345",
        };

        var validator = new CreateRestaurantCommandValidator();

        // act

        var result = validator.TestValidate(command);

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void Validator_ForInvalidCommand_ShouldHaveValidationErrors()
    {
        // arrange

        var command = new CreateRestaurantCommand()
        {
            Name = "Te",
            Category = "Ita",
            ContactEmail = "@test.com",
            PostalCode = "12345",
        };

        var validator = new CreateRestaurantCommandValidator();

        // act

        var result = validator.TestValidate(command);

        // assert

        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Category);
        result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }


    [Theory()]
    [InlineData("Italian")]
    [InlineData("Mexican")]
    [InlineData("Japanese")]
    [InlineData("American")]
    [InlineData("Indian")]
    public void Validator_ForValidCategory_ShouldNotHaveValidationErrorsForCategoryProperty(string category)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { Category = category };

        // act

        var result = validator.TestValidate(command);

        // assert
        result.ShouldNotHaveValidationErrorFor(c => c.Category);

    }

    [Theory()]
    [InlineData("10220")]
    [InlineData("102-20")]
    [InlineData("10 220")]
    [InlineData("10-2 20")]
    public void Validator_ForInvalidPostalCode_ShouldHaveValidationErrorsForPostalCodeProperty(string postalCode)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { PostalCode = postalCode };

        // act

        var result = validator.TestValidate(command);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }
    [Fact]
    public void Validator_ForMissingRequiredFields_ShouldHaveValidationErrors()
    {
        // arrange
        var command = new CreateRestaurantCommand
        {
            Name = "", // Empty name
            Category = "", // Empty category
            ContactEmail = "", // Empty email
            PostalCode = "" // Empty postal code
        };

        var validator = new CreateRestaurantCommandValidator();

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Category);
        result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }
    [Theory]
    [InlineData("A")]  // Too short
    [InlineData("AB")] // Too short
    [InlineData("ThisIsAVeryLongRestaurantNameThatExceedsOneHundredCharactersWhichShouldFailValidationBecauseItIsTooLongAndInvalid")] // Too long
    public void Validator_ForInvalidNameLength_ShouldHaveValidationErrors(string name)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { Name = name };

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }
    [Theory]
    [InlineData("French")]
    [InlineData("Thai")]
    [InlineData("UnknownCategory")]
    public void Validator_ForInvalidCategory_ShouldHaveValidationError(string category)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { Category = category };

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.Category);
    }
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("name.surname@domain.org")]
    [InlineData("contact@company.net")]
    public void Validator_ForValidEmail_ShouldNotHaveValidationError(string email)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { ContactEmail = email };

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldNotHaveValidationErrorFor(c => c.ContactEmail);
    }
    [Theory]
    [InlineData("plainaddress")]
    [InlineData("missing@dotcom")]
    [InlineData("wrong@format.")]
    public void Validator_ForInvalidEmail_ShouldHaveValidationError(string email)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { ContactEmail = email };

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
    }
    [Theory]
    [InlineData("12-345")]
    [InlineData("01-001")]
    [InlineData("99-999")]
    public void Validator_ForValidPostalCode_ShouldNotHaveValidationErrors(string postalCode)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { PostalCode = postalCode };

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldNotHaveValidationErrorFor(c => c.PostalCode);
    }
    [Theory]
    [InlineData("12345")]  // Missing dash
    [InlineData("123-45")] // Wrong format
    [InlineData("12 345")] // Space instead of dash
    public void Validator_ForInvalidPostalCode_ShouldHaveValidationErrors(string postalCode)
    {
        // arrange
        var validator = new CreateRestaurantCommandValidator();
        var command = new CreateRestaurantCommand { PostalCode = postalCode };

        // act
        var result = validator.TestValidate(command);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }

}