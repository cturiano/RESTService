using NUnit.Framework;
using Service.DAL;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class OperatorTests
    {
        [Test]
        public void EqualsTests()
        {
            Assert.IsFalse(Operator.Equal.Equals((object)null));
            Assert.IsTrue(Operator.Equal.Equals((object)Operator.Equal));
        }

        [Test]
        public void GetHashCodeTests()
        {
            Assert.AreNotEqual(Operator.Equal.GetHashCode(), Operator.LessThan.GetHashCode());
            Assert.AreEqual(Operator.Equal.GetHashCode(), Operator.Equal.GetHashCode());
        }

        [Test]
        public void PropertiesTests()
        {
            Assert.AreEqual("=", Operator.Equal.ToString());
            Assert.AreEqual("==", Operator.Equalequal.ToString());
            Assert.AreEqual("<>", Operator.GreaterOrLesser.ToString());
            Assert.AreEqual(">", Operator.GreaterThan.ToString());
            Assert.AreEqual(">=", Operator.GreaterThanOrEqual.ToString());
            Assert.AreEqual("<", Operator.LessThan.ToString());
            Assert.AreEqual("<=", Operator.LessThanOrEqual.ToString());
            Assert.AreEqual("!=", Operator.NotEqual.ToString());
            Assert.AreEqual("!>", Operator.NotGreaterThan.ToString());
            Assert.AreEqual("!<", Operator.NotLessThan.ToString());
        }
    }
}