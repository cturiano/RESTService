using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Service.Extensions;
using Service.Models;

namespace Service.Tests.ExtensionsTests
{
    [TestFixture]
    public class JsonExtensionsTests
    {
        private const string IdKey = "Id";
        private const string ArtistIdKey = "ArtistId";
        private const string GenreIdKey = "GenreId";
        private const string NameKey = "Name";
        private const string YearKey = "Year";

        private const int Id = 1;
        private const int ArtistId = 2;
        private const int GenreId = 3;
        private const int Year = 1235;

        [Test]
        public async Task ConvertJObjectToDictionaryAsyncTest()
        {
            var dict = await JObject.FromObject(HelperObjectFactory.MakeAlbum(Id, ArtistId, GenreId, NameKey, Year)).ConvertJObjectToDictionaryAsync();
            Assert.IsTrue(dict.ContainsKey(IdKey));
            Assert.IsTrue(dict.ContainsKey(ArtistIdKey));
            Assert.IsTrue(dict.ContainsKey(GenreIdKey));
            Assert.IsTrue(dict.ContainsKey(NameKey));
            Assert.IsTrue(dict.ContainsKey(YearKey));

            Assert.IsTrue(dict[IdKey] == Id.ToString());
            Assert.IsTrue(dict[ArtistIdKey] == ArtistId.ToString());
            Assert.IsTrue(dict[GenreIdKey] == GenreId.ToString());
            Assert.IsTrue(dict[NameKey] == NameKey);
            Assert.IsTrue(dict[YearKey] == Year.ToString());
        }
        
        [Test]
        public async Task ConvertJObjectToDictionaryAsyncNullTest()
        {
            var dict = await ((JObject)null).ConvertJObjectToDictionaryAsync();
            Assert.IsNull(dict);
        }

        [Test]
        public async Task SerializeObjectTest()
        {
            var a1 = HelperObjectFactory.MakeAlbum(Id, ArtistId, GenreId, NameKey, Year);
            var s = JsonExtensions.SerializeObject(a1);
            Assert.IsNotNull(s);

            var a2 = await s.DeserializeObjectAsync<Album>();
            Assert.AreEqual(a1, a2);

            var obj = StringExtensions.GetRandomStringAsync(256).Result;
            s = JsonExtensions.SerializeObject(obj);
            Assert.AreEqual('"' + obj + '"', s);

            var random = new Random().Next();
            s = JsonExtensions.SerializeObject(random);
            Assert.AreEqual(random.ToString(), s);
        }
    }
}