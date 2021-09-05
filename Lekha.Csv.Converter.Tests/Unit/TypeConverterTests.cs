using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Lekha.Csv.Converter.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lekha.Csv.Converter.Tests
{

    public class TypeConverterTests
    {
        [Theory]
        [InlineData("1", "aaa", DataType.String, false, 10, "aaa", false)]
        [InlineData("2", "", DataType.String, false, 10, "", false)]
        [InlineData("3", "333", DataType.String, false, 10, "333", false)]
        [InlineData("4", null, DataType.String, false, 10, null, false)]
        [InlineData("5", "   ", DataType.String, true, 10, "   ", false)]
        [InlineData("6", "abcdefj", DataType.String, true, 3, null, true)]
        public void ConvertFromStringToStringTests(string testDescription, string valueToTest, string dataType, bool allowEmptyField, int maxLength, string expected, bool throwsException)
        {
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField,
                AllowedMaximumLength = maxLength
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }

        [Theory]
        [InlineData("1", "aaa", DataType.String, false, 10, "aaa", false)]
        [InlineData("2", "", DataType.String, false, 10, "", false)]
        [InlineData("3", "333", DataType.String, false, 10, "333", false)]
        [InlineData("4", null, DataType.String, false, 10, null, false)]
        [InlineData("5", "   ", DataType.String, true, 10, "   ", false)]
        [InlineData("6", "abcdefj", DataType.String, true, 3, null, true)]
        public void ConvertToStringFromStringTests(string testDescription, string valueToTest, string dataType, bool allowEmptyField, int maxLength, string expected, bool throwsException)
        {
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField,
                AllowedMaximumLength = maxLength
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest,
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }

        [Theory]
        [InlineData("1", "0", DataType.SignedNumber, false, 0, false)]
        [InlineData("2", "-111", DataType.SignedNumber, false, -111, false)]
        [InlineData("3", "88788", DataType.SignedNumber, false, 88788, false)]
        [InlineData("4", "2147483647", DataType.SignedNumber, false, 2147483647, false)]
        [InlineData("5", "-2147483647", DataType.SignedNumber, false, -2147483647, false)]
        [InlineData("6", "9223372036854775807", DataType.SignedNumber, false, long.MaxValue, false)]
        [InlineData("7", "-9223372036854775808", DataType.SignedNumber, false, long.MinValue, false)]
        [InlineData("8", "   ", DataType.SignedNumber, true, null, false)]
        [InlineData("9", "   ", DataType.SignedNumber, false, null, true)]
        [InlineData("10", "afsdf", DataType.SignedNumber, true, null, true)]
        [InlineData("11", "9223372036854775808", DataType.SignedNumber, false, null, true)]
        [InlineData("12", "-9223372036854775809", DataType.SignedNumber, false, null, true)]
        public void ConvertFromStringToSignedNumberTests(string testDescription, string valueToTest, string dataType, bool allowEmptyField, object expected, bool throwsException)
        {
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", 0, DataType.SignedNumber, false, "0", false)]
        [InlineData("2", -111, DataType.SignedNumber, false, "-111", false)]
        [InlineData("3", 88788, DataType.SignedNumber, false, "88788", false)]
        [InlineData("4", 2147483647, DataType.SignedNumber, false, "2147483647", false)]
        [InlineData("5", -2147483647, DataType.SignedNumber, false, "-2147483647", false)]
        [InlineData("6", long.MaxValue, DataType.SignedNumber, false, "9223372036854775807", false)]
        [InlineData("7", long.MinValue, DataType.SignedNumber, false, "-9223372036854775808", false)]
        [InlineData("8", null, DataType.SignedNumber, true, null, false)]
        public void ConvertToStringFromSignedNumberTests(string testDescription, object valueToTest, string dataType, bool allowEmptyField, string expected, bool throwsException)
        {
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest,
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "0", DataType.UnsignedNumber, false, 0U, false)]
        [InlineData("2", "-111", DataType.UnsignedNumber, false, null, true)]
        [InlineData("3", "88788", DataType.UnsignedNumber, false, 88788U, false)]
        [InlineData("4", "2147483647", DataType.UnsignedNumber, false, 2147483647U, false)]
        [InlineData("5", "-2147483647", DataType.UnsignedNumber, false, null, true)]
        [InlineData("6", "9223372036854775807", DataType.UnsignedNumber, false, 9223372036854775807U, false)]
        [InlineData("7", "-9223372036854775808", DataType.UnsignedNumber, false, null, true)]
        [InlineData("8", "   ", DataType.UnsignedNumber, true, null, false)]
        [InlineData("9", "   ", DataType.UnsignedNumber, false, null, true)]
        [InlineData("10", "afsdf", DataType.UnsignedNumber, true, null, true)]
        [InlineData("11", "9223372036854775808", DataType.UnsignedNumber, false, 9223372036854775808U, false)]
        [InlineData("12", "-9223372036854775809", DataType.UnsignedNumber, false, null, true)]
        [InlineData("13", "18446744073709551615", DataType.UnsignedNumber, false, ulong.MaxValue, false)]  // max ulong
        [InlineData("14", "18446744073709551616", DataType.UnsignedNumber, false, ulong.MaxValue, true)]   // max ulong plus 1
        public void ConvertFromStringToUnsignedNumberTests(string testDescription, string valueToTest, string dataType, bool allowEmptyField, object expected, bool throwsException)
        {
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", 0U, DataType.UnsignedNumber, false, "0", false)]
        [InlineData("3", 88788U, DataType.UnsignedNumber, false, "88788", false)]
        [InlineData("4", 2147483647U, DataType.UnsignedNumber, false, "2147483647", false)]
        [InlineData("6", ulong.MaxValue, DataType.UnsignedNumber, false, "18446744073709551615", false)]
        [InlineData("8", null, DataType.UnsignedNumber, true, null, false)]
        public void ConvertToStringFromUnsignedNumberTests(string testDescription, object valueToTest, string dataType, bool allowEmptyField, string expected, bool throwsException)
        {
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest,
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", "0", DataType.Decimal, false, false)]
        [InlineData("2", "2", "-111", DataType.Decimal, false, false)]
        [InlineData("3", "3", "88788", DataType.Decimal, false, false)]
        [InlineData("4", "4", "2147483647", DataType.Decimal, false, false)]
        [InlineData("5", "5", "-2147483647", DataType.Decimal, false, false)]
        [InlineData("6", "6", "9223372036854775807", DataType.Decimal, false, false)]
        [InlineData("7", "7", "-9223372036854775808", DataType.Decimal, false, false)]
        [InlineData("8", "8", "9223372036854775807.89", DataType.Decimal, false, false)]
        [InlineData("9", "9", "-9223372036854775808.89", DataType.Decimal, false, false)]
        [InlineData("10", "10", "   ", DataType.Decimal, true, false)]
        [InlineData("11", "11", "   ", DataType.Decimal, false, true)]
        [InlineData("12", "12", "afsdf", DataType.Decimal, true, true)]
        [InlineData("13", "13", "9223372036854775808", DataType.Decimal, false, false)]
        [InlineData("14", "14", "-9223372036854775809", DataType.Decimal, false, false)]
        [InlineData("15", "15", "79228162514264337593543950335", DataType.Decimal, false, false)]
        [InlineData("16", "16", "-79228162514264337593543950335", DataType.Decimal, false, false)]
        [InlineData("17", "17", "79228162514264337593543950336", DataType.Decimal, false, true)]
        [InlineData("18", "18", "-79228162514264337593543950336", DataType.Decimal, false, true)]
        [InlineData("19", "19", "792281625.123456789012345678901234567890123456789", DataType.Decimal, false, false)]
        [InlineData("20", "20", "-792281625.123456789012345678901234567890123456789", DataType.Decimal, false, false)]
        [InlineData("21", "21", "792281625.199", DataType.Decimal, false, false)]
        [InlineData("22", "22", "-792281625.199", DataType.Decimal, false, false)]
        [InlineData("23", "23", "-0.9999999999999999999999999999", DataType.Decimal, false, false)]
        [InlineData("24", "24", "0.9999999999999999999999999999", DataType.Decimal, false, false)]
        [InlineData("25", "25", "-0.99999999999999999999999999999", DataType.Decimal, false,  false)]
        [InlineData("26", "26", "0.99999999999999999999999999999", DataType.Decimal, false, false)]
        public void ConvertFromStringToDecimalTests(string testDescription, string expectedResultKey, string valueToTest, string dataType, bool allowEmptyField, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var expectedResult = new Dictionary<string, object>
            {
                {  "1",  0.00m },
                {  "2",  -111m },
                {  "3",  88788m },
                {  "4",  2147483647m },
                {  "5",  -2147483647m },
                {  "6",  9223372036854775807m },
                {  "7",  -9223372036854775808m },
                {  "8",  9223372036854775807.89m },
                {  "9",  -9223372036854775808.89m },
                {  "10", null },
                {  "11", null },
                {  "12",  null },
                {  "13",  9223372036854775808m },
                {  "14",  -9223372036854775809m },
                {  "15",  79228162514264337593543950335m },
                {  "16",  -79228162514264337593543950335M },
                {  "17",  null },
                {  "18",  null },
                {  "19",  792281625.123456789012345678901234567890123456789M },
                {  "20",  -792281625.123456789012345678901234567890123456789M },
                {  "21",  792281625.199M },
                {  "22",  -792281625.199M },
                {  "23",  -0.9999999999999999999999999999M },
                {  "24",  0.9999999999999999999999999999M },
                {  "25",  -1.0M },
                {  "26",  1.0M },
            };
            //NumberFormatInfo setPrecision = new NumberFormatInfo();

            //setPrecision.NumberDecimalDigits = 2;
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult[expectedResultKey], $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", DataType.Decimal, false, "0", false)]
        [InlineData("2", "2", DataType.Decimal, false, "88788", false)]
        [InlineData("3", "3", DataType.Decimal, false, "2147483647", false)]
        [InlineData("4", "4", DataType.Decimal, false, "9223372036854775807", false)]
        [InlineData("5", "5", DataType.Decimal, false, "-9223372036854775807", false)]
        [InlineData("6", "6", DataType.Decimal, true, null, false)]
        [InlineData("7", "7", DataType.Decimal, false, "-9223372036854775807.747446", false)]
        [InlineData("8", "8", DataType.Decimal, false, "79228162514264337593543950335", false)]
        [InlineData("9", "9", DataType.Decimal, false, "-79228162514264337593543950335", false)]
        [InlineData("10", "10", DataType.Decimal, false, "79228162514264337593543950336", true)]
        [InlineData("11", "11", DataType.Decimal, false, "-79228162514264337593543950336", true)]
        [InlineData("12", "12", DataType.Decimal, false, "79228162514264337593543950335", false)]
        [InlineData("13", "13", DataType.Decimal, false, "-79228162514264337593543950335", false)]
        [InlineData("14", "14", DataType.Decimal, false, "792281625.199", false)]
        [InlineData("15", "15", DataType.Decimal, false, "-792281625.199", false)]
        [InlineData("16", "16", DataType.Decimal, false, "-0.79228162514264337593543999", false)]
        [InlineData("17", "17", DataType.Decimal, false, "0.79228162514264337593543999", false)]
        [InlineData("18", "18", DataType.Decimal, false, "-0.9999999999999999999999999999", false)]
        [InlineData("19", "19", DataType.Decimal, false, "0.9999999999999999999999999999", false)]
        [InlineData("20", "20", DataType.Decimal, false, "-1.0000000000000000000000000000", false)]
        [InlineData("21", "21", DataType.Decimal, false, "1.0000000000000000000000000000", false)]
        public void ConvertToStringFromDecimalTests(string testDescription, string inputKey, string dataType, bool allowEmptyField, string expected, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with inputKey
            var valueToTest = new Dictionary<string, object>
            {
                {  "1",  0m },
                {  "2",  88788m },
                {  "3",  2147483647m },
                {  "4",  9223372036854775807m },
                {  "5",  -9223372036854775807m },
                {  "6", null },
                {  "7",  -9223372036854775807.747446m },
                {  "8",  79228162514264337593543950335m },
                {  "9",  -79228162514264337593543950335M },
                {  "10",  null },
                {  "11",  null },
                {  "12",  decimal.MaxValue},
                {  "13",  decimal.MinValue },
                {  "14",  792281625.199M },
                {  "15",  -792281625.199M },
                {  "16",  -0.79228162514264337593543999M },
                {  "17",  0.79228162514264337593543999M },
                {  "18",  -0.9999999999999999999999999999M }, // 29 digit precision max supported by a decimal
                {  "19",  0.9999999999999999999999999999M },  // 29 digit precision max supported by a decimal
                {  "20",  -0.99999999999999999999999999999M }, // 30 digit precision 1 more than max supported by a decimal - result: rounded-off -1.0
                {  "21",  0.99999999999999999999999999999M },  // 30 digit precision 1 more than max supported by a decima - result: rounded-off to 1.0
            };
            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = DefaultFormat.Date,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest[inputKey],
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expected, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", "0", DefaultFormat.Date, DataType.Date, false, true)]
        [InlineData("2", "2", "aaaaa", DefaultFormat.Date, DataType.Date, false, true)]
        [InlineData("3", "3", "2021/10/21", DefaultFormat.Date, DataType.Date, false, false)]
        [InlineData("4", "4", "2021/10/21", "aaa", DataType.Date, false, true)]
        [InlineData("5", "5", " ", DefaultFormat.Date, DataType.Date, true, false)]
        [InlineData("6", "6", null, DefaultFormat.Date, DataType.Date, true, false)]
        [InlineData("7", "7", "2021/21/10", "yyyy/dd/MM", DataType.Date, false, false)]
        [InlineData("8", "8", "21/10/2021", "dd/MM/yyyy", DataType.Date, false, false)]
        [InlineData("9", "9", "9999/21/10", "yyyy/dd/MM", DataType.Date, false, false)]
        [InlineData("10", "10", "2021/21", "yyyy/dd", DataType.Date, false, false)]
        [InlineData("11", "11", "2021/21/10", "dd/MM/yyyy", DataType.Date, false, false)]
        public void ConvertFromStringToDateTests(string testDescription, string expectedResultKey, string valueToTest, string dateFormat, string dataType, bool allowEmptyField, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var expectedResult = new Dictionary<string, DateTimeOffset?>
            {
                {  "1",  null },
                {  "2",  null },
                {  "3",  new DateTimeOffset(2021, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "4",  null },
                {  "5",  null },
                {  "6",  null },
                {  "7",  new DateTimeOffset(2021, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "8",  new DateTimeOffset(2021, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "9",  new DateTimeOffset(9999, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "10", new DateTimeOffset(2021, 1, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "11", null },
            };

            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = dateFormat,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult[expectedResultKey], $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", DefaultFormat.Date, DataType.Date, false, null, true)]
        [InlineData("3", "3", DefaultFormat.Date, DataType.Date, false, "2021/10/21", false)]
        [InlineData("7", "7", "yyyy/dd/MM", DataType.Date, false, "2021/21/10", false)]
        [InlineData("8", "8", "dd/MM/yyyy", DataType.Date, false, "21/10/2021", false)]
        [InlineData("9", "9", "yyyy/dd/MM", DataType.Date, false, "9999/21/10", false)]
        [InlineData("10", "10", "yyyy/dd", DataType.Date, false, "2021/21", false)]
        [InlineData("11", "11", "yyyy/dd", DataType.Date, false, "9999/31", false)]
        [InlineData("12", "12", "yyyy/dd/MM", DataType.Date, false, "9999/31/12", false)]
        [InlineData("13", "13", "yyyy/dd", DataType.Date, false, "0001/01", false)]
        [InlineData("14", "14", "yyyy/dd/MM", DataType.Date, false, "0001/01/01", false)]
        public void ConvertToStringFromDateTests(string testDescription, string valueToTestKey, string dateFormat, string dataType, bool allowEmptyField, string expectedResult, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var valueToTest = new Dictionary<string, DateTimeOffset?>
            {
                {  "1",  null },
                {  "3",  new DateTimeOffset(2021, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "7",  new DateTimeOffset(2021, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "8",  new DateTimeOffset(2021, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "9",  new DateTimeOffset(9999, 10, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "10", new DateTimeOffset(2021, 1, 21, 0, 0, 0, TimeSpan.Zero) },
                {  "11", new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero) },
                {  "12", new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero) },
                {  "13", new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero) },
                {  "14", new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero) },
            };

            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = dateFormat,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest[valueToTestKey],
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", "0", DefaultFormat.Time, DataType.Time, false, true)]
        [InlineData("2", "2", "aaaaa", DefaultFormat.Time, DataType.Time, false, true)]
        [InlineData("3", "3", "13:19", DefaultFormat.Time, DataType.Time, false, false)]
        [InlineData("4", "4", "13:19", "aaa", DataType.Time, false, true)]
        [InlineData("5", "5", " ", DefaultFormat.Time, DataType.Time, true, false)]
        [InlineData("6", "6", null, DefaultFormat.Time, DataType.Time, true, false)]
        [InlineData("7", "7", "19:13", "mm:HH", DataType.Time, false, true)]
        [InlineData("8", "8", "24:00", "HH:mm", DataType.Time, false, true)]
        [InlineData("9", "9", "19:13", "hh\\:mm", DataType.Time, false, false)]
        [InlineData("10", "10", "19:13", "mm\\:hh", DataType.Time, false, false)]
        [InlineData("11", "11", "24:00", "HH\\:mm", DataType.Time, false, true)]
        [InlineData("12", "12", "24:10", "HH\\:mm", DataType.Time, false, true)]
        [InlineData("13", "13", "-4:10", "HH\\:mm", DataType.Time, false, true)]
        public void ConvertFromStringToTimeTests(string testDescription, string expectedResultKey, string valueToTest, string dateFormat, string dataType, bool allowEmptyField, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var expectedResult = new Dictionary<string, TimeSpan?>
            {
                {  "1",  new TimeSpan(13, 19, 0) },
                {  "2",  new TimeSpan(13, 19, 0) },
                {  "3",  new TimeSpan(13, 19, 0) },
                {  "4",  new TimeSpan(13, 19, 0) },
                {  "5",  null },
                {  "6",  null },
                {  "7",  new TimeSpan(13, 19, 0) },
                {  "8",  new TimeSpan(13, 19, 0) },
                {  "9",  new TimeSpan(19, 13, 0) },
                {  "10",  new TimeSpan(13, 19, 0) },
                {  "11",  new TimeSpan(13, 19, 0) },
                {  "12",  new TimeSpan(13, 19, 0) },
                {  "13",  new TimeSpan(13, 19, 0) },
            };

            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = dateFormat,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult[expectedResultKey], $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", DefaultFormat.Time, DataType.Time, false, null, true)]
        [InlineData("3", "3", DefaultFormat.Time, DataType.Time, false, "13:19", false)]
        [InlineData("4", "4", "aaa", DataType.Time, false, "13:19", true)]
        [InlineData("7", "7", "mm:HH", DataType.Time, false, "19:13", true)]
        [InlineData("8", "8", "HH:mm", DataType.Time, false, "24:00", true)]
        [InlineData("9", "9", "hh\\:mm", DataType.Time, false, "19:13", false)]
        [InlineData("10", "10", "mm\\:hh", DataType.Time, false, "19:13", false)]
        public void ConvertToStringFromTimeTests(string testDescription, string valueToTestKey, string dateFormat, string dataType, bool allowEmptyField, string expectedResult, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var valueToTest = new Dictionary<string, TimeSpan?>
            {
                {  "1",  null },
                {  "3",  new TimeSpan(13, 19, 0) },
                {  "4",  null },
                {  "7",  null },
                {  "8",  null },
                {  "9",  new TimeSpan(19, 13, 0) },
                {  "10",  new TimeSpan(13, 19, 0) }
            };

            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = dateFormat,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest[valueToTestKey],
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult, $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", "0", DefaultFormat.DateTime, DataType.DateTime, false, true)]
        [InlineData("2", "2", "aaaaa", DefaultFormat.DateTime, DataType.DateTime, false, true)]
        [InlineData("3", "3", "2021/10/21 13:59", DefaultFormat.DateTime, DataType.DateTime, false, false)]
        [InlineData("4", "4", "2021/10/21 13:59", "aaa", DataType.DateTime, false, true)]
        [InlineData("5", "5", " ", DefaultFormat.DateTime, DataType.DateTime, true, false)]
        [InlineData("6", "6", null, DefaultFormat.DateTime, DataType.DateTime, true, false)]
        [InlineData("7", "7", "2021/21/10 13:59", "yyyy/dd/MM HH:mm", DataType.DateTime, false, false)]
        [InlineData("8", "8", "21/10/2021 13:59", "dd/MM/yyyy HH:mm", DataType.DateTime, false, false)]
        [InlineData("9", "9", "9999/21/10 13:59", "yyyy/dd/MM HH:mm", DataType.DateTime, false, false)]
        [InlineData("10", "10", "2021/21 13:59", "yyyy/dd HH:mm", DataType.DateTime, false, false)]
        [InlineData("11", "11", "2021/21/10 13:59", "dd/MM/yyyy HH:mm", DataType.DateTime, false, false)]
        public void ConvertFromStringToDateTimeTests(string testDescription, string expectedResultKey, string valueToTest, string dateFormat, string dataType, bool allowEmptyField, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var expectedResult = new Dictionary<string, DateTimeOffset?>
            {
                {  "1",  null },
                {  "2",  null },
                {  "3",  new DateTimeOffset(2021, 10, 21, 13, 59, 0, TimeSpan.Zero) },
                {  "4",  null },
                {  "5",  null },
                {  "6",  null },
                {  "7",  new DateTimeOffset(2021, 10, 21, 13, 59, 0, TimeSpan.Zero) },
                {  "8",  new DateTimeOffset(2021, 10, 21, 13, 59, 0, TimeSpan.Zero) },
                {  "9",  new DateTimeOffset(9999, 10, 21, 13, 59, 0, TimeSpan.Zero) },
                {  "10", new DateTimeOffset(2021, 1, 21, 13, 59, 0, TimeSpan.Zero) },
                {  "11", null },
            };

            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = dateFormat,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertFromString(valueToTest,
                new Mock<IReader>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult[expectedResultKey], $"Error in test {testDescription}");
            }
        }


        [Theory]
        [InlineData("1", "1", DefaultFormat.DateTime, DataType.DateTime, false, null, true)]
        [InlineData("3", "3", DefaultFormat.DateTime, DataType.DateTime, false, "2021/10/21 13:19", false)]
        [InlineData("7", "7", "yyyy/dd/MM HH:mm", DataType.DateTime, false, "2021/21/10 13:19", false)]
        [InlineData("8", "8", "dd/MM/yyyy HH:mm", DataType.DateTime, false, "21/10/2021 13:19", false)]
        [InlineData("9", "9", "yyyy/dd/MM HH:mm", DataType.DateTime, false, "9999/21/10 13:19", false)]
        [InlineData("10", "10", "yyyy/dd HH:mm", DataType.DateTime, false, "2021/21 13:19", false)]
        [InlineData("11", "11", "yyyy/dd HH:mm", DataType.DateTime, false, "9999/31 23:59", false)]
        [InlineData("12", "12", "yyyy/dd/MM HH:mm", DataType.DateTime, false, "9999/31/12 23:59", false)]
        [InlineData("13", "13", "yyyy/dd HH:mm", DataType.DateTime, false, "0001/01 00:00", false)]
        [InlineData("14", "14", "yyyy/dd/MM HH:mm", DataType.DateTime, false, "0001/01/01 00:00", false)]
        public void ConvertToStringFromDateTimeTests(string testDescription, string valueToTestKey, string dateFormat, string dataType, bool allowEmptyField, string expectedResult, bool throwsException)
        {
            // Cannot pass (compilation error) decimal as a parameter as inlinedata - so using this dictionary as a lookup with expectedResultKey
            var valueToTest = new Dictionary<string, DateTimeOffset?>
            {
                {  "1",  null },
                {  "3",  new DateTimeOffset(2021, 10, 21, 13, 19, 0, TimeSpan.Zero) },
                {  "7",  new DateTimeOffset(2021, 10, 21, 13, 19, 0, TimeSpan.Zero) },
                {  "8",  new DateTimeOffset(2021, 10, 21, 13, 19, 0, TimeSpan.Zero) },
                {  "9",  new DateTimeOffset(9999, 10, 21, 13, 19, 0, TimeSpan.Zero) },
                {  "10", new DateTimeOffset(2021, 1, 21, 13, 19, 0, TimeSpan.Zero) },
                {  "11", new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero) },
                {  "12", new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero) },
                {  "13", new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero) },
                {  "14", new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero) },
            };

            //
            // Setup
            //
            var profile = new FieldConfiguration
            {
                Name = "Test",
                DataType = dataType,
                DateTimeFormat = dateFormat,
                AllowEmptyField = allowEmptyField
            };
            var converter = new FieldTypeConverter(profile);

            //
            // Act
            //
            object result = null;
            var exception = Xunit.Record.Exception(() =>
               result = converter.ConvertToString(valueToTest[valueToTestKey],
                new Mock<IWriterRow>().Object,
                new MemberMapData(this.GetType().GetMember(nameof(ConvertFromStringToStringTests)).First())));

            //
            // Verify
            //
            if (throwsException)
            {
                Assert.NotNull(exception);
                Assert.IsType<FieldTypeConverterException>(exception);
            }
            else
            {
                result.Should().Be(expectedResult, $"Error in test {testDescription}");
            }
        }
    }
}
