using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        public static string FilterSpecialChars(this string value)
        {
            int charCount = 0;
            var specialChars = ".:,;|/()[]'+—=!?";
            var fromChars = "àáãçéêíóõôúÀÁÃÇÉÍÓÔÕÚ";
            var toChars = "aaaceeiooouAAACEIOOOU";

            foreach (var character in specialChars)
                value = value.Replace(character, ' ');

            foreach (var character in fromChars)
                value = value.Replace(character, toChars[charCount++]);

            return value.Replace("\\", string.Empty)
                        .Replace("\"", string.Empty)
                        .Replace("\n", " ").Trim();
        }

        public static string RemoveTextConnectives(this string value)
        {
            var connectives = @"
            a,ao,aos,à,às,as,o,os,da,do,das,dos,no,na,nas,nos,
            de,por,para,pra,com,sem,sobre,entre,ate,apos,após,ate,
            em,ao,onde,quando,que,se,pois,como,ou,mas,porém,todavia,entretanto,
            entao,então,logo,assim,portanto,dessa forma,deste modo,desse modo,
            primeiro,primeiramente,sobretudo,acima de tudo,antes,depois,
            anteriormente,posteriormente,em seguida,agora,atualmente,hoje,
            sempre,raramente,às vezes,vezes,frequentemente,constantemente,
            eventualmente,ocasionalmente,enquanto,desde,desde que,
            apenas,ja,mal,quase,nem,bem,
            igualmente,da mesma forma,do mesmo modo,similarmente,analogamente,
            conforme,segundo,consoante,de acordo,em conformidade,
            tal qual,tanto quanto,assim como,
            ainda,alem disso,ademais,outrossim,tambem,nao so,mas tambem,
            bem como,como tambem,nao apenas,
            talvez,provavelmente,possivelmente,certamente,com certeza,
            por exemplo,isto e,quer dizer,ou seja,a saber,
            para que,a fim de,com o fim de,com a finalidade de,com o intuito de,
            porque,porquanto,por isso,
            porem,contudo,todavia,entretanto,no entanto,
            embora,apesar de,mesmo que,posto que,conquanto,se bem que,
            por mais que,por menos que,ao passo que,enquanto que,
            em suma,em sintese,em resumo,em conclusao,enfim,
            perto,proximo,junto,juntamente,dentro,fora,aqui,ali,la,
            este,esta,isto,esse,essa,isso,aquele,aquela,aquilo,
            vem,vai,vao,foi,estao,estava,foram,ser,estar,ter,
            valor,valor de,valor do,valor da";

            var connectiveArray = connectives.Split(',').ToList();
            foreach (var connective in connectiveArray)
                value = value.Replace(connective, " ");

            if (!string.IsNullOrWhiteSpace(value))
                value = value.ToLower();

            foreach (var connective in connectiveArray)
                value = value.Replace(connective, " ");

            return value.Trim();
        }

        public static string[] Tokenize(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var result = value.FilterSpecialChars().RemoveTextConnectives().Split(' ');

            return result.Where(c => !string.IsNullOrWhiteSpace(c.Trim()) && !c.Equals("-")).ToArray();
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

        public static async Task<object?> ExecuteAsSourceCode(string sourceCode, object globals, Type[] allowedTypes,
                                                              int timeoutMs = 300, CancellationToken externalCancellation = default)
        {
            if (allowedTypes == null || allowedTypes.Length == 0)
                throw new ArgumentException("É necessário informar pelo menos um tipo permitido.");

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
                throw new Exception("Erro ao compilar script:\n" + string.Join("\n", ex.Diagnostics));
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Execução excedeu {timeoutMs}ms.");
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
