using System.Globalization;
using NUnit.Framework;

namespace Service.Tests.PropertiesTests
{
    [TestFixture]
    public class ResourcesTests
    {
        [Test]
        public void ResourcesTest()
        {
            Assert.AreEqual("dataAccess", Properties.Resources.DataAccessObjectName);
            Assert.AreEqual("Database.sqlite", Properties.Resources.DatabaseName);
            Assert.AreEqual("albums.csv", Properties.Resources.DataFile);
            Assert.AreEqual("The Id or the name must be provided.", Properties.Resources.IdOrNameNotProvided);
            Assert.AreEqual(null, Properties.Resources.Culture);

            Properties.Resources.Culture = new CultureInfo("en-US");
            Assert.AreEqual(new CultureInfo("en-US"), Properties.Resources.Culture);
        }
    }
}
