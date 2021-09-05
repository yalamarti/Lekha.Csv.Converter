namespace Lekha.Csv.Converter.Models
{
    internal class FieldConfigurationDto
    {
        public FieldConfiguration Configuration { get; set; }
        public string SanitizedFieldName { get; set; }
        public FieldConfigurationSource FieldConfigSource { get; set; }
    }
}
