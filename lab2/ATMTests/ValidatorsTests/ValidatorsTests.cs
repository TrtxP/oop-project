using ClassLibraryATM.Validators;
using Xunit;

namespace ATMTests.ValidatorsTests
{
    public class CardValidatorTests
    {
        private readonly CardValidator _validator = new();

        [Fact]
        public void IsValid_ValidCard_ReturnsTrue()
        {
            // Arrange
            string validCard = "1234 5678 9012 3456";

            // Act
            bool result = _validator.IsValid(validCard);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ValidCardNoSpaces_ReturnsTrue()
        {
            // Arrange
            string validCard = "1234567890123456";

            // Act
            bool result = _validator.IsValid(validCard);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_EmptyCard_ReturnsFalse()
        {
            // Arrange
            string invalidCard = "";

            // Act
            bool result = _validator.IsValid(invalidCard);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_NullCard_ReturnsFalse()
        {
            // Arrange
            string? invalidCard = null;

            // Act
            bool result = _validator.IsValid(invalidCard!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_CardWithLetters_ReturnsFalse()
        {
            // Arrange
            string invalidCard = "123A 5678 9012 3456";

            // Act
            bool result = _validator.IsValid(invalidCard);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_CardTooShort_ReturnsFalse()
        {
            // Arrange
            string invalidCard = "1234 5678 9012";

            // Act
            bool result = _validator.IsValid(invalidCard);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetValidationError_ValidCard_ReturnsEmpty()
        {
            // Arrange
            string validCard = "1234 5678 9012 3456";

            // Act
            string error = _validator.GetValidationError(validCard);

            // Assert
            Assert.Empty(error);
        }

        [Fact]
        public void GetValidationError_InvalidCard_ReturnsErrorMessage()
        {
            // Arrange
            string invalidCard = "1234 5678";

            // Act
            string error = _validator.GetValidationError(invalidCard);

            // Assert
            Assert.NotEmpty(error);
        }
    }

    public class PinValidatorTests
    {
        private readonly Validators.PinValidator _validator = new();

        [Fact]
        public void IsValid_ValidPin_ReturnsTrue()
        {
            // Arrange
            string validPin = "1234";

            // Act
            bool result = _validator.IsValid(validPin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_EmptyPin_ReturnsFalse()
        {
            // Arrange
            string invalidPin = "";

            // Act
            bool result = _validator.IsValid(invalidPin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_PinTooShort_ReturnsFalse()
        {
            // Arrange
            string invalidPin = "123";

            // Act
            bool result = _validator.IsValid(invalidPin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_PinWithLetters_ReturnsFalse()
        {
            // Arrange
            string invalidPin = "12A4";

            // Act
            bool result = _validator.IsValid(invalidPin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetValidationError_InvalidPin_ReturnsErrorMessage()
        {
            // Arrange
            string invalidPin = "12";

            // Act
            string error = _validator.GetValidationError(invalidPin);

            // Assert
            Assert.NotEmpty(error);
        }
    }

    public class AmountValidatorTests
    {
        private readonly AmountValidator _validator = new();

        [Fact]
        public void IsValid_PositiveAmount_ReturnsTrue()
        {
            // Arrange
            decimal validAmount = 100m;

            // Act
            bool result = _validator.IsValid(validAmount);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ZeroAmount_ReturnsFalse()
        {
            // Arrange
            decimal invalidAmount = 0m;

            // Act
            bool result = _validator.IsValid(invalidAmount);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_NegativeAmount_ReturnsFalse()
        {
            // Arrange
            decimal invalidAmount = -100m;

            // Act
            bool result = _validator.IsValid(invalidAmount);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetValidationError_InvalidAmount_ReturnsErrorMessage()
        {
            // Arrange
            decimal invalidAmount = -50m;

            // Act
            string error = _validator.GetValidationError(invalidAmount);

            // Assert
            Assert.NotEmpty(error);
        }
    }
}
