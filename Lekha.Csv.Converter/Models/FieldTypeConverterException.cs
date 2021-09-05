using System;

namespace Lekha.Csv.Converter.Models
{
    public class FieldTypeConverterException : Exception
    {
        public FieldTypeConverterException(string message) : base(message)
        {
        }
    }
}
