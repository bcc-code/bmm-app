using BMM.Core.Implementations.Validators;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Validators
{
    [TestFixture]
    public class MailValidatorTests
    {
        [Test]
        public void ValidatorShouldReturnTrue_IfMailIsValid()
        {
            // Arrange
            var validator = new MailValidator();

            // Act
            var result = validator.ValidateMail("example@gmail.com");

            // Assert
            Assert.True(result);
        }

        [Test]
        public void ValidatorShouldReturnFalse_IfMailIsInvalid()
        {
            // Arrange
            var validator = new MailValidator();

            // Act
            var result = validator.ValidateMail("example@gmailcom");

            // Assert
            Assert.False(result);
        }
    }
}
