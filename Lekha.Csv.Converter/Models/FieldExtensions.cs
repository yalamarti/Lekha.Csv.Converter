namespace Lekha.Csv.Converter.Models
{
    internal static class FieldExtensions
    {
        public static string ToSanitizedFieldName(this string fieldName)
        {
            return fieldName.Trim().ToLower();
        }
    }

}
