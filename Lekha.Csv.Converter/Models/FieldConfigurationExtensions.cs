namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Field Configuration Extenions
    /// </summary>
    internal static class FieldConfigurationExtensions
    {
        /// <summary>
        /// Message string used for reporting the value of the CSV field
        /// </summary>
        /// <param name="fieldConfiguration"></param>
        /// <param name="fieldTextualValue">Textual representation of the field</param>
        /// <returns></returns>
        public static string ToMessage(this FieldConfiguration fieldConfiguration, string fieldTextualValue)
        {
            return fieldConfiguration == null ? null : fieldConfiguration.ExposeableToPublic ? $"value:{fieldTextualValue}" : $"value of field:{fieldConfiguration.Name}";
        }
    }
}
