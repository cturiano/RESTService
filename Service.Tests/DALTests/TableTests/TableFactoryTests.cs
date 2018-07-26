using NUnit.Framework;
using Service.DAL.Tables;
using Service.Models;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.Tests.DALTests.TableTests
{
    [TestFixture]
    public class TableFactoryTests
    {
        /// <exception cref="MissingMethodException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the
        ///     base class exception, <see cref="T:System.MissingMemberException" />, instead.The type that is specified for
        ///     <paramref name="T" /> does not have a parameterless constructor.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The property is retrieved and <paramref name="key" /> does not exist in the
        ///     collection.
        /// </exception>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        ///     <paramref name="value" /> is null or <paramref name="expectedType" /> is not found in the inheritance hierarchy of
        ///     <paramref name="value" />.
        /// </exception>
        [Test]
        public void GetTableTest()
        {
            var t1 = TableFactory<Album>.GetTable<AlbumTable>();
            Assert.IsInstanceOf(typeof(AlbumTable), t1);

            var t2 = TableFactory<Artist>.GetTable<ArtistTable>();
            Assert.IsInstanceOf(typeof(ArtistTable), t2);

            var t3 = TableFactory<Genre>.GetTable<GenreTable>();
            Assert.IsInstanceOf(typeof(GenreTable), t3);

            var s1 = TableFactory<Album>.GetTable<AlbumTable>();
            Assert.IsTrue(ReferenceEquals(t1, s1));

            var s2 = TableFactory<Artist>.GetTable<ArtistTable>();
            Assert.IsTrue(ReferenceEquals(t2, s2));

            var s3 = TableFactory<Genre>.GetTable<GenreTable>();
            Assert.IsTrue(ReferenceEquals(t3, s3));
        }
    }
}