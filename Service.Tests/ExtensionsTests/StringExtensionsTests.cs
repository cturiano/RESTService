using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Service.Extensions;
using Service.Models;

// ReSharper disable ExpressionIsAlwaysNull

namespace Service.Tests.ExtensionsTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public async Task DeserializeObjectAsyncTest()
        {
            string s = null;
            var obj = await s.DeserializeObjectAsync<object>();
            Assert.AreEqual(default(object), obj);

            s = string.Empty;
            obj = await s.DeserializeObjectAsync<object>();
            Assert.AreEqual(default(object), obj);

            var sa = new Album(1, 2, 3, "apiKey", 1234);
            var jsonString = JsonExtensions.SerializeObject(sa);
            var sa2 = await jsonString.DeserializeObjectAsync<Album>();
            Assert.AreEqual(sa, sa2);
        }

        [Test]
        public void EmptyIfNullTest()
        {
            string s = null;
            Assert.AreEqual(string.Empty, s.EmptyIfNull());

            s = string.Empty;
            Assert.AreEqual(string.Empty, s.EmptyIfNull());

            s = "not null";
            Assert.AreNotEqual(string.Empty, s.EmptyIfNull());
        }

        [Test]
        public void EqualsTest()
        {
            string s1 = null;
            Assert.IsTrue(StringExtensions.Equals(s1, s1));

            var s2 = string.Empty;
            Assert.IsTrue(StringExtensions.Equals(s2, s2));

            s2 = null;
            Assert.IsTrue(StringExtensions.Equals(s1, s2));

            s1 = string.Empty;
            s2 = string.Empty;
            Assert.IsTrue(StringExtensions.Equals(s1, s2));

            s1 = string.Empty;
            s2 = null;
            Assert.IsFalse(StringExtensions.Equals(s1, s2));

            s1 = null;
            s2 = string.Empty;
            Assert.IsFalse(StringExtensions.Equals(s1, s2));

            s1 = "not null or empty";
            Assert.IsFalse(StringExtensions.Equals(s1, s2));

            s2 = "not null or empty";
            Assert.IsTrue(StringExtensions.Equals(s1, s2));
        }

        [Test]
        public void FirstLetterToUpperTest()
        {
            string s = null;
            Assert.IsNull(s.FirstLetterToUpper());

            s = string.Empty;
            Assert.IsTrue(s.FirstLetterToUpper().IsNullOrEmpty());

            s = "not null or empty";
            Assert.AreEqual("Not null or empty", s.FirstLetterToUpper());

            s = "Not null or empty";
            Assert.AreEqual("Not null or empty", s.FirstLetterToUpper());
        }

        [Test]
        public async Task GetBytesAsyncTest()
        {
            string s = null;
            byte[] bytes = null;

            try
            {
                bytes = await s.GetBytesAsync();
                Assert.Fail("Should have thrown ArgumentNullException.");
            }
            catch
            {
            } //ignore

            s = string.Empty;
            bytes = await s.GetBytesAsync();
            Assert.IsTrue(bytes != null && bytes.Length == 0);

            s = "not null or empty";
            bytes = await s.GetBytesAsync();
            Assert.IsTrue(bytes != null && bytes.Length == s.Length);
        }

        [Test]
        public async Task GetRandomStringAsyncTest()
        {
            var s = await StringExtensions.GetRandomStringAsync(-13);
            Assert.IsTrue(s != null && s.Length == 0);

            for (var i = 0; i < 10000; i++)
            {
                s = await StringExtensions.GetRandomStringAsync(i);
                Assert.IsTrue(s != null && s.Length == i);
            }
        }

        [Test]
        public async Task GetSizeStringAsyncTest()
        {
            var s = await StringExtensions.GetSizeStringAsync(0);
            Assert.AreEqual(s, "0 B");

            s = await StringExtensions.GetSizeStringAsync(1);
            Assert.AreEqual(s, "1 B");

            s = await StringExtensions.GetSizeStringAsync(1024);
            Assert.AreEqual(s, "1 KB");

            s = await StringExtensions.GetSizeStringAsync(1024 * 1024);
            Assert.AreEqual(s, "1 MB");

            s = await StringExtensions.GetSizeStringAsync(1024 * 1024 * 1024);
            Assert.AreEqual(s, "1 GB");

            s = await StringExtensions.GetSizeStringAsync(Math.Pow(1024, 4));
            Assert.AreEqual(s, "1 TB");

            s = await StringExtensions.GetSizeStringAsync(Math.Pow(1024, 5));
            Assert.AreEqual(s, "1 PB");

            s = await StringExtensions.GetSizeStringAsync(Math.Pow(1024, 6));
            Assert.AreEqual(s, "1 EB");

            s = await StringExtensions.GetSizeStringAsync(Math.Pow(1024, 7));
            Assert.AreEqual(s, "1 ZB");

            s = await StringExtensions.GetSizeStringAsync(Math.Pow(1024, 8));
            Assert.AreEqual(s, "1 YB");

            s = await StringExtensions.GetSizeStringAsync(double.MaxValue);
            Assert.AreEqual(s, "Many Bytes");
        }

        [Test]
        public async Task GetStreamAsyncTest()
        {
            string s = null;
            Stream stream;

            try
            {
                stream = await s.GetStreamAsync();
                Assert.Fail("Should have thrown ArgumentNullException.");
            }
            catch
            {
            } //ignore

            s = string.Empty;
            stream = await s.GetStreamAsync();
            Assert.IsTrue(stream != null && stream.Length == 0);

            s = "not null or empty";
            stream = await s.GetStreamAsync();
            Assert.IsTrue(stream != null && stream.Length == s.Length);
        }

        [Test]
        public async Task InAsyncTest()
        {
            string s = null;
            Assert.IsFalse(await s.InAsync(false, null));
            Assert.IsFalse(await s.InAsync(true, null));

            s = string.Empty;
            Assert.IsFalse(await s.InAsync(false, null));
            Assert.IsFalse(await s.InAsync(true, null));

            s = string.Empty;
            Assert.IsFalse(await s.InAsync(false));
            Assert.IsFalse(await s.InAsync(true));

            s = string.Empty;
            Assert.IsTrue(await s.InAsync(false, string.Empty));
            Assert.IsTrue(await s.InAsync(true, string.Empty));

            s = "not null or empty";
            Assert.IsFalse(await s.InAsync(false));
            Assert.IsFalse(await s.InAsync(true));

            s = "not null or empty";
            Assert.IsFalse(await s.InAsync(false, string.Empty));
            Assert.IsFalse(await s.InAsync(true, string.Empty));

            s = "not null or empty";
            Assert.IsFalse(await s.InAsync(false, "not"));
            Assert.IsFalse(await s.InAsync(true, "Not"));

            s = "not null or empty";
            Assert.IsFalse(await s.InAsync(false, "not", "null"));
            Assert.IsFalse(await s.InAsync(true, "Not", "Null"));

            s = "not null or empty";
            Assert.IsFalse(await s.InAsync(false, "not", "null", "or"));
            Assert.IsFalse(await s.InAsync(true, "Not", "Null", "Or"));

            s = "not null or empty";
            Assert.IsFalse(await s.InAsync(false, "not", "null", "or", "empty"));
            Assert.IsFalse(await s.InAsync(true, "Not", "Null", "Or", "Empty"));

            s = "not null or empty";
            Assert.IsTrue(await s.InAsync(false, "not null or empty", "not", "null", "or", "empty"));
            Assert.IsTrue(await s.InAsync(true, "Not nUll or empty", "Not", "nuLl", "oR", "emPty"));

            s = "not null or empty";
            Assert.IsTrue(await s.InAsync(false, "not null or empty not null or empty"));
            Assert.IsTrue(await s.InAsync(true, "noT nulL Or eMpty nOt nUll oR empTy"));
        }

        [Test]
        public void IsAllLowerTest()
        {
            string s = null;
            Assert.IsFalse(s.IsAllLower());

            s = string.Empty;
            Assert.IsFalse(s.IsAllLower());

            s = "Not nUll or empty";
            Assert.IsFalse(s.IsAllLower());

            s = "NOT NULL OR EMPTY";
            Assert.IsFalse(s.IsAllLower());

            s = "not null or empty";
            Assert.IsTrue(s.IsAllLower());
        }

        [Test]
        public void IsAllUpperTest()
        {
            string s = null;
            Assert.IsFalse(s.IsAllUpper());

            s = string.Empty;
            Assert.IsFalse(s.IsAllUpper());

            s = "Not nUll or empty";
            Assert.IsFalse(s.IsAllUpper());

            s = "NOT NULL OR EMPTY";
            Assert.IsTrue(s.IsAllUpper());

            s = "not null or empty";
            Assert.IsFalse(s.IsAllUpper());
        }

        [Test]
        public void IsNullOrEmptyTest()
        {
            string s = null;
            Assert.IsTrue(s.IsNullOrEmpty());

            s = string.Empty;
            Assert.IsTrue(s.IsNullOrEmpty());

            s = "Not nUll or empty";
            Assert.IsFalse(s.IsNullOrEmpty());
        }

        [Test]
        public void NullIfEmptyTest()
        {
            string s = null;
            Assert.IsNull(s.NullIfEmpty());

            s = string.Empty;
            Assert.IsNull(s.NullIfEmpty());

            s = "not null or empty";
            Assert.IsNotNull(s.NullIfEmpty());
        }

        [Test]
        public async Task ReplaceAllAsyncTest()
        {
            string s = null;
            try
            {
                await s.ReplaceAllAsync("", "");
                Assert.Fail("Should have thrown ArgumentNullException.");
            }
            catch
            {
                //ignore
            }

            s = string.Empty;
            Assert.AreEqual("", await s.ReplaceAllAsync("", ""));

            s = string.Empty;
            Assert.AreEqual(".", await s.ReplaceAllAsync("", "."));

            s = "not null or empty";
            Assert.AreEqual("not null . empty", await s.ReplaceAllAsync("or", "."));

            s = "not null \n empty\n";
            Assert.AreEqual("not null . empty.", await s.ReplaceAllAsync("\n", "."));
        }

        [Test]
        public async Task ReplaceLastOccurrenceAsyncTest()
        {
            string s = null;
            try
            {
                await s.ReplaceLastOccurrenceAsync("", "");
                Assert.Fail("Should have thrown ArgumentNullException.");
            }
            catch
            {
                //ignore
            }

            s = string.Empty;
            Assert.AreEqual("", await s.ReplaceLastOccurrenceAsync("", ""));

            s = string.Empty;
            Assert.AreEqual(".", await s.ReplaceLastOccurrenceAsync("", "."));

            s = "not null or empty";
            Assert.AreEqual("not null . empty", await s.ReplaceLastOccurrenceAsync("or", "."));

            s = "not null \n empty\n";
            Assert.AreEqual("not null \n empty.", await s.ReplaceLastOccurrenceAsync("\n", "."));
        }

        [Test]
        public async Task ToLowerCaseAsyncTest()
        {
            string s = null;
            Assert.AreEqual(null, await s.ToLowerCaseAsync());

            s = string.Empty;
            Assert.AreEqual(string.Empty, await s.ToLowerCaseAsync());

            s = "Not nUll or empty";
            Assert.AreEqual("not null or empty", await s.ToLowerCaseAsync());

            s = "NOT NULL OR EMPTY";
            Assert.AreEqual("not null or empty", await s.ToLowerCaseAsync());

            s = "not null or empty";
            Assert.AreEqual("not null or empty", await s.ToLowerCaseAsync());
        }
    }
}