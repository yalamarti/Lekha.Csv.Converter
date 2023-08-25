using CsvHelper;
using CsvHelper.Configuration;
using Lekha.Csv.Converter.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lekha.Csv.Converter
{
    /// <summary>
    /// Implements the CSV to a Dictionary<string, objecg></string> convertion functionality
    /// </summary>
    public class CsvToDictionaryConverter : ICsvToDictionaryConverter
    {
        public const string NoRecordsProcessed = "No records processed";
        public const string DefaultDelimiter = ",";
        const int MaximumFieldCountAllowed = 20000;
        private readonly ILogger logger = null;

        /// <summary>
        /// Constructor - for use with no logging
        /// </summary>
        public CsvToDictionaryConverter()
        {
            this.logger = new NullLogger<CsvToDictionaryConverter>();
        }

        /// <summary>
        /// Constructor - with logger specified
        /// </summary>
        /// <param name="logger"></param>
        public CsvToDictionaryConverter(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
        }

        private FieldConfiguration NewFieldConfiguration(int index, string fieldNamePrefix)
        {
            var name = $"{fieldNamePrefix}{index}";
            return NewFieldConfiguration(name);
        }
        private FieldConfiguration NewFieldConfiguration(string name)
        {
            return new FieldConfiguration
            {
                Name = name,
                DataType = DataType.String,
                AllowedMaximumLength = FieldLimits.MaximumLength,
                AllowEmptyField = true,
                Required = false,
                Title = name
            };
        }
        private (ConversionResult ParseResult, List<FieldConfiguration> FieldConfigurations) SetupDefaultFieldConfigurations(Stream stream, bool hasHeaderRecord, string fieldNamePrefix)
        {
            using var reader = new StreamReader(stream, Encoding.Default, false, 1024, true);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var retVal = new List<FieldConfiguration>();

            csvReader.Read();
            if (hasHeaderRecord)
            {
                var index = 1;
                try
                {
                    csvReader.ReadHeader();
                }
                catch (ReaderException ex)
                {
                    logger.LogWarning(ex, "Either no records in the CSV data or there was an error reading the CSV data - while setting up default field configuration");
                    return (new ConversionResult
                    {
                        Success = true,
                        Message = NoRecordsProcessed,
                        Observations = new List<string> { "Either no records in the CSV data or there was an error reading the CSV data." }
                    }, retVal);
                }
                foreach (var header in csvReader.HeaderRecord)
                {
                    var fieldConfiguration = NewFieldConfiguration(header);
                    retVal.Add(fieldConfiguration);
                    index++;
                    logger.LogInformation("Header field {FieldName} at index {FieldIndex} with default properties", fieldConfiguration.Name, index);
                }
            }
            else
            {
                for (int index = 0; index <= MaximumFieldCountAllowed; index++)
                {
                    var value = csvReader.TryGetField(index, out string fieldValue);
                    if (value)
                    {
                        var fieldConfiguration = NewFieldConfiguration(index + 1, fieldNamePrefix);
                        retVal.Add(fieldConfiguration);
                        logger.LogInformation("Header field {FieldName} at index {FieldIndex} with default properties", fieldConfiguration.Name, index);
                    }
                    else
                    {
                        logger.LogInformation("Determined header as having {FieldCount} from the first record in the stream", index);
                        break;
                    }
                }

            }
            return (null, retVal);
        }

        private ConversionResult ValidateHeaderAndFieldConfiguration(Stream stream, List<FieldConfigurationDto> sanitizedFieldConfigurations,
            bool hasHeaderRecord, bool userSpecifiedFieldConfigurations)
        {
            var requiredFieldCount = sanitizedFieldConfigurations
                .Where(i => i.Configuration.Required == true)
                .Count();
            if (userSpecifiedFieldConfigurations && hasHeaderRecord == false && requiredFieldCount > 0)
            {
                var requiredFieldNames = sanitizedFieldConfigurations
                    .Where(i => i.Configuration.Required == true)
                    .Select(i => i.Configuration.Name);
                var fieldNames = string.Join(",", requiredFieldNames.ToArray());
                return new ConversionResult
                {
                    Message = $"Field(s) '{fieldNames}' are configured as 'Required'.  'Required' fields are supported only when the '{nameof(ConverterConfiguration.HasHeaderRecord)}' is set to true and the CSV data has a header record."
                };
            }

            using var reader = new StreamReader(stream, Encoding.Default, false, 1024, true);
            using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { IgnoreBlankLines = true });

            var retVal = new List<FieldConfiguration>();
            string[] headerRowFromStream = null;

            try
            {
                csvReader.Read();
                csvReader.ReadHeader();
            }
            catch (ReaderException ex)
            {
                logger.LogWarning(ex, "Either no records in the CSV data or there was an error reading the CSV data - while validating the header/configuration");
                return new ConversionResult
                {
                    Success = true,
                    Message = NoRecordsProcessed,
                    Observations = new List<string> { "Either no records in the CSV data or there was an error reading the CSV data." }
                };
            }

            headerRowFromStream = csvReader.HeaderRecord;
            if (headerRowFromStream == null || headerRowFromStream.Length == 0)
            {
                // No records in the CSV data
                // Nothing to process
                return new ConversionResult
                {
                    Success = true,
                    Message = NoRecordsProcessed
                };
            }

            var fieldConfigurationSource = sanitizedFieldConfigurations.FirstOrDefault()?.FieldConfigSource;
            // Validate if Field info specified in CSV data matches with actual Header fields
            var fieldsWithNoName = sanitizedFieldConfigurations
                .Where(i => string.IsNullOrWhiteSpace(i.SanitizedFieldName));
            if (fieldsWithNoName.Count() > 0)
            {
                return new ConversionResult
                {
                    Message =
                        fieldConfigurationSource == FieldConfigurationSource.FromFieldConfiguration ?
                            "CSV data Record Configuration - field with no name specified found!  When one or more fields are specified, Field Name is required for every field!"
                            : "Header - field with no name specified found!  When one or more fields are specified, Field Name is required for every field!"
                };
            }

            // Validate if Field info specified in CSV data matches with actual Header fields
            var duplicateFields = sanitizedFieldConfigurations
                .GroupBy(s => s.SanitizedFieldName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            if (duplicateFields.Count() > 0)
            {
                string names = string.Join(",", duplicateFields);
                return new ConversionResult
                {
                    Message =
                        fieldConfigurationSource == FieldConfigurationSource.FromFieldConfiguration
                        ? $"CSV data Record Configuration - field with same name(s) '{names}' appears more than once"
                        : $"Header - field with same name(s) '{names}' appears more than once",
                    DuplicateFields = duplicateFields.ToArray()
                };
            }

            //
            // Expected header record specified by the user
            //

            var missingHeaderFieldInStream = new List<string>();
            var sanitizedFieldsRequired = sanitizedFieldConfigurations
                .Where(i => i.Configuration.Required == true);
            foreach (var sanitizedFieldRequired in sanitizedFieldsRequired)
            {
                if (headerRowFromStream.FirstOrDefault(i => i.ToSanitizedFieldName() == sanitizedFieldRequired.SanitizedFieldName) == null)
                {
                    missingHeaderFieldInStream.Add(sanitizedFieldRequired.Configuration.Name);
                }
            }
            if (missingHeaderFieldInStream.Count > 0)
            {
                var fieldNames = string.Join(",", missingHeaderFieldInStream.ToArray());

                return new ConversionResult
                {
                    Message = $"Field(s) '{fieldNames}' specified in Record Configuration but are missing in the header data",
                    FoundHeaders = headerRowFromStream,
                    ExpectedHeaders = sanitizedFieldsRequired.Select(i => i.Configuration.Name).ToArray(),
                    MissingHeaderFields = missingHeaderFieldInStream.ToArray()
                };
            }

            return null;
        }

        private List<FieldConfigurationDto> SanitizedFieldInfo(List<FieldConfiguration> fields, FieldConfigurationSource fieldConfigurationSource)
        {
            var retVal = new List<FieldConfigurationDto>();
            foreach (var field in fields)
            {
                retVal.Add(new FieldConfigurationDto
                {
                    Configuration = field,
                    SanitizedFieldName = field.Name.ToSanitizedFieldName(),
                    FieldConfigSource = fieldConfigurationSource
                });
            }
            return retVal;
        }

        private void SetupObservations(ConverterConfiguration converterConfiguration, ConversionResult parseResult)
        {
            if (converterConfiguration.CommentCharacter == null)
            {
                parseResult.Observations.Add("No comment character specified.  Assuming no commented lines.");
            }
            else
            {
                parseResult.Observations.Add($"Comment character specified.  Lines starting with '{converterConfiguration.CommentCharacter.Value}' character will be ignored.");
            }
            if (string.IsNullOrWhiteSpace(converterConfiguration.RecordConfiguration?.Delimiter))
            {
                parseResult.Observations.Add("No field delimiter specified.  Delimiter will be auto-detected.");
            }
        }

        private CsvConfiguration SetupCsvReaderConfiguration(ConverterConfiguration converterConfiguration)
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = (converterConfiguration.HasHeaderRecord == true)
            };
            if (converterConfiguration.CommentCharacter != null)
            {
                csvConfiguration.Comment = converterConfiguration.CommentCharacter.Value;
                csvConfiguration.AllowComments = true;
            }

            if (string.IsNullOrWhiteSpace(converterConfiguration.RecordConfiguration?.Delimiter))
            {
                csvConfiguration.DetectDelimiter = true;
            }
            else
            {
                csvConfiguration.Delimiter = converterConfiguration.RecordConfiguration?.Delimiter;
            }

            csvConfiguration.PrepareHeaderForMatch += new PrepareHeaderForMatch((PrepareHeaderForMatchArgs args) =>
            {
                return args.Header?.ToSanitizedFieldName();
            });

            return csvConfiguration;
        }

        private ConverterConfiguration GetDefaultConverterConfiguration()
        {
            return new ConverterConfiguration
            {
                RecordConfiguration = new RecordConfiguration
                {
                    Delimiter = DefaultDelimiter,
                    Fields = new List<FieldConfiguration>(),
                }
            };
        }


        public async Task<ConversionResult> ConvertAsync(Stream stream,
            Func<long, Dictionary<string, object>, Task> processedRecordCallback,
            Func<ParseError, Task<bool>> errorCallback)
        {
            var converterConfiguration = GetDefaultConverterConfiguration();
            return await ConvertAsync(stream,
                converterConfiguration,
                processedRecordCallback,
                errorCallback);
        }

        public async Task<ConversionResult> ConvertAsync(Stream stream,
            ConverterConfiguration converterConfiguration,
            Func<long, Dictionary<string, object>, Task> processedRecordCallback,
            Func<ParseError, Task<bool>> errorCallback)
        {
            Dictionary<string, object> processedRecord = new Dictionary<string, object>();
            var retVal = await Task.Run(() => ConvertAsync(stream,
            converterConfiguration,
            (long recordIndex, int fieldIndex, string fieldName, object fieldValue) =>
            {
                if (fieldIndex == -1)
                {
                    var t = processedRecordCallback(recordIndex, processedRecord);
                    t.Wait();
                    processedRecord = new Dictionary<string, object>();
                }
                else
                {
                    processedRecord[fieldName] = fieldValue;
                }
                return true;
            }, (ParseError parseError) =>
            {
                var t = errorCallback(parseError);
                t.Wait();
                return t.Result;
            }));
            return retVal;
        }

        public ConversionResult ConvertAsync(Stream stream,
            Func<long, int, string, object, bool> processedFieldCallback,
            Func<ParseError, bool> errorCallback)
        {
            var converterConfiguration = GetDefaultConverterConfiguration();
            return ConvertAsync(stream,
                converterConfiguration,
                processedFieldCallback,
                errorCallback);
        }

        public ConversionResult ConvertAsync(Stream stream,
            ConverterConfiguration converterConfiguration,
            Func<long, int, string, object, bool> processedFieldCallback,
            Func<ParseError, bool> errorCallback)
        {
            var result = new ConversionResult
            {
                Observations = new List<string>()
            };

            #region Setup and validate input
            if (converterConfiguration == null)
            {
                converterConfiguration = new ConverterConfiguration
                {
                };
                // No header
                result.Observations.Add("No header specified.  Defaulting to 'No Header'.");
            }
            result.Configuration = converterConfiguration;

            if (converterConfiguration.RecordConfiguration == null)
            {
                converterConfiguration.RecordConfiguration = new RecordConfiguration
                {
                };
                result.Observations.Add($"No Field specification found.  Will default all fields to be of {DataType.String} data type.");
            }

            var userSpecifiedFieldConfigurations = converterConfiguration.RecordConfiguration?.Fields?.Count > 0;

            bool hasHeaderRecord = (converterConfiguration.HasHeaderRecord == true);

            FieldConfigurationSource fieldInfoSource = FieldConfigurationSource.FromFieldConfiguration;

            if (userSpecifiedFieldConfigurations == false)
            {
                if (hasHeaderRecord)
                {
                    fieldInfoSource = FieldConfigurationSource.FromHeader;
                    result.Observations.Add($"No fields specified as part of {nameof(ConverterConfiguration)}.  Fields wiill be auto-detected.");
                }
                else
                {
                    fieldInfoSource = FieldConfigurationSource.AutoGenerated;
                    result.Observations.Add($"No header in CSV data or fields specified as part of {nameof(ConverterConfiguration)}.  Header and fields will be auto-detected.  All fields will be of {DataType.String} type.");
                }
                (ConversionResult defaultConfigurationResult, List<FieldConfiguration> fieldConfigurations) = SetupDefaultFieldConfigurations(stream, hasHeaderRecord, converterConfiguration.FieldNamePrefix);
                if (defaultConfigurationResult != null)
                {
                    return defaultConfigurationResult;
                }
                converterConfiguration.RecordConfiguration.Fields = fieldConfigurations;
                stream.Position = 0;
            }

            var sanitizedFieldConfigurations = SanitizedFieldInfo(converterConfiguration.RecordConfiguration.Fields, fieldInfoSource);

            CsvConfiguration csvConfiguration = SetupCsvReaderConfiguration(converterConfiguration);
            var parseResult = ValidateHeaderAndFieldConfiguration(stream, sanitizedFieldConfigurations, hasHeaderRecord, userSpecifiedFieldConfigurations);
            if (parseResult != null)
            {
                return parseResult;
            }
            stream.Position = 0;

            //
            // Set defaults for missing attributes
            //

            foreach (var sanitizedField in sanitizedFieldConfigurations)
            {
                if (string.IsNullOrWhiteSpace(sanitizedField.Configuration.DataType))
                {
                    sanitizedField.Configuration.DataType = DataType.String;
                }
            }

            //
            // Setup CSV Reader configuration
            //
            SetupObservations(converterConfiguration, result);

            var typeConverters = new Dictionary<FieldConfigurationDto, FieldTypeConverter>();

            foreach (var field in sanitizedFieldConfigurations)
            {
                typeConverters[field] = new FieldTypeConverter(field.Configuration);
            }

            #endregion Setup and validate input

            #region Parse

            using var reader = new StreamReader(stream);
            using var csvReader = new CsvReader(reader, csvConfiguration);

            ParseError error = null;
            var fieldIndex = 0;
            bool parseFieldToValue = false;

            if (hasHeaderRecord)
            {
                csvReader.Read();
                csvReader.ReadHeader();
            }

            // 
            // Note.  Tried the CsvReader's RecordMap way of mapping the record to a Dictionary<string, object>
            //   but ran into issues.  Could not figure out how to map to individual elements ... all the
            //   examples referred to class/property-name based mapping, not dictionary-key mapping
            //   So, parsing individual field 'by hand' here.
            //

            while (csvReader.Read())
            {
                error = null;
                fieldIndex = 0;

                #region Parse individual record
                error = null;

                foreach (var sanitizedFieldDto in sanitizedFieldConfigurations)
                {
                    object objectValue = null;
                    parseFieldToValue = false;

                    try
                    {
                        if (hasHeaderRecord)
                        {
                            objectValue = csvReader.GetField<object>(sanitizedFieldDto.SanitizedFieldName, typeConverters[sanitizedFieldDto]);
                        }
                        else
                        {
                            // User specified configurations have datatype specified.  try getting the field as 'object'
                            objectValue = csvReader.GetField<object>(fieldIndex, typeConverters[sanitizedFieldDto]);
                        }
                        parseFieldToValue = true;
                    }
                    catch (FieldTypeConverterException ex)
                    {
                        var fieldIndexToReport = csvReader.CurrentIndex + 1;
                        bool emptyValuedOptionalField = false;
                        if (sanitizedFieldDto.Configuration.Required == false)
                        {
                            var stringValue = csvReader.GetField<string>(fieldIndex);
                            emptyValuedOptionalField = string.IsNullOrWhiteSpace(stringValue);
                        }
                        if (emptyValuedOptionalField)
                        {
                            result.Observations.Add($"Found an empty optional field at index {fieldIndexToReport} with name {sanitizedFieldDto.Configuration.Name} on record {csvReader.Context.Parser.Row}");
                        }
                        else
                        {
                            error = new ParseError
                            {
                                Location = new Field
                                {
                                    Name = sanitizedFieldDto.SanitizedFieldName,
                                    RecordIndex = csvReader.Context.Parser.Row,
                                    FieldIndex = csvReader.CurrentIndex + 1
                                },
                                Code = ParserErrorCode.FieldFormatFailure,
                                Message = ex.Message
                            };
                            logger.LogError(ex, "Error parsing field {FieldError}", error);
                        }
                    }
                    catch (CsvHelper.MissingFieldException ex)
                    {
                        var fieldIndexToReport = csvReader.CurrentIndex + 1;
                        if (sanitizedFieldDto.Configuration.Required)
                        {
                            error = new ParseError
                            {
                                Location = new Field
                                {
                                    Name = sanitizedFieldDto.Configuration.Name,
                                    RecordIndex = csvReader.Context.Parser.Row,
                                    FieldIndex = fieldIndexToReport
                                },
                                Code = ParserErrorCode.MissingField,
                                Message = "Missing required field"
                            };
                            logger.LogError(ex, "Error parsing field {FieldError}", error);
                        }
                        else
                        {
                            result.Observations.Add($"Missing optional field at index {fieldIndexToReport} with name {sanitizedFieldDto.Configuration.Name} on record {csvReader.Context.Parser.Row}");
                        }
                    }

                    if (parseFieldToValue)
                    {
                        var retVal = processedFieldCallback(result.ProcessedRecordCount + 1, fieldIndex + 1, sanitizedFieldDto.Configuration.Name, objectValue);
                        if (retVal == false)
                        {
                            break;
                        }
                    }
                    else if (error != null)
                    {
                        result.Errors.Add(error);
                        result.ErrorRecordCount++;
                        break;
                    }

                    fieldIndex++;
                }

                result.ProcessedRecordCount++;

                #endregion Parse individual record

                #region Act on parsed record

                if (error == null)
                {
                    // Do something with the record.
                    if (processedFieldCallback != null)
                    {
                        // -1 represents - end of record
                        processedFieldCallback(result.ProcessedRecordCount, -1, null, null);
                    }
                }
                else
                {
                    var continueParsing = errorCallback(error);
                    if (continueParsing == false)
                    {
                        break;
                    }
                }

                #endregion Act on parsed record
            }

            #endregion Parse

            result.Success = result.Errors.Count == 0;
            result.Message = string.IsNullOrWhiteSpace(result.Message) ? (result.ProcessedRecordCount == 0 ? NoRecordsProcessed : null) : result.Message; return result;
        }
    }
}
