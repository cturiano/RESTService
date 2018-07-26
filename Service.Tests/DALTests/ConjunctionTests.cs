using NUnit.Framework;
using Service.DAL;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class ConjunctionTests
    {
        [Test]
        public void PropertiesTests()
        {
            Assert.AreEqual("AND", Conjunction.And.ToString());
            Assert.AreEqual("BETWEEN", Conjunction.Between.ToString());
            Assert.AreEqual("||", Conjunction.ConCat.ToString());
            Assert.AreEqual("EXISTS", Conjunction.Exists.ToString());
            Assert.AreEqual("GLOB", Conjunction.Glob.ToString());
            Assert.AreEqual("IN", Conjunction.In.ToString());
            Assert.AreEqual("IS", Conjunction.Is.ToString());
            Assert.AreEqual("IS NOT", Conjunction.IsNot.ToString());
            Assert.AreEqual("IS NULL", Conjunction.IsNull.ToString());
            Assert.AreEqual("LIKE", Conjunction.Like.ToString());
            Assert.AreEqual("NOT", Conjunction.Not.ToString());
            Assert.AreEqual("NOT IN", Conjunction.NotIn.ToString());
            Assert.AreEqual("OR", Conjunction.Or.ToString());
            Assert.AreEqual("UNIQUE", Conjunction.Unique.ToString());
            Assert.AreEqual(string.Empty, Conjunction.None.ToString());
        }
    }
}