using NUnit.Framework;
using Service.DAL;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class SqlWhereClauseTests
    {
        [Test]
        public void ConstructorAndPropertiesTests()
        {
            const string cn = "columnName";
            const string cv = "columnValue";
            var swc = HelperObjectFactory.MakeSqlWhereClause(Conjunction.Between, cn, Operator.NotEqual, cv);
            Assert.AreEqual(Conjunction.Between, swc.Conjunction);
            Assert.AreEqual(cn, swc.ColumnName);
            Assert.AreEqual(Operator.NotEqual, swc.Operator);
            Assert.AreEqual(cv, swc.ColumnValue);

            var swc2 = new SqlWhereClause(swc);
            Assert.AreEqual(swc2.Conjunction, swc.Conjunction);
            Assert.AreEqual(swc2.ColumnName, swc.ColumnName);
            Assert.AreEqual(swc2.Operator, swc.Operator);
            Assert.AreEqual(swc2.ColumnValue, swc.ColumnValue);
        }

        [Test]
        public void DumpConjunctionTest()
        {
            var swc = HelperObjectFactory.MakeSqlWhereClause();
            Assert.AreNotEqual(Conjunction.None, swc.Conjunction);
            swc.DumpConjunction();
            Assert.AreEqual(Conjunction.None, swc.Conjunction);
        }

        [Test]
        public void EqualsTest()
        {
            const string cn = "columnName";
            const string cv = "columnValue";
            var swc1 = HelperObjectFactory.MakeSqlWhereClause();
            var swc2 = HelperObjectFactory.MakeSqlWhereClause(Conjunction.Between, cn, Operator.NotEqual, cv);
            var swc3 = new SqlWhereClause(swc1);

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
            const string cn = "columnName";
            const string cv = "columnValue";
            var swc1 = HelperObjectFactory.MakeSqlWhereClause();
            var swc2 = HelperObjectFactory.MakeSqlWhereClause(Conjunction.Between, cn, Operator.NotEqual, cv);
            var swc3 = new SqlWhereClause(swc1);

            Assert.AreEqual(swc1.GetHashCode(), swc1.GetHashCode());
            Assert.AreEqual(swc1.GetHashCode(), swc3.GetHashCode());
            Assert.AreNotEqual(swc1.GetHashCode(), swc2.GetHashCode());
        }

        [Test]
        public void ToStringTest()
        {
            const string cn = "columnName";
            const string cv = "columnValue";
            var swc1 = HelperObjectFactory.MakeSqlWhereClause();
            var swc2 = HelperObjectFactory.MakeSqlWhereClause(Conjunction.Between, cn, Operator.NotEqual, cv);

            Assert.AreEqual("AND id = @id", swc1.ToString());
            swc1.DumpConjunction();
            Assert.AreEqual("id = @id", swc1.ToString());

            Assert.AreEqual("BETWEEN columnName != columnValue", swc2.ToString());
            swc2.DumpConjunction();
            Assert.AreEqual("columnName != columnValue", swc2.ToString());
        }
    }
}