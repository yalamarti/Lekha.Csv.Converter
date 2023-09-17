﻿using FluentAssertions;
using Lekha.Csv.Converter.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Lekha.Csv.Converter.Tests
{
    public class CsvToDictionaryConverterTests
    {
        public class TestCase
        {
            public decimal TestCaseName { get; set; }
            public string CsvData { get; set; }
            public List<string> TestDataDetails { get; set; }
            public ConverterConfiguration FileConfiguration { get; set; }
            public TestResult ExpectedResult { get; set; }
            public string ErrorMessage { get; set; }
        }
        public class TestResult
        {
            [JsonConverter(typeof(ItemListConverter<DictionaryStringObjectJsonConverter, Dictionary<string, object>>))]
            public List<Dictionary<string, object>> RecordsWithDataTypeValues { get; set; }
            public List<Dictionary<string, string>> RecordsWithStringValues { get; set; }
            public string ExceptionType { get; set; }
            public long ProcessedRecordCount { get; set; }
            public long ErrorRecordCount { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        [Theory]
        [InlineData(2.001, "Should parse a single record successfully, detecting comma as delimiter,  with no header and no field configuration specified")]
        [InlineData(2.002, "Should parse a single record, with single field successfully, detecting comma as delimiter, with no header and no field configuration specified")]
        [InlineData(2.003, "Should parse a multiple records, with single field successfully, detecting comma as delimiter,  with no header and no field configuration specified")]
        [InlineData(3, "Should parse a single record successfully with header and field configuration specified")]
        [InlineData(4, "Should parse a single record successfully with no header and field configuration specified")]
        [InlineData(5, "Should parse successfully two records, and detect all fields as 'string' datatype with header specified but no field configuration specified")]
        [InlineData(6, "Should parse a single record successfully and detect all fields as 'string' datatype with header specified but no field configuration specified")]
        [InlineData(7, "Should parse successfully two records, skip additional field and detect all fields as 'string' datatype with header specified but no field configuration specified")]
        [InlineData(8, "Should return and process no records, when data is empty")]
        [InlineData(8.001, "Should parse successfully header specified, but no field configuration is specified, and process empty records, when data is empty")]
        [InlineData(8.002, "Should parse successfully when no header specified, but a field configuration is specified, and process empty records, when blank lines specified")]
        [InlineData(8.003, "Should parse successfully header specified, and field configuration is specified, and process empty records, when data is empty")]
        [InlineData(8.004, "Should parse successfully header specified, and field configuration is specified, and process empty records, when data is blank")]
        [InlineData(9, "Should parse successfully two records, skip additional middle field and detect all fields as 'string' datatype with header specified but no field configuration specified")]
        [InlineData(10, "Should parse and default to a 'string' data type, when no datatype is specified as part of a field configuration")]
        [InlineData(11.001, "Should parse successfully two records, skip additional field, detect and skip first-line comment line, and detect all fields as 'string' datatype with header specified but no field configuration specified")]
        [InlineData(11.002, "Should parse successfully two records, skip additional field, detect and non-first-line skip comment line, and detect all fields as 'string' datatype with header specified but no field configuration specified")]
        [InlineData(12.001, "Should parse successfully two records, detect quoted fields, detect and skip comment line, and detect all fields as 'string' datatype with header specified and field configuration specified")]
        [InlineData(12.002, "Should parse successfully two records, detect quoted fields, detect and skip comment line, detect default separator as 'comma' and detect all fields' datatypes with header specified and field configuration specified")]
        [InlineData(13.001, "Should fail parsing a single record  with header and field configuration specified and a mismatching data type in one of the fields")]
        [InlineData(13.002, "Should fail parsing a single record  with header and field configuration specified and a missing field in the header")]
        [InlineData(13.003, "Should parse successfully a single record  with header and field configuration specified and a missing a 'optional' field in the header")]
        [InlineData(13.004, "Should parse successfully a single record  with header and field configuration and additional field values in the rows with no corresponding header, ignoring additional fields")]
        [InlineData(13.005, "Should parse successfully a multiple records  with header and field configuration and additional field values in the rows with no corresponding header and additional values in records, ignoring additional fields")]
        [InlineData(13.006, "Should parse successfully a multiple records  with no header but field configuration specified, and additional values in records, ignoring additional fields")]
        [InlineData(13.007, "Should parse successfully a multiple records  with header and field configuration and, header having leading and trailing spaces in the name")]
        [InlineData(13.008, "Should parse successfully a multiple records  with header and field configuration and, header and field configuration having leading and trailing spaces in the name")]
        [InlineData(13.009, "Should parse successfully a multiple records  with header and field configuration and, header and field configuration having leading and trailing spaces in the name, and names differ in case sensitivity")]
        [InlineData(13.010, "Should parse successfully a multiple records  with header and field configuration and, header and field configuration having leading and trailing spaces in the name, and names differ in case sensitivity 2")]
        [InlineData(14.000, "Should parse successfully when no header and field configuration specified, and some records have missing values for optional fields")]
        [InlineData(14.001, "Should parse successfully when no header and field configuration specified, and some records have empty values for optional fields")]
        [InlineData(14.002, "Should parse successfully when no field configuration specified, but a header is specified, and some records have empty values for optional fields")]
        [InlineData(14.003, "Should parse successfully when no field configuration specified, but a header is specified, and some records have empty values for optional fields")]
        [InlineData(14.004, "Should parse successfully when no header specified, but a field configuration is specified, and some records have empty values for optional fields")]
        [InlineData(14.005, "Should parse successfully when no header specified, but a field configuration is specified, and some records have empty values for optional fields")]
        [InlineData(14.006, "Should parse successfully when both header and field configuration is specified, and some records have empty values for optional fields")]
        [InlineData(14.007, "Should parse successfully when both header and field configuration is specified, and some records have empty values for optional fields")]
        [InlineData(14.008, "Should fail parsing when no header specified, but a field configuration is specified, and some records have empty values for required fields")]
        [InlineData(14.009, "Should fail parsing when both header and field configuration is specified, and some records have empty values for required fields")]
        [InlineData(15, "Should fail parsing when header but no field configuration is specified, and a header field name is missing")]
        [InlineData(15.1, "Should fail parsing when header but no field configuration is specified, and the same header field is specified more than once")]
        [InlineData(15.2, "Should fail parsing when header but no field configuration is specified, and the same header field is specified more than once, case insensitive")]
        [InlineData(15.3, "Should fail parsing when header but no field configuration is specified, and the same header field is specified more than once")]
        [InlineData(15.4, "Should fail parsing when header but no field configuration is specified, and the same header field is specified more than once, case insensitive")]
        [InlineData(15.5, "Should fail parsing when no header specified, but field configuration is specified, and has a missing field name")]
        [InlineData(15.6, "Should fail parsing when header and field configuration are specified, and missing required fields in the header")]
        [InlineData(16, "Should parse successfully when optional fields have empty values")]
        [InlineData(17, "Should Failed to convert incorrectly formatted datetime value")]
        [InlineData(18, "Should parse a single record successfully, detecting as delimiter when empt delimiter specified,  with no header and no field configuration specified")]
        [InlineData(19, "Should parse a single record successfully with header and field configuration specified using a tab character as delimiter")]
        [InlineData(20, "Should parse a single record successfully with header and field configuration specified using a space character as delimiter")]

        public async Task TestCsvToDictionaryConverter(decimal testCaseName, string description)
        {
            //
            // Setup
            //
            var testCaseDataFile = "DataFiles/TestCase.json";
            var data = File.ReadAllText(testCaseDataFile);

            var testCases = JsonSerializer.Deserialize<List<TestCase>>(data);

            var actualResult = new TestResult
            {
                RecordsWithDataTypeValues = new List<Dictionary<string, object>>(),
                RecordsWithStringValues = new List<Dictionary<string, string>>()
            };

            var testCase = testCases.FirstOrDefault(i => i.TestCaseName == testCaseName);
            if (testCase == null)
            {
                throw new System.Exception($"Error locating test case in test case data file {testCaseDataFile} test.  Make sure there is a test case having {testCaseName} as the TestCaseName.  Test case description: {description}");
            }
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(testCase.CsvData));
            var sut = new CsvToDictionaryConverter(new NullLogger<CsvToDictionaryConverter>());

            //
            // Act
            //
            string errorMessageFound = null;
            ConversionResult result = null;
            var exception = await Xunit.Record.ExceptionAsync(async () => result = await sut.ConvertAsync(stream, testCase.FileConfiguration,
                 (long recordIndex, Dictionary<string, object> parsedRecord) =>
                 {
                     if (testCase.ExpectedResult.RecordsWithStringValues.Count > 0)
                     {
                         var convertedDictionary = new Dictionary<string, string>();
                         foreach (var kv in parsedRecord)
                         {
                             convertedDictionary.Add(kv.Key, kv.Value.ToString());
                         }
                         actualResult.RecordsWithStringValues.Add(convertedDictionary);
                     }
                     else
                     {
                         actualResult.RecordsWithDataTypeValues.Add(parsedRecord);
                     }
                     return Task.CompletedTask;
                 },
                 (ParseError error) =>
                 {
                     errorMessageFound = error.Message;
                     return Task.FromResult(true);
                 }));
            if (result != null)
            {
                actualResult.ProcessedRecordCount = result.ProcessedRecordCount;
                actualResult.ErrorRecordCount = result.ErrorRecordCount;
                actualResult.Success = result.Success;
                actualResult.Message = result.Message;
            }

            //
            // Verify
            //
            if (string.IsNullOrWhiteSpace(testCase.ExpectedResult.ExceptionType) == false)
            {
                exception.GetType().Name.Should().Be(testCase.ExpectedResult.ExceptionType);
            }
            else
            {
                var json = JsonSerializer.Serialize(actualResult);
                exception.Should().Be(null);
                actualResult.Should().BeEquivalentTo(testCase.ExpectedResult);
            }

            errorMessageFound.Should().Be(testCase.ErrorMessage);
        }

        [Theory]
        [InlineData(13.005, "Should parse successfully a multiple records  with header and field configuration and additional field values in the rows with no corresponding header and additional values in records, ignoring additional fields")]

        public void ShouldConvertUsingCsvToDictionaryConverter(decimal testCaseName, string description)
        {
            //
            // Setup
            //
            var testCaseDataFile = "DataFiles/TestCase.json";
            var data = File.ReadAllText(testCaseDataFile);

            var testCases = JsonSerializer.Deserialize<List<TestCase>>(data);

            var actualResult = new TestResult
            {
                RecordsWithDataTypeValues = new List<Dictionary<string, object>>(),
                RecordsWithStringValues = new List<Dictionary<string, string>>()
            };

            var testCase = testCases.FirstOrDefault(i => i.TestCaseName == testCaseName);
            if (testCase == null)
            {
                throw new System.Exception($"Error locating test case in test case data file {testCaseDataFile} test.  Make sure there is a test case having {testCaseName} as the TestCaseName.  Test case description: {description}");
            }
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(testCase.CsvData));
            var sut = new CsvToDictionaryConverter(new NullLogger<CsvToDictionaryConverter>());

            //
            // Act
            //
            Dictionary<string, object> processedRecord = new Dictionary<string, object>();
            ConversionResult result = null;
            var exception = Xunit.Record.Exception(() => result = sut.Convert(stream, testCase.FileConfiguration,
            (long recordIndex, int fieldIndex, string fieldName, object fieldValue) =>
            {
                if (fieldIndex == -1)
                {
                    if (testCase.ExpectedResult.RecordsWithStringValues.Count > 0)
                    {
                        var convertedDictionary = new Dictionary<string, string>();
                        foreach (var kv in processedRecord)
                        {
                            convertedDictionary.Add(kv.Key, kv.Value.ToString());
                        }
                        actualResult.RecordsWithStringValues.Add(convertedDictionary);
                    }
                    else
                    {
                        actualResult.RecordsWithDataTypeValues.Add(processedRecord);
                    }
                    processedRecord = new Dictionary<string, object>();
                }
                else
                {
                    processedRecord[fieldName] = fieldValue;
                }
                return true;
            }, (ParseError parseError) =>
            {
                return true;
            }));
            if (result != null)
            {
                actualResult.ProcessedRecordCount = result.ProcessedRecordCount;
                actualResult.ErrorRecordCount = result.ErrorRecordCount;
                actualResult.Success = result.Success;
                actualResult.Message = result.Message;
            }

            //
            // Verify
            //
            if (string.IsNullOrWhiteSpace(testCase.ExpectedResult.ExceptionType) == false)
            {
                exception.GetType().Name.Should().Be(testCase.ExpectedResult.ExceptionType);
            }
            else
            {
                exception.Should().Be(null);
                actualResult.Should().BeEquivalentTo(testCase.ExpectedResult);
            }
        }

    }
}
