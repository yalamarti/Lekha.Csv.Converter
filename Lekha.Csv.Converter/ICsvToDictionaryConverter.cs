﻿using Lekha.Csv.Converter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lekha.Csv.Converter
{
    /// <summary>
    /// Exposes the CSV convertion functionality
    /// </summary>
    public interface ICsvToDictionaryConverter
    {
        /// <summary>
        /// Converts CSV from the specified stream to a Dictionary<string, object>.
        /// Uses default ConverterConfiguration. 
        /// Refer to ConverterConfiguration class defintion for more details on default behavior.
        /// </summary>
        /// <param name="stream">CSV source stream</param>
        /// <param name="processedRecordCallback">Callback for handling a converted CSV record</param>
        /// <param name="errorRecordCallback">Callback for handling error encounted when converting a CSV record</param>
        /// <seealso cref="ConverterConfiguration"/>
        /// <returns></returns>
        Task<ConversionResult> ConvertAsync(Stream stream, Func<long, Dictionary<string, object>, Task> processedRecordCallback, Func<ParseError, Task<bool>> errorCallback);

        /// <summary>
        /// Converts CSV from the specified stream to a Dictionary<string, object>
        /// </summary>
        /// <param name="stream">CSV source stream</param>
        /// <param name="configuration">Configuration to apply when converting from CSV</param>
        /// <param name="processedRecordCallback">Callback for handling a converted CSV record</param>
        /// <param name="errorRecordCallback">Callback for handling error encounted when converting a CSV record.  
        /// To contiue with processing remaining records, return a true from this callback.</param>
        /// <returns></returns>
        Task<ConversionResult> ConvertAsync(Stream stream, ConverterConfiguration configuration,
            Func<long, Dictionary<string, object>, Task> processedRecordCallback, Func<ParseError, Task<bool>> errorRecordCallback);
    }
}
