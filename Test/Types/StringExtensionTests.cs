﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Visyn.Public.Text;

namespace Visyn.Public.Test.Types
{
    [TestFixture]
    public class StringExtensionTests
    {
        private const string TEST_STRING_LETTERS_AND_NUMBERS = "This is test 10 string with letters and numbers";
        private const string COMMA_DELIMITED_INTS = "0,1,2,3,4,5,6,7,8,9,10,11,12";
        private const string COMMA_DELIMITED_INTS_WITH_SPACES = " 0 ,1, 2,3  ,   4,5,  6,7, 8 ,9,10 ,11  , 12";
        private const string COMMA_DELIMITED_DOUBLES_WITH_SPACES = " 0 ,1.1, 2.2,3.3  ,   4.4,5.5,  6.6,7.7, 8.8 ,9.9,11.0 ,12.1  , 13.2";

        [Test]
        public void ToByteArrayTest()
        {
            var bytes = TEST_STRING_LETTERS_AND_NUMBERS.ToByteArray();

            Assert.NotNull(bytes);
            Assert.AreEqual(TEST_STRING_LETTERS_AND_NUMBERS.Length, bytes.Length);

            for(var i=0;i<TEST_STRING_LETTERS_AND_NUMBERS.Length;i++)
            {
                Assert.AreEqual((byte)TEST_STRING_LETTERS_AND_NUMBERS[i],bytes[i]);
            }
        }

        [Test]
        public void FromBytesTest()
        {
            var bytes = TEST_STRING_LETTERS_AND_NUMBERS.ToByteArray();

            Assert.AreEqual(TEST_STRING_LETTERS_AND_NUMBERS.Length, bytes.Length);

            for (var i = 0; i < TEST_STRING_LETTERS_AND_NUMBERS.Length; i++)
            {
                Assert.AreEqual((byte)TEST_STRING_LETTERS_AND_NUMBERS[i], bytes[i]);
            }

            var recreatedString = StringExtensions.FromBytes(bytes);

            Assert.AreEqual(TEST_STRING_LETTERS_AND_NUMBERS.Length,recreatedString.Length);
            Assert.AreEqual(TEST_STRING_LETTERS_AND_NUMBERS,bytes);
        }

#region ParseDelimitedString Tests
        [Test]
        public void ParseDelimitedStringIntegersTest()
        {
            {
                var ints = COMMA_DELIMITED_INTS.ParseDelimitedString(new[] {','}, int.Parse, StringSplitOptions.RemoveEmptyEntries);

                Assert.NotNull(ints);
                Assert.AreEqual(13, ints.Length);
                for (int i = 0; i < ints.Length; i++)
                {
                    Assert.AreEqual(i, ints[i]);
                }
            }
            {
                var ints = COMMA_DELIMITED_INTS_WITH_SPACES.ParseDelimitedString(new[] {','}, int.Parse,
                    StringSplitOptions.RemoveEmptyEntries);

                Assert.NotNull(ints);
                Assert.AreEqual(13, ints.Length);
                for (int i = 0; i < ints.Length; i++)
                {
                    Assert.AreEqual(i, ints[i]);
                }
            }
        }

        [Test]
        public void ParseDelimitedDoublesAsIntsTest()
        {
            Assert.Throws<System.FormatException>(() =>
            {
                COMMA_DELIMITED_DOUBLES_WITH_SPACES.ParseDelimitedString(new[] {','}, int.Parse,
                    StringSplitOptions.RemoveEmptyEntries);
            });
        }

        [Test]
        public void ParseDelimitedStringDoublesTest()
        {
            {
                var doubles = COMMA_DELIMITED_INTS.ParseDelimitedString(new[] { ',' }, int.Parse, StringSplitOptions.RemoveEmptyEntries);

                Assert.NotNull(doubles);
                Assert.AreEqual(13, doubles.Length);
                for (int i = 0; i < doubles.Length; i++)
                {
                    Assert.AreEqual(i, doubles[i]);
                }
            }
            {
                var doubles = COMMA_DELIMITED_INTS_WITH_SPACES.ParseDelimitedString(new[] { ',' }, int.Parse,
                    StringSplitOptions.RemoveEmptyEntries);

                Assert.NotNull(doubles);
                Assert.AreEqual(13, doubles.Length);
                for (int i = 0; i < doubles.Length; i++)
                {
                    Assert.AreEqual(i, doubles[i]);
                }
            }
        }

        [Test]
        public void ParseDelimitedDoublesTest()
        {
            var doubles = COMMA_DELIMITED_DOUBLES_WITH_SPACES.ParseDelimitedString(new[] { ',' }, double.Parse,
                StringSplitOptions.RemoveEmptyEntries);

            Assert.NotNull(doubles);
            Assert.AreEqual(13, doubles.Length);
            for (int i = 0; i < doubles.Length; i++)
            {
                Assert.AreEqual(i*1.1, doubles[i], 0.01);
            }
        }
        #endregion ParseDelimitedString Tests

