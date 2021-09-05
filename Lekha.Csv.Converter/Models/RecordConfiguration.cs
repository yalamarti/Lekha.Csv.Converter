using System.Collections.Generic;

namespace Lekha.Csv.Converter.Models
{
    /// <summary>
    /// Represents the configuration of a record within the CSV data that is being converted
    /// </summary>
    public class RecordConfiguration
    {
        /// <summary>
        /// Field delimiter within a record.
        /// Default: comma ","
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Field configuration specification for the fields in a record
        /// </summary>
        public List<FieldConfiguration> Fields { get; set; }
    }

}
