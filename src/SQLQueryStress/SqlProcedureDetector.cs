using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLQueryStress
{
    internal class SqlProcedureDetector
    {
        /// <summary>
        /// Detecta y extrae el nombre completo de un procedimiento almacenado (esquema.nombre) 
        /// de una cadena SQL que contenga EXEC o EXECUTE
        /// </summary>
        /// <param name="sqlText">Cadena SQL a analizar</param>
        /// <returns>Nombre completo del procedimiento (esquema.procedimiento) o string vacío si no se encuentra</returns>
        public static string DetectStoredProcedure(string sqlText)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                return string.Empty;

            // Patrón para buscar EXEC/EXECUTE seguido de [esquema].[procedimiento] o esquema.procedimiento
            string pattern = @"\b(?:EXEC|EXECUTE)\s+(?:\[?([a-zA-Z_][a-zA-Z0-9_]*)\]?\.\[?([a-zA-Z_][a-zA-Z0-9_]*)\]?|\b(?:EXEC|EXECUTE)\s+\[?([a-zA-Z_][a-zA-Z0-9_]*)\]?)";

            var match = Regex.Match(sqlText, pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
                return string.Empty;

            // Determinar si tiene esquema o no
            if (!string.IsNullOrEmpty(match.Groups[1].Value) && !string.IsNullOrEmpty(match.Groups[2].Value))
            {
                // Caso con esquema: esquema.procedimiento
                return $"{match.Groups[1].Value}.{match.Groups[2].Value}";
            }
            else if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                // Caso sin esquema: solo nombre del procedimiento
                return match.Groups[3].Value;
            }

            return string.Empty;
        }
    }
}
