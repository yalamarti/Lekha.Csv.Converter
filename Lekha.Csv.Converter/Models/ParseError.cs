namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represent CSV Parser error
    /// </summary>
    public class ParseError
    {
        /// <summary>
        /// Location of the parse error
        /// </summary>
        public Field Location { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }
    }
}
