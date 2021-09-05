namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Data types supported for CSV fields
    /// </summary>
    public struct FieldType
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

        public const string Time = "time";
    }

}
