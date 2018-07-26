using System;
using NUnit.Framework;
using Service.Models;

namespace Service.Tests.ModelsTests
{
    [TestFixture]
    public class AlbumTests
    {
        [TestCase(1, 2, 3, "A", 1977)]
        [TestCase(1, 2, 3, "12345678901234567890123456789012345678901234567890", 1977)]
        public void ConstructorAndPropertiesTest(int id, int artistId, int genreId, string name, int year)
        {
            var item = new Album(id, artistId, genreId, name, year);
            Assert.AreEqual(id, item.Id);
            Assert.AreEqual(artistId, item.ArtistId);
            Assert.AreEqual(genreId, item.GenreId);
            Assert.AreEqual(name, item.Name);
            Assert.AreEqual(year, item.Year);
        }

        [TestCase(1, 2, 3, null, 1977, typeof(ArgumentNullException))]
        [TestCase(1, 2, 3, "", 1977, typeof(ArgumentNullException))]
        [TestCase(1, 2, 3, "123456789012345678901234567890123456789012345678901", 1977, typeof(ArgumentException))]
        public void ConstructorAndPropertiesExceptionsTest(int id, int artistId, int genreId, string name, int year, Type expectedException)
        {
            Assert.That(() => new Album(id, artistId, genreId, name, year), Throws.Exception.TypeOf(expectedException));
        }

        [TestCase(1, 2, 3, "A", 1977)]
        public void EqualsTest(int id, int artistId, int genreId, string name, int year)
        {
            var item1 = new Album(id, artistId, genreId, name, year);
            var item2 = new Album(id, artistId, genreId, name, year);
            var item3 = new Album(2, 3, 4, "blah", 2015);

            Assert.IsTrue(item1.Equals(item2));
            Assert.IsTrue(item1.Equals((object)item2));
            Assert.IsTrue(item1 == item2);

            Assert.IsFalse(item1.Equals(item3));
            Assert.IsFalse(item1.Equals((object)item3));
            Assert.IsFalse(item1.Equals((object)null));
            Assert.IsFalse(item1.Equals(null));
            Assert.IsTrue(item1 != item3);
        }

        [TestCase(1, 2, 3, "A", 1977)]
        public void GetHashCodeTest(int id, int artistId, int genreId, string name, int year)
        {
            var item1 = new Album(id, artistId, genreId, name, year);
            var item2 = new Album(id, artistId, genreId, name, year);
            var item3 = new Album(2, 3, 4, "blah", 2015);

            Assert.AreEqual(item1.GetHashCode(), item2.GetHashCode());
            Assert.AreNotEqual(item1.GetHashCode(), item3.GetHashCode());
        }
    }

    [TestFixture]
    public class ArtistTests
    {
        [TestCase(1, "A")]
        [TestCase(1, "12345678901234567890123456789012345678901234567890")]
        public void ConstructorAndPropertiesTest(int id, string name)
        {
            var item = new Artist(id, name);
            Assert.AreEqual(id, item.Id);
            Assert.AreEqual(name, item.Name);
        }

        [TestCase(1, null, typeof(ArgumentNullException))]
        [TestCase(1, "", typeof(ArgumentNullException))]
        [TestCase(1, "123456789012345678901234567890123456789012345678901", typeof(ArgumentException))]
        public void ConstructorAndPropertiesExceptionsTest(int id, string name, Type expectedException)
        {
            Assert.That(() => new Artist(id, name), Throws.Exception.TypeOf(expectedException));
        }
    }

    [TestFixture]
    public class GenreTests
    {
        [TestCase(1, "A")]
        [TestCase(1, "12345678901234567890123456789012345678901234567890")]
        public void ConstructorAndPropertiesTest(int id, string name)
        {
            var item = new Genre(id, name);
            Assert.AreEqual(id, item.Id);
            Assert.AreEqual(name, item.Name);
        }

        [TestCase(1, null, typeof(ArgumentNullException))]
        [TestCase(1, "", typeof(ArgumentNullException))]
        [TestCase(1, "123456789012345678901234567890123456789012345678901", typeof(ArgumentException))]
        public void ConstructorAndPropertiesExceptionsTest(int id, string name, Type expectedException)
        {
            Assert.That(() => new Genre(id, name), Throws.Exception.TypeOf(expectedException));
        }
    }
}