using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLQueryStress
{
    public class SqlSentencesReader
    {
        /// <summary>
        /// Lee un archivo de texto con sentencias SQL separadas por líneas vacías
        /// y las envuelve en transacciones
        /// </summary>
        /// <param name="filePath">Ruta del archivo a leer</param>
        /// <returns>Arreglo de strings con las sentencias SQL envueltas en transacciones</returns>
        public static string[] ReadSqlSentencesWithTransactions(string filePath)
        {
            try
            {
                // Verificar si el archivo existe
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"El archivo {filePath} no fue encontrado.");
                }

                // Leer todo el contenido del archivo
                string fileContent = File.ReadAllText(filePath);

                // Dividir por dos o más saltos de línea (considerando diferentes formatos: \n, \r\n, \r)
                string[] sentences = fileContent.Split(new[] { "\n\n", "\r\n\r\n", "\r\r" },
                                                   StringSplitOptions.RemoveEmptyEntries)
                                            .Select(s => s.Trim())
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => FormatWithTransaction(s))
                                            .ToArray();

                return sentences;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer el archivo de sentencias SQL: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Versión asíncrona para leer sentencias SQL separadas por líneas vacías
        /// </summary>
        public static async Task<string[]> ReadSqlSentencesWithTransactionsAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"El archivo {filePath} no fue encontrado.");
                }

                string fileContent = await File.ReadAllTextAsync(filePath);

                string[] sentences = fileContent.Split(new[] { "\n\n", "\r\n\r\n", "\r\r" },
                                                   StringSplitOptions.RemoveEmptyEntries)
                                            .Select(s => s.Trim())
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => FormatWithTransaction(s))
                                            .ToArray();

                return sentences;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer el archivo de sentencias SQL: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Formatea una sentencia SQL con bloques de transacción
        /// </summary>
        private static string FormatWithTransaction(string sqlSentence)
        {
            // Eliminar BEGIN/COMMIT/ROLLBACK existentes para evitar anidación
            string cleanedSentence = Regex.Replace(sqlSentence,
                                                @"^\s*(BEGIN|COMMIT|ROLLBACK).*?;?\s*$",
                                                "",
                                                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            cleanedSentence = cleanedSentence.Trim();

            // Construir el bloque de transacción
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN TRANSACTION;");
            sb.AppendLine(cleanedSentence);
            sb.Append("ROLLBACK TRANSACTION;");

            return sb.ToString();
        }

        /// <summary>
        /// Une las sentencias con dos saltos de línea entre ellas
        /// </summary>
        public static string JoinWithDoubleNewLines(string[] sentences)
        {
            return string.Join("\n\n", sentences);
        }
    }


}
