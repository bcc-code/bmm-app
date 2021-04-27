using BMM.Core.Helpers;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Helpers
{
    [TestFixture]
    public class HttpUtilityTests
    {
        [Test]
        public void ParseQueryString_SingleValue()
        {
            var key = "key";
            var value = "value";

            var valueCollection = HttpUtility.ParseQueryString(key + "=" + value);

            Assert.AreEqual(valueCollection[key], value);
        }

        [Test]
        public void ParseQueryString_MultipleValues()
        {
            var key1 = "key1";
            var value1 = "value1";

            var key2 = "key2";
            var value2 = "value2";

            var valueCollection = HttpUtility.ParseQueryString(key1 + "=" + value1 + "&" + key2 + "=" + value2);

            Assert.AreEqual(valueCollection[key1], value1);
            Assert.AreEqual(valueCollection[key2], value2);
        }

        [Test]
        public void ParseQueryString_WithPrecedingQuestionMark()
        {
            var key = "key";
            var value = "value";

            var valueCollection = HttpUtility.ParseQueryString("?" + key + "=" + value);

            Assert.AreEqual(valueCollection[key], value);
        }
    }
}