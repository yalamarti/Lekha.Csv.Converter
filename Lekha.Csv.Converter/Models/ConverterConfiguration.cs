namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represents the configuration for the CSV converter
    /// </summary>
    public class ConverterConfiguration
    {
        /// <summary>
        /// Indicates if the CSV being converted has a header record.
        /// Optional.
        /// Default: false, indicating no header record
        /// </summary>
        public bool? HasHeaderRecord { get; set; }

        /// <summary>
        /// Indicates the comment character to be used.  
        /// Any line that starts with this character is considered a comment line.
        /// Optional.
        /// Default: empty, indicting the CSV represented by this configuration has no comment lines.  
        /// </summary>
        public char? CommentCharacter { get; set; }

        /// <summary>
        /// Configuration for records that are part of the CSV data being converted.
        /// Optional.
        /// Default: Refer to RecordConfiguration class, for defaults
        /// </summary>
        public RecordConfiguration RecordConfiguration { get; set; }
    }

}
