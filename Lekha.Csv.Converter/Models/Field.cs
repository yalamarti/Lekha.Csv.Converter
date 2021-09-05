namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represents a CSV field specification for the parser result
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 1-based index of the record.
        /// </summary>
        public long? RecordIndex { get; set; }

        /// <summary>
        /// 1-based index of the field
        /// </summary>
        public int? FieldIndex { get; set; }
    }
}
