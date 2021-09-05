using Lekha.Csv.Converter.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lekha.Csv.Converter.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await ProcessUsingDefaultConfiguration();
            //await ProcessUsingSpecifiedConfigurationWithNoHeaderInCsvData();
            //await ProcessUsingSpecifiedConfigurationWithHeaderInCsvData();
            await ProcessUsingSpecifiedConfigurationWithHeaderInCsvData();
        }

        /// <summary>
        /// Sample showing the default behavior
        /// Uses comma as the delimited.  "Field" as the prefix name.  Considers to header in the CSV data
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessUsingDefaultConfiguration()
        {
            var convertedDictionaryData = new List<Dictionary<string, object>>();

            using var fileStream = File.OpenRead("SampleDataFiles/SampleData.csv");
            ICsvToDictionaryConverter converter = new CsvToDictionaryConverter();

            ConversionResult result = await converter.ConvertAsync(fileStream,
                 (long recordIndex, Dictionary<string, object> parsedRecord) =>
                 {
                     convertedDictionaryData.Add(parsedRecord);
                     return Task.CompletedTask;
                 },
                 (ParseError error) =>
                 {
                     return Task.FromResult(true);
                 });
            Console.WriteLine(JsonSerializer.Serialize(convertedDictionaryData, new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        /// Sample showing using custom configuration
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessUsingSpecifiedConfigurationWithNoHeaderInCsvData()
        {
            var convertedDictionaryData = new List<Dictionary<string, object>>();

            using var fileStream = File.OpenRead("SampleDataFiles/SampleData2.csv");
            ICsvToDictionaryConverter converter = new CsvToDictionaryConverter();

            ConversionResult result = await converter.ConvertAsync(fileStream, 
                new ConverterConfiguration
                {
                    FieldNamePrefix = "",
                    HasHeaderRecord = false,
                    CommentCharacter = '#',
                    RecordConfiguration = new RecordConfiguration
                    {
                        Delimiter = "|",
                        Fields = new List<FieldConfiguration>
                        {
                            new FieldConfiguration
                            {
                                Name = "CustomerName"
                            },
                            new FieldConfiguration
                            {
                                Name = "Age",
                                DataType = DataType.UnsignedNumber
                            },
                            new FieldConfiguration
                            {
                                Name = "Salary",
                                DataType = DataType.Decimal
                            },
                        }
                    }
                },
                (long recordIndex, Dictionary<string, object> parsedRecord) =>
                {
                    convertedDictionaryData.Add(parsedRecord);
                    return Task.CompletedTask;
                },
                (ParseError error) =>
                {
                    return Task.FromResult(true);
                });
            Console.WriteLine(JsonSerializer.Serialize(convertedDictionaryData, 
                new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        ///  Sample showing using custom configuration, with CSV data having header
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessUsingSpecifiedConfigurationWithHeaderInCsvData()
        {
            var convertedDictionaryData = new List<Dictionary<string, object>>();

            using var fileStream = File.OpenRead("SampleDataFiles/SampleData3.csv");
            ICsvToDictionaryConverter converter = new CsvToDictionaryConverter();

            ConversionResult result = await converter.ConvertAsync(fileStream, 
                new ConverterConfiguration
                {
                    FieldNamePrefix = "",
                    HasHeaderRecord = false,
                    CommentCharacter = '#',
                    RecordConfiguration = new RecordConfiguration
                    {
                        Delimiter = "|",
                        Fields = new List<FieldConfiguration>
                            {
                                new FieldConfiguration
                                {
                                    Name = "CustomerName"
                                },
                                new FieldConfiguration
                                {
                                    Name = "Age",
                                    DataType = DataType.UnsignedNumber
                                },
                                new FieldConfiguration
                                {
                                    Name = "Salary",
                                    DataType = DataType.Decimal
                                },
                                new FieldConfiguration
                                {
                                    Name = "HiredDate",
                                    DataType = DataType.DateTime,
                                    DateTimeFormat = "yyyy/MM/dd HH:mm:ss"
                                }
                            }
                    }
                },
                (long recordIndex, Dictionary<string, object> parsedRecord) =>
                {
                    convertedDictionaryData.Add(parsedRecord);
                    return Task.CompletedTask;
                },
                (ParseError error) =>
                {
                    return Task.FromResult(true);
                });
            Console.WriteLine(JsonSerializer.Serialize(convertedDictionaryData, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