        #region TryParseDelimitedString Tests
        [Test]
        public void TryParseDelimitedStringIntegersTest()
        {
            {
                int successCount;
                List<int> ints;
                var success = COMMA_DELIMITED_INTS.TryParseDelimitedString<int>(new[] { ',' }, int.TryParse, StringSplitOptions.RemoveEmptyEntries, out ints, out successCount);

                Assert.True(success);
                Assert.NotNull(ints);
                Assert.AreEqual(13, ints.Count);
                Assert.AreEqual(13, successCount);
                for (int i = 0; i < ints.Count; i++)
                {
                    Assert.AreEqual(i, ints[i]);
                }
            }
            {
                int successCount;
                List<int> ints;
                var success = COMMA_DELIMITED_INTS_WITH_SPACES.TryParseDelimitedString<int>(new[] { ',' }, int.TryParse,
                    StringSplitOptions.RemoveEmptyEntries, out ints, out successCount);

                Assert.True(success);
                Assert.NotNull(ints);
                Assert.AreEqual(13, ints.Count);
                Assert.AreEqual(13, successCount);
                for (int i = 0; i < ints.Count; i++)
                {
                    Assert.AreEqual(i, ints[i]);
                }
            }
        }

        [Test]
        public void TryParseDelimitedDoublesAsIntsTest()
        {
            int successCount;
            List<int> ints;
            var success = COMMA_DELIMITED_DOUBLES_WITH_SPACES.TryParseDelimitedString<int>(new[] { ',' }, int.TryParse,
                    StringSplitOptions.None, out ints, out successCount);

            Assert.False(success);
            Assert.NotNull(ints);
            Assert.AreEqual(13, ints.Count);
            Assert.AreEqual(1, successCount);
            for (int i = 0; i < ints.Count; i++)
            {
                if(i == 0) Assert.AreEqual(i, ints[i]);
                else Assert.AreEqual(0, ints[i]);
            }
        }

        [Test]
        public void TryParseDelimitedDoublesAsIntsTestRemoveEmpty()
        {
            int successCount;
            List<int> ints;
            var success = COMMA_DELIMITED_DOUBLES_WITH_SPACES.TryParseDelimitedString<int>(new[] { ',' }, int.TryParse,
                    StringSplitOptions.RemoveEmptyEntries, out ints, out successCount);

            Assert.False(success);
            Assert.NotNull(ints);
            Assert.AreEqual(1, ints.Count);
            Assert.AreEqual(1, successCount);
            for (int i = 0; i < ints.Count; i++)
            {
                if (i == 0) Assert.AreEqual(i, ints[i]);
                else Assert.AreEqual(0, ints[i]);
            }
        }

        [Test]
        public void TryParseDelimitedStringDoublesTest()
        {
            {
                int successCount;
                List<double> doubles;
                var success = COMMA_DELIMITED_INTS.TryParseDelimitedString(new[] { ',' }, double.TryParse, StringSplitOptions.RemoveEmptyEntries, out doubles, out successCount);

                Assert.True(success);
                Assert.NotNull(doubles);
                Assert.AreEqual(13, doubles.Count);
                Assert.AreEqual(13, successCount);
                for (int i = 0; i < doubles.Count; i++)
                {
                    Assert.AreEqual(i, doubles[i]);
                }
            }
            {
                int successCount;
                List<double> doubles;
                var success = COMMA_DELIMITED_INTS_WITH_SPACES.TryParseDelimitedString(new[] { ',' }, double.TryParse,
                    StringSplitOptions.RemoveEmptyEntries,out doubles, out successCount);

                Assert.True(success);
                Assert.NotNull(doubles);
                Assert.AreEqual(13, doubles.Count);
                Assert.AreEqual(13, successCount);
                for (int i = 0; i < doubles.Count; i++)
                {
                    Assert.AreEqual(i, doubles[i]);
                }
            }
        }

        [Test]
        public void TryParseDelimitedDoublesTest()
        {
            int successCount;
            List<double> doubles;
            var success = COMMA_DELIMITED_DOUBLES_WITH_SPACES.TryParseDelimitedString(new[] { ',' }, double.TryParse,
                    StringSplitOptions.RemoveEmptyEntries, out doubles, out successCount);

            Assert.True(success);
            Assert.NotNull(doubles);
            Assert.AreEqual(13, doubles.Count);
            Assert.AreEqual(13, successCount);
            for (int i = 0; i < doubles.Count; i++)
            {
                Assert.AreEqual(i * 1.1, doubles[i], 0.01);
            }
        }

        [Test]
        public void TryParseNullTest()
        {
            int successCount;
            List<double> doubles;
            var success = StringExtensions.TryParseDelimitedString(null,new[] { ',' }, double.TryParse,
                    StringSplitOptions.RemoveEmptyEntries, out doubles, out successCount);

            Assert.False(success);
            Assert.NotNull(doubles);
            Assert.AreEqual(0, doubles.Count);
            Assert.AreEqual(0, successCount);
        }

        [Test]
        public void TryParseEmptyTest()
        {
            int successCount;
            List<double> doubles;
            var success = string.Empty.TryParseDelimitedString(new[] { ',' }, double.TryParse,
                    StringSplitOptions.RemoveEmptyEntries, out doubles, out successCount);

            Assert.False(success);
            Assert.NotNull(doubles);
            Assert.AreEqual(0, doubles.Count);
            Assert.AreEqual(0, successCount);
        }

        [Test]
        public void TryParseNullDelegateTest()
        {
            int successCount;
            List<double> doubles;
            Assert.Throws<NullReferenceException>(() =>
            {
                "1,2,3".TryParseDelimitedString(new[] {','}, null,
                    StringSplitOptions.RemoveEmptyEntries, out doubles, out successCount);
            });
        }
        #endregion TryParseDelimitedString Tests
    }
}
