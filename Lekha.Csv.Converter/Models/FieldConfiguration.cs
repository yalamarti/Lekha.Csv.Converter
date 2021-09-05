namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represents the configuration of a field within record of CSV data
    /// </summary>
    public class FieldConfiguration
    {
        /// <summary>
        /// Name of the field.  
        /// Optional. 
        /// When not specified, implies name from HeaderRecords or a system generated name.
        /// Case insensitive: (meaning case of the letters in the name doesn't matter when comparing with, 
        /// say, comparing the field name to a corresponding field header in the CSV data)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A displayable title for the field.  Optional. Default: Same as Name value.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Data type of the field.
        /// Valid values: number, unsigned-number, decimal, date, datetime, time, string.
        /// Optional.
        /// Default: string
        ///   https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types: 
        ///      Number:   long      System.Int64    Size: Signed 64-bit integer
        ///         Range: -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        ///         
        ///      Unsigned-Number:   ulong      System.UInt64    Size: Unsigned Signed 64-bit integer
        ///         Range: 0 to 18,446,744,073,709,551,615
        ///         
        ///      Decimal : decimal 	System.Decimal 	Size: 16 bytes 	
        ///         Precision: - 28-29 decimal places 	(28-29: includes significant digits and decimal places)
        ///         Range:     +-1.0 x 10 power 28 to +-7.9 x 10 power 28
        ///         
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Format of the date, datetime or time field value.  
        /// Optional.
        /// Default: For date - 'yyyy/MM/dd'.  
        /// Default: For time: 'HH\:mm'.  For datetime: 'yyyy/MM/dd HH:mm' 
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Indicates if the field value can be empty or blank/spaces
        /// Optional.
        /// Default: false
        /// </summary>
        public bool AllowEmptyField { get; set; }

        /// <summary>
        /// Indicates if the field value has to be specified as part of the record.
        /// Optional.
        /// Applies when a header record is specified.
        /// When value is 'true', in case the field value is missing in a record, the record will be marked as 'in error'
        /// Default: false
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Maximum allowed length of the field
        /// Default: Refer to FieldLimits.MaximumLength
        /// </summary>
        public int AllowedMaximumLength { get; set; } = FieldLimits.MaximumLength;

        /// <summary>
        /// Indicates if the value of the field can be part of exception/logging reporting
        /// Default: falsel
        /// </summary>
        public bool ExposeableToPublic { get; set; }
    }
}
