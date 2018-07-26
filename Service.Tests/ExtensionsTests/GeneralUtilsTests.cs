using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Service.Extensions;

// ReSharper disable ExpressionIsAlwaysNull

namespace Service.Tests.ExtensionsTests
{
    [TestFixture]
    public class GeneralUtilsTests
    {
        [Test]
        public void AddUniqueTest()
        {
            List<int> target = null;
            List<int> source = null;
            try
            {
                target.AddUnique(source, true);
                Assert.Fail("Should have thrown a NullReferenceException.");
            }
            catch (Exception)
            {
                //ignore
            }

            target = new List<int>(new[]
                                   {
                                       1,
                                       2,
                                       3
                                   });

            target.AddUnique(source, true);
            CollectionAssert.AreEqual(new List<int>(new[]
                                                    {
                                                        1,
                                                        2,
                                                        3
                                                    }),
                                      target);

            source = new List<int>(new[]
                                   {
                                       4,
                                       5,
                                       6
                                   });

            target.AddUnique(source, true);
            CollectionAssert.AreEqual(new List<int>(new[]
                                                    {
                                                        1,
                                                        2,
                                                        3,
                                                        4,
                                                        5,
                                                        6
                                                    }),
                                      target);

            source = new List<int>(new[]
                                   {
                                       7,
                                       7,
                                       8
                                   });

            target.AddUnique(source, false);
            CollectionAssert.AreEqual(new List<int>(new[]
                                                    {
                                                        1,
                                                        2,
                                                        3,
                                                        4,
                                                        5,
                                                        6,
                                                        7,
                                                        7,
                                                        8
                                                    }),
                                      target);
        }

        [Test]
        public void AppendByteArraysTest()
        {
            byte[] bytes1 = null;
            byte[] bytes2 = null;
            
            Assert.Throws<NullReferenceException>(() => GeneralUtils.AppendByteArrays(bytes1, bytes2));
            Assert.Throws<ArgumentNullException>(() => GeneralUtils.AppendByteArrays(null));

            bytes1 = new byte[]
                     {
                         1,
                         2,
                         3
                     };

            bytes2 = new byte[]
                     {
                         4,
                         5,
                         6
                     };

            var bytes3 = GeneralUtils.AppendByteArrays(bytes1, bytes2);
            CollectionAssert.AreEqual(new byte[]
                                      {
                                          1,
                                          2,
                                          3,
                                          4,
                                          5,
                                          6
                                      },
                                      bytes3);
        }
        
        [Test]
        public void AppendByteArraysNullTest()
        {
            byte[] bytes1 = null;
            byte[] bytes2 = null;

            try
            {
                GeneralUtils.AppendByteArrays(bytes1, bytes2);
                Assert.Fail("Should have thrown ArgumentNullException.");
            }
            catch (Exception)
            {
                //ignore
            }

            bytes1 = new byte[]
                     {
                         1,
                         2,
                         3
                     };

            bytes2 = new byte[]
                     {
                         4,
                         5,
                         6
                     };

            var bytes3 = GeneralUtils.AppendByteArrays(bytes1, bytes2);
            CollectionAssert.AreEqual(new byte[]
                                      {
                                          1,
                                          2,
                                          3,
                                          4,
                                          5,
                                          6
                                      },
                                      bytes3);
        }

        [Test]
        public void DictEqualsTest()
        {
            Dictionary<char, int> dict1 = null;
            Dictionary<char, int> dict2 = null;

            try
            {
                dict1.DictEquals(dict2);
                Assert.Fail("Should have thrown a NullReferenceException.");
            }
            catch (Exception)
            {
                //ignore
            }

            dict1 = new Dictionary<char, int>
                    {
                        ['a'] = 1,
                        ['b'] = 2,
                        ['c'] = 3
                    };

            Assert.IsFalse(dict1.DictEquals(dict2));

            Assert.IsTrue(dict1.DictEquals(dict1));

            dict2 = new Dictionary<char, int>
                    {
                        ['a'] = 1,
                        ['b'] = 2
                    };

            Assert.IsFalse(dict1.DictEquals(dict2));

            dict2['c'] = 3;
            Assert.IsTrue(dict1.DictEquals(dict2));
        }

        [Test]
        public void IsNullOrEmptyTest()
        {
            List<int> list1 = null;
            Assert.IsTrue(list1.IsNullOrEmpty());

            int[] ints = null;
            Assert.IsTrue(ints.IsNullOrEmpty());

            Dictionary<char, int> dict1 = null;
            Assert.IsTrue(dict1.IsNullOrEmpty());

            list1 = new List<int>();
            Assert.IsTrue(list1.IsNullOrEmpty());

            ints = new int[0];
            Assert.IsTrue(ints.IsNullOrEmpty());

            dict1 = new Dictionary<char, int>();
            Assert.IsTrue(dict1.IsNullOrEmpty());

            list1.Add(1);
            Assert.IsFalse(list1.IsNullOrEmpty());

            ints = new[]
                   {
                       0
                   };

            Assert.IsFalse(ints.IsNullOrEmpty());

            dict1['a'] = 1;
            Assert.IsFalse(dict1.IsNullOrEmpty());
        }

        [Test]
        public void ListEqualsTest()
        {
            List<int> first = null;
            List<int> second = null;
            try
            {
                first.ListEquals(second, true);
                Assert.Fail("Should have thrown a NullReferenceException.");
            }
            catch (Exception)
            {
                //ignore
            }

            first = new List<int>(new[]
                                  {
                                      1,
                                      2,
                                      3
                                  });

            Assert.IsFalse(first.ListEquals(second, true));

            Assert.IsTrue(first.ListEquals(first, true));

            second = new List<int>(new[]
                                   {
                                       1,
                                       2
                                   });

            Assert.IsFalse(first.ListEquals(second, true));

            second.Add(3);
            Assert.IsTrue(first.ListEquals(second, true));

            second = new List<int>(new[]
                                   {
                                       3,
                                       2,
                                       1
                                   });

            Assert.IsTrue(first.ListEquals(second, false));
        }

        [Test]
        public void MergeAndRemoveDuplicatesTest()
        {
            List<int> list1 = null;
            List<int> list2 = null;
            try
            {
                GeneralUtils.MergeAndRemoveDuplicates(list1, list2);
                Assert.Fail("Should have thrown a NullReferenceException.");
            }
            catch (Exception)
            {
                //ignore
            }

            list1 = new List<int>();
            var list3 = GeneralUtils.MergeAndRemoveDuplicates(list1, list2).ToList();
            Assert.IsNotNull(list3);
            Assert.AreEqual(0, list3.Count);

            list2 = new List<int>();
            list3 = GeneralUtils.MergeAndRemoveDuplicates(list1, list2).ToList();
            Assert.IsNotNull(list3);
            Assert.AreEqual(0, list3.Count);

            list1.Add(1);
            list2.Add(1);
            list3 = GeneralUtils.MergeAndRemoveDuplicates(list1, list2).ToList();
            Assert.IsNotNull(list3);
            Assert.AreEqual(1, list3.Count);

            list1.Add(2);
            list2.Add(3);
            list3 = GeneralUtils.MergeAndRemoveDuplicates(list1, list2).ToList();
            Assert.IsNotNull(list3);
            CollectionAssert.AreEqual(new List<int>(new[]
                                                    {
                                                        1,
                                                        2,
                                                        3
                                                    }),
                                      list3);
        }
    }
}