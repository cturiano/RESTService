using System.Data;
using NUnit.Framework;
using Service.Extensions;

namespace Service.Tests.ExtensionsTests
{
    [TestFixture]
    public class SqliteExtensionsTests
    {
        [Test]
        public void CreateTest()
        {
            var parameterName = "parameter";
            var type = DbType.Boolean;
            var value = true;

            var p = SqliteExtensions.Create(parameterName, type, value);
            Assert.AreEqual("@" + parameterName, p.ParameterName);
            Assert.AreEqual(type, p.DbType);
            Assert.AreEqual(value, p.Value);

            parameterName = "@parameter";

            p = SqliteExtensions.Create(parameterName, type, value);
            Assert.AreEqual(parameterName, p.ParameterName);
            Assert.AreEqual(type, p.DbType);
            Assert.AreEqual(value, p.Value);
        }
    }
}