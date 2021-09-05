using System;

namespace Lekha.Csv.Converter.Models
{
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}
