namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Default field formats
    /// </summary>
    public struct DefaultFormat
    {
        public const string Date = "yyyy/MM/dd";
        public const string Time = "hh\\:mm"; // not HH intentionally ... it is hh as the implementaiton uses TimeSpan
        public const string DateTime = "yyyy/MM/dd HH:mm";
    }
}
