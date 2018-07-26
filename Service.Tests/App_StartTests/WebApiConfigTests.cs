using System.Web.Http;
using NUnit.Framework;

namespace Service.Tests
{
    [TestFixture]
    public class WebApiConfigTests
    {
        [Test]
        public void RegisterTest()
        {
            using (var config = new HttpConfiguration())
            {
                WebApiConfig.Register(config);
                Assert.AreEqual(4, config.Routes.Count);
            }
        }
    }
}