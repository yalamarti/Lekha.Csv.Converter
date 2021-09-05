using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Lekha.Csv.Converter.Models;
using System;
using System.Globalization;

namespace Lekha.Csv.Converter
{
    /// <summary>
    /// Field type converter implementation used for converting CSV fields.
    /// Converts objects to and from strings.
    /// </summary>
    public class FieldTypeConverter : ITypeConverter
    {
        private readonly FieldConfiguration fieldConfiguration;

        public FieldTypeConverter(FieldConfiguration fieldConfiguration)
        {
            this.fieldConfiguration = fieldConfiguration;
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="text">The string to convert to an object.</param>
        /// <param name="row">The CsvHelper.IReaderRow for the current record.</param>
        /// <param name="memberMapData">The CsvHelper.Configuration.MemberMapData for the member being created.</param>
        /// <returns></returns>
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch (fieldConfiguration.DataType)
            {
                case DataType.String:
                    if (string.IsNullOrWhiteSpace(text) == false && text.Length > fieldConfiguration.AllowedMaximumLength)
                    {
                        throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.SignedNumber}.  Exceeds maximum allowed length of {fieldConfiguration.AllowedMaximumLength}");
                    }
                    return text;
                case DataType.SignedNumber:
                    {
                        if (string.IsNullOrWhiteSpace(text) && fieldConfiguration.AllowEmptyField)
                        {
                            return null;
                        }
                        if (long.TryParse(text, out long value) == false)
                        {
                            throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.SignedNumber}");
                        }
                        return value;
                    }
                case DataType.UnsignedNumber:
                    {
                        if (string.IsNullOrWhiteSpace(text) && fieldConfiguration.AllowEmptyField)
                        {
                            return null;
                        }
                        if (ulong.TryParse(text, out ulong value) == false)
                        {
                            throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.UnsignedNumber}");
                        }
                        return value;
                    }
                case DataType.Decimal:
                    {
                        if (string.IsNullOrWhiteSpace(text) && fieldConfiguration.AllowEmptyField)
                        {
                            return null;
                        }
                        if (decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value) == false)
                        {
                            throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Decimal}");
                        }
                        return value;
                    }
                case DataType.Date:
                    {
                        if (string.IsNullOrWhiteSpace(text) && fieldConfiguration.AllowEmptyField)
                        {
                            return null;
                        }
                        if (DateTimeOffset.TryParseExact(text,
                            string.IsNullOrWhiteSpace(fieldConfiguration.DateTimeFormat) ? DefaultFormat.Date : fieldConfiguration.DateTimeFormat,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset value) == false)
                        {
                            if (string.IsNullOrWhiteSpace(fieldConfiguration.DateTimeFormat))
                                throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Date} using '{DefaultFormat.Date}' format");
                            else
                                throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Date} using '{fieldConfiguration.DateTimeFormat}' format");
                        }
                        return value;
                    }
                case DataType.Time:
                    {
                        if (string.IsNullOrWhiteSpace(text) && fieldConfiguration.AllowEmptyField)
                        {
                            return null;
                        }

                        if (TimeSpan.TryParseExact(text,
                            string.IsNullOrWhiteSpace(fieldConfiguration.DateTimeFormat) ? DefaultFormat.Time : fieldConfiguration.DateTimeFormat,
                            CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan value) == false)
                        {
                            if (string.IsNullOrWhiteSpace(fieldConfiguration.DateTimeFormat))
                                throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Time} using '{DefaultFormat.Time}' format");
                            else
                                throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Date} using '{fieldConfiguration.DateTimeFormat}' format");
                        }
                        return value;
                    }
                case DataType.DateTime:
                    {
                        if (string.IsNullOrWhiteSpace(text) && fieldConfiguration.AllowEmptyField)
                        {
                            return null;
                        }
                        if (DateTimeOffset.TryParseExact(text,
                            string.IsNullOrWhiteSpace(fieldConfiguration.DateTimeFormat) ? DefaultFormat.DateTime : fieldConfiguration.DateTimeFormat,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset value) == false)
                        {
                            if (string.IsNullOrWhiteSpace(fieldConfiguration.DateTimeFormat))
                                throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Date} using '{DefaultFormat.Date}' format");
                            else
                                throw new FieldTypeConverterException($"Failed to convert '{fieldConfiguration.ToMessage(text)}' to {DataType.Date} using '{fieldConfiguration.DateTimeFormat}' format");
                        }
                        return value;
                    }
                default:
                    {
                        throw new FieldTypeConverterException($"ConvertFromString: Invalid Field Type {fieldConfiguration.DataType} specified for '{fieldConfiguration.ToMessage(text)}'! Valid field types are : " +
                            $"{DataType.String},{DataType.SignedNumber},{DataType.UnsignedNumber},{DataType.Decimal},{DataType.Date},{DataType.Time},{DataType.DateTime}");
                    }
            }
        }

         /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <param name="value">The object to convert to a string.</param>
        /// <param name="row">The CsvHelper.IWriterRow for the current record.</param>
        /// <param name="memberMapData">The CsvHelper.Configuration.MemberMapData for the member being written.</param>
        /// <returns></returns>
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            switch (fieldConfiguration.DataType)
            {
                case DataType.String:
                    if (value?.ToString()?.Length > fieldConfiguration.AllowedMaximumLength)
                    {
                        throw new FieldTypeConverterException($"Failed to convert '{value}' to a {fieldConfiguration.DataType}.  Exceeds maximum allowed length of {fieldConfiguration.AllowedMaximumLength}");
                    }
                    return value?.ToString();
                case DataType.SignedNumber:
                case DataType.UnsignedNumber:
                    if (value == null && fieldConfiguration.AllowEmptyField == false)
                    {
                        throw new FieldTypeConverterException($"Failed to convert '{value}' to a {fieldConfiguration.DataType}.  Value is empty");
                    }
                    return value?.ToString();
                case DataType.Decimal:
                    if (value == null && fieldConfiguration.AllowEmptyField == false)
                    {
                        throw new FieldTypeConverterException($"Failed to convert '{value}' to a {fieldConfiguration.DataType}.  Value is empty");
                    }
                    return value == null ? null : ((decimal)(value)).ToString("G", CultureInfo.InvariantCulture);
                case DataType.Date:
                case DataType.DateTime:
                    if (value == null && fieldConfiguration.AllowEmptyField == false)
                    {
                        throw new FieldTypeConverterException($"Failed to convert '{value}' to a {fieldConfiguration.DataType}.  Value is empty");
                    }
                    return value == null ? null : ((DateTimeOffset)(value)).ToString(fieldConfiguration.DateTimeFormat);
                case DataType.Time:
                    if (value == null && fieldConfiguration.AllowEmptyField == false)
                    {
                        throw new FieldTypeConverterException($"Failed to convert '{value}' to a {fieldConfiguration.DataType}.  Value is empty");
                    }
                    return value == null ? null : ((TimeSpan)(value)).ToString(fieldConfiguration.DateTimeFormat);
                default:
                    {
                        throw new Exception($"ConvertToString: Invalid Field Type {fieldConfiguration.DataType} specified for value '{value}'! Valid field types are : " +
                            $"{DataType.String},{DataType.SignedNumber},{DataType.UnsignedNumber},{DataType.Decimal},{DataType.Date},{DataType.Time},{DataType.DateTime}");
                    }
            }
        }
    }
}
