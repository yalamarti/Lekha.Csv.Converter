# Lekha.Csv.Converter

A library for converting CSV data into a Dictionary<string, object> data type.

Features:

- Supports .NET Standard 2.0
- Makes extensive use of [CsvHelper](https://joshclose.github.io/CsvHelper/) NuGet package.  So, it supports most of the behavior from that library.
- [FluentValidations](https://github.com/FluentValidation/FluentValidation) for validations in unit tests.
- XUnit for unit testing
- .NET System.Text.Json for all JSON handling

- Supports following data types as part of the Dictionary value objects:

```csharp
    /// <summary>
    /// Data types supported for CSV fields
    /// </summary>
    public struct DataType
    {
        /// <summary>
        /// Equivalent to .NET string type
        /// </summary>
        public const string String = "string";

        /// <summary>
        /// Equivalent to .NET signed long
        /// </summary>
        public const string SignedNumber = "number";

        /// <summary>
        /// Equivalent to .NET unsigned long
        /// </summary>
        public const string UnsignedNumber = "unsigned-number";

        /// <summary>
        /// Equivalent to .NET decimal
        /// </summary>
        public const string Decimal = "decimal";

        /// <summary>
        /// Equivalent/uses to .NET DateTimeOffset
        /// </summary>
        public const string Date = "date";

        /// <summary>
        /// Equivalent/uses to .NET DateTimeOffset
        /// </summary>
        public const string DateTime = "datetime";
    }

```
## Sample Usage 


### Using default configuration

* Sample CSV data 

```csv
Jon Doe,34658.22
Jane Dane,45002.33
```

* Sample code

```csharp
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
```

* Result:

```json
[
  {
    "Field1": "Jon Doe",
    "Field2": "34658.22"
  },
  {
    "Field1": "Jane Dane",
    "Field2": "45002.33"
  }
]
```


### Using custom configutation

* Sample CSV data 

```csv
Jon Doe|35|34658.22
#This is a commented line.  The whole line is ignored
Jane Dane|29|45002.33
```

* Sample code

```csharp
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
        new CsvConverterConfiguration
        {
            FieldNamePrefix = "",
            HasHeaderRecord = false,
            Comment = '#',
            AllowComments = true,
            Delimiter = "|",
            RecordConfiguration = new RecordConfiguration
            {
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

```

* Result

```json
[
  {
    "CustomerName": "Jon Doe",
    "Age": 35,
    "Salary": 34658.22
  },
  {
    "CustomerName": "Jane Dane",
    "Age": 29,
    "Salary": 45002.33
  }
]
```


### Using custom configutation with header, quoted fields and DateTime datatype in CSV data

* Sample CSV data 

```csv
CustomerName|Age|Salary|HiredDate
"Jon Doe"|"35"|"34658.22"|"2018/01/23 00:01:02"
#This is a commented line.  The whole line is ignored
"Jane Dane"|"29"|"45002.33"|"2019/01/22 13:01:02"
```

* Sample code

```csharp
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
        new CsvConverterConfiguration
        {
            FieldNamePrefix = "",
            HasHeaderRecord = false,
            Comment = '#',
            AllowComments = true,
            Delimiter = "|",
            RecordConfiguration = new RecordConfiguration
            {
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
```

* Result

```json
[
  {
    "CustomerName": "Jon Doe",
    "Age": 35,
    "Salary": 34658.22,
    "HiredDate": "2018-01-23T00:01:02+00:00"
  },
  {
    "CustomerName": "Jane Dane",
    "Age": 29,
    "Salary": 45002.33,
    "HiredDate": "2019-12-22T13:01:02+00:00"
  }
]
```

# Release Notes
  
 * v1.1.3
    * Added full support for CsvConfiguration model from CsvHelper, by introducing CsvConverterConfiguration 
   
 * v1.1.2
    * Release notes documentation added to Readme document. 
   
 * v1.1.0
    * Non-async support, with individual field parsed/processed callback. 
    * Updated CsvHelper package to latest version.
   
 * v1.0.1 (Breaking change)
    * Constructor with no ILogger requirement added.
    * Renamed FieldType to DataType.
    * Support for custom fieldname prefix.
    * Added Readme.md file.
    * Added samples project.

 * v1.0.0
   * Initial Version
