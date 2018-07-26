using NUnit.Framework;
using Service.DAL;
using Service.Extensions;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class IdentifyingInfoTests
    {
        [Test]
        public void ConstructorAndPropertiesTests()
        {
            int? id = HelperObjectFactory.GetRandomInt();
            const string filterText = "filter text";
            var name = StringExtensions.GetRandomStringAsync(25).Result;

            var ii = HelperObjectFactory.MakeIdentifyingInfo(id, filterText, name);
            Assert.AreEqual(id, ii.Id);
            Assert.AreEqual(filterText, ii.FilterText);
            Assert.AreEqual(name, ii.Name);

            var ii2 = new IdentifyingInfo(ii);
            Assert.AreEqual(id, ii2.Id);
            Assert.AreEqual(filterText, ii2.FilterText);
            Assert.AreEqual(name, ii2.Name);
        }

        [Test]
        public void EqualsTest()
        {
            var swc1 = HelperObjectFactory.MakeIdentifyingInfo();
            var swc2 = HelperObjectFactory.MakeIdentifyingInfo();
            var swc3 = new IdentifyingInfo(swc1);

            Assert.IsFalse(swc1.Equals((object)null));
            Assert.IsFalse(swc1.Equals(null));

            Assert.IsTrue(swc1.Equals((object)swc1));
            Assert.IsTrue(swc1.Equals(swc1));

            Assert.IsFalse(swc1.Equals(swc2));
            Assert.IsTrue(swc1.Equals(swc3));
        }

        [Test]
        public void GetHashCodeTest()
        {
            var swc1 = HelperObjectFactory.MakeIdentifyingInfo();
            var swc2 = HelperObjectFactory.MakeIdentifyingInfo();
            var swc3 = new IdentifyingInfo(swc1);

            Assert.AreEqual(swc1.GetHashCode(), swc1.GetHashCode());
            Assert.AreEqual(swc1.GetHashCode(), swc3.GetHashCode());
            Assert.AreNotEqual(swc1.GetHashCode(), swc2.GetHashCode());
        }
    }
}