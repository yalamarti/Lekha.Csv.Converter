using System;
using System.Collections.Generic;
using System.Linq;

namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represents the result of the CSV conversion
    /// </summary>
    public class ConversionResult
    {
        /// <summary>
        /// Resolved configuration used during conversion.
        /// </summary>
        [Obsolete("Use CsvConfiguration property instead")]
        public ConverterConfiguration Configuration { get; set; }
        /// <summary>
        /// Resolved configuration used during conversion.
        /// </summary>
        public CsvConverterConfiguration CsvConfiguration { get; set; }

        /// <summary>
        /// Conversion successfully completed or failed.
        /// true - success; false - failure
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Success or failure message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Total CSV records processed
        /// </summary>
        public long ProcessedRecordCount { get; set; }

        /// <summary>
        /// Total CSV records with errors
        /// </summary>
        public long ErrorRecordCount => Errors == null ? 0 : Errors.LongCount();

        /// <summary>
        /// Headers resolved from the CSV data
        /// </summary>
        public string[] FoundHeaders { get; set; }

        /// <summary>
        /// Expected header names, in case of a 'missing headers' errors.
        /// </summary>
        public string[] ExpectedHeaders { get; set; }
        public string[] DuplicateFields { get; set; }
        public string[] MissingHeaderFields { get; set; }

        public List<string> Observations { get; set; }

        /// <summary>
        /// Field Configurations that were determined from the CSV stream
        /// </summary>
        public List<ParseError> Errors { get; set; } = new List<ParseError>();
    }

}
