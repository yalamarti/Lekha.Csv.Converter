using CsvHelper.Configuration;
using System.Globalization;
using System;

namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represents the configuration for the CSV converter
    /// </summary>
    public record CsvConverterConfiguration : CsvConfiguration
    {
        public CsvConverterConfiguration()
            : base(CultureInfo.InvariantCulture)
        {

        }

        public CsvConverterConfiguration(CultureInfo cultureInfo, Type? attributesType = null)
            : base(cultureInfo, attributesType)
        {

        }

        /// <summary>
        /// Field name prefix.  Applicable when no field names are specified
        /// Default: Defined by FieldPrefix.Default constant
        /// <seealso cref="FieldPrefix.Default"/>
        /// </summary>
        public string FieldNamePrefix { get; set; } = FieldPrefix.Default;

        /// <summary>
        /// Configuration for records that are part of the CSV data being converted.
        /// Optional.
        /// Default: Refer to RecordConfiguration class, for defaults
        /// </summary>
        public RecordConfiguration RecordConfiguration { get; set; }
    }


}
