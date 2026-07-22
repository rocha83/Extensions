using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Rochas.Extensions.Helpers;

namespace Rochas.Extensions
{
    public static class StringExtension
    {

        #region Serialization methods

        public static object? FromJson(this string value)
        {
            return JsonSerializer.Deserialize<object>(value);
        }

        public static T FromJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        public static T FromXML<T>(this string value, string rootElem)
        {
            T result = default;

            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElem));

            using (var reader = new StringReader(value))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }
        public static byte[] FromBase64String(this string value)
        {
            return Convert.FromBase64String(value);
        }

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        #endregion

        #region Data mining methods

        private static readonly HashSet<string> StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "ao", "aos", "as", "o", "os", "da", "do", "das", "dos", "no", "na", "nas", "nos",
            "de", "por", "para", "pra", "com", "sem", "sobre", "entre", "ate", "apos", "em", "onde",
            "quando", "que", "se", "pois", "como", "ou", "mas", "porem", "todavia", "entretanto",
            "entao", "logo", "assim", "portanto", "primeiro", "primeiramente", "sobretudo", "acima",
            "antes", "depois", "anteriormente", "posteriormente", "seguida", "agora", "atualmente",
            "hoje", "sempre", "raramente", "vezes", "frequentemente", "constantemente", "eventualmente",
            "ocasionalmente", "enquanto", "desde", "apenas", "ja", "mal", "quase", "nem", "bem",
            "igualmente", "similarmente", "analogamente", "conforme", "segundo", "consoante",
            "ainda", "ademais", "outrossim", "tambem", "talvez", "provavelmente", "possivelmente",
            "certamente", "porque", "porquanto", "porisso", "embora", "contudo", "perto", "proximo",
            "junto", "juntamente", "dentro", "fora", "aqui", "ali", "la", "este", "esta", "isto",
            "esse", "essa", "isso", "aquele", "aquela", "aquilo", "vem", "vai", "vao", "foi",
            "estao", "estava", "foram", "ser", "estar", "ter", "valor", "qual", "quais", "um",
            "uma", "uns", "umas", "ele", "ela", "eles", "elas", "meu", "minha", "meus", "minhas",
            "seu", "sua", "seus", "suas", "nosso", "nossa", "nossos", "nossas", "teu", "tua",
            "teus", "tuas", "dele", "dela", "deles", "delas", "eu", "tu", "voce", "voces", "nos",
            "lhe", "lhes", "e", "exemplo", "forma", "modo", "dessa", "deste", "desse", "mesma",
            "acordo", "conformidade", "disso", "certeza", "quer", "dizer", "seja", "saber",
            "fim", "finalidade", "intuito", "apesar", "posto", "passo", "suma", "sintese",
            "resumo", "conclusao", "enfim"
        };

        public static string FilterSpecialChars(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // Fold all diacritics / accents using FormD (supports all PT-BR accents: àáãçéêíóõôúâÂÊ, etc.)
            var normalized = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    if (char.IsLetterOrDigit(ch))
                        sb.Append(ch);
                    else
                        sb.Append(' ');
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        public static string RemoveTextConnectives(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var clean = value.FilterSpecialChars();
            var words = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var filtered = words.Where(w => !StopWords.Contains(w));

            return string.Join(' ', filtered);
        }

        public static string[]? Tokenize(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var clean = value.FilterSpecialChars();
            var words = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var tokens = words
                .Where(w => !StopWords.Contains(w) && !w.Equals("-"))
                .ToArray();

            return tokens;
        }

        public static string ToNormalizedDescription(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var descArray = value.Split('\n').ToList();
            descArray.RemoveAll(ln => string.IsNullOrWhiteSpace(ln.Trim()));
            var newArray = descArray.ToList();

            if (descArray.Any(ln => !ln.Contains(':')))
            {
                var count = 0;
                foreach (var item in newArray)
                {
                    var cleanedItem = item.Trim().Replace("\r", string.Empty)
                                                 .Replace("\n", string.Empty);
                    if (!cleanedItem.EndsWith(':'))
                    {
                        var descItem = descArray[count].Trim().Replace("\r", string.Empty)
                                                              .Replace("\n", string.Empty);
                        if (descItem.Contains(':') && !cleanedItem.Contains(':'))
                        {
                            descArray[count] = $"{descItem} {cleanedItem}\n";
                            descArray.Remove(item);
                        }
                        else
                            descArray[count] = $"{cleanedItem}\n";

                        count++;
                    }
                }
            }
            else
            {
                var count = 0;
                foreach (var item in newArray)
                {
                    descArray[count] = descArray[count].Trim().Replace("\r", string.Empty)
                                                              .Replace("\n", string.Empty);
                    descArray[count] += "\n";
                    count++;
                }
            }

            return string.Join('\n', descArray);
        }

        public static bool HasNumber(this string value)
        {
            var numbers = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            return value.Any(c => numbers.Contains(c));
        }
                
        public static uint GetCustomHashCode(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            //Implementação do FNV-1a 32bits

            const uint prime = 16777619;
            uint hash = 2166136261;

            foreach (char c in value)
            {
                hash ^= (byte)c;
                hash *= prime;
            }

            return hash;
        }


        #endregion

        #region Runtime code execution methods

        public static async Task<object?> ExecuteAsSourceCode(this string sourceCode, object globals, Type[] allowedTypes,
                                                              int timeoutMs = 300, CancellationToken externalCancellation = default)
        {
            if (allowedTypes == null || allowedTypes.Length == 0)
                throw new ArgumentException("Allowed types is required.");

            var allowedAssemblies = allowedTypes.Select(t => t.Assembly)
                                                .Concat(new[] { typeof(object).Assembly })  // System.Private.CoreLib
                                                .Concat(new[] { typeof(Console).Assembly }) // System.Console
                                                .Distinct().ToArray();

            var allowedNamespaces = allowedTypes
                .Select(t => t.Namespace)
                .Where(ns => ns != null)
                .Distinct()
                .ToList();

            allowedNamespaces.Add("System");

            var refs = allowedAssemblies
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToArray();

            var scriptOptions = ScriptOptions.Default
                .WithReferences(refs)
                .WithImports(allowedNamespaces)
                .WithAllowUnsafe(false);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, externalCancellation);
                        
            try
            {
                var script = CSharpScript.Create(sourceCode, scriptOptions, globals.GetType());
                var runner = script.CreateDelegate();
                return await runner(globals, linkedCts.Token);
            }
            catch (CompilationErrorException ex)
            {
                throw new Exception("Script compilation error:\n" + string.Join("\n", ex.Diagnostics));
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Execution time exceeded {timeoutMs}ms.");
            }
        }

        #endregion

        public static bool IsCPFOrCNPJ(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Normaliza
            value = value.Trim()
                         .Replace(".", "")
                         .Replace("-", "")
                         .Replace("/", "")
                         .Replace(",", "");

            // -------------------------------------------
            // CPF (11 dígitos)
            // -------------------------------------------
            if (value.Length == 11)
            {
                // Evita CPFs inválidos conhecidos (11111111111, 2222222..., etc.)
                if (new string(value[0], 11) == value)
                    return false;

                int sum = 0;
                int[] m1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] m2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                // 1º dígito
                for (int i = 0; i < 9; i++)
                    sum += (value[i] - '0') * m1[i];

                int rest = sum % 11;
                int d1 = rest < 2 ? 0 : 11 - rest;

                // 2º dígito
                sum = 0;
                for (int i = 0; i < 10; i++)
                    sum += (value[i] - '0') * m2[i];

                rest = sum % 11;
                int d2 = rest < 2 ? 0 : 11 - rest;

                return value[9] - '0' == d1 && value[10] - '0' == d2;
            }

            // -------------------------------------------
            // CNPJ (14 dígitos)
            // -------------------------------------------
            if (value.Length == 14)
            {
                // bloqueia CNPJs repetidos (000000..., 111111..., etc)
                if (new string(value[0], 14) == value)
                    return false;

                int sum = 0;
                int[] m1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] m2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

                // 1º dígito
                for (int i = 0; i < 12; i++)
                    sum += (value[i] - '0') * m1[i];

                int rest = sum % 11;
                int d1 = rest < 2 ? 0 : 11 - rest;

                // 2º dígito
                sum = 0;
                for (int i = 0; i < 13; i++)
                    sum += (value[i] - '0') * m2[i];

                rest = sum % 11;
                int d2 = rest < 2 ? 0 : 11 - rest;

                return value[12] - '0' == d1 && value[13] - '0' == d2;
            }

            return false;
        }

        #region Data compression methods
        public static string Zip(this string value)
        {
            return Compressor.ZipText(value);
        }

        public static string UnZip(this string value)
        {
            return Compressor.UnZipText(value);
        }

        #endregion
    }
}
