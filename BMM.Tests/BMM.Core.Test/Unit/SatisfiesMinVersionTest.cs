using BMM.Core.Implementations.FeatureToggles;
using NUnit.Framework;

namespace BMM.Core.Test.Unit
{
    public class SatisfiesMinVersionTest
    {
        [Test]
        [TestCase("19.2.1", "19.2.1", true, Description = "Equal versions are satisfying.")]
        [TestCase("20.2.1", "19.2.1", true, Description = "It's satisfying if version in question has larger major number.")]
        [TestCase("19.2.1", "20.2.1", false, Description = "Minimum required version can't have higher major number.")]
        [TestCase("19.7.1", "19.2.1", true, Description = "It's satisfying if version in question has larger minor number.")]
        [TestCase("19.2.1", "19.8.1", false, Description = "Minimum required version can't have higher minor number.")]
        [TestCase("19.2.16", "19.2.5", true, Description = "It's satisfying if version in question has larger patch number.")]
        [TestCase("19.2.2", "19.2.24", false, Description = "Minimum required version can't have higher patch number.")]
        [TestCase("2.0.0", "1.11.654", true, Description = "App version in question is shorter but still higher version.")]
        [TestCase("0.0.1", "0.0.654", false, Description = "Zeroes as first numbers.")]
        [TestCase("1298.382198.032731", "38231.28373.8493872", false, Description = "Random long strings but still version in question doesn't satisfies required minimum.")]
        [TestCase("0.0.0", "0.0.0", true, Description = "All zeroes just in case.")]
        [TestCase("12.2.0-alpha3", "9.1.0", true, Description = "Version in question has 'alpha' version.")]
        [TestCase("14.9.2-alpha", "13.2.1-beta4", true, Description = "Version in question doesn't have 'alpha' number.")]
        [TestCase("1.12", "0.1", true, Description = "Only major and minor.")]
        [TestCase("8", "7", true, Description = "Only major.")]
        public void SatisfiesMinVersion(string appVersionInQuestion, string minAppVersion, bool expectation)
        {
            // Arrange
            SemanticVersionComparer appVersionComparer = new SemanticVersionComparer();
            SemanticVersionParser semanticVersionParser = new SemanticVersionParser();

            var appVersionInQuestionObject = semanticVersionParser.ParseStringToSemanticVersionObject(appVersionInQuestion);
            var minAppVersionObject = semanticVersionParser.ParseStringToSemanticVersionObject(minAppVersion);

            // Act
            var result = appVersionComparer.SatisfiesMinVersion(appVersionInQuestionObject, minAppVersionObject);

            // Assert
            Assert.AreEqual(expectation, result);
        }
    }
}
