using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rochas.Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

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
            var connectives = "em primeiro lugar,antes de mais nada,antes de tudo,em principio,primeiramente,acima de tudo,precipuamente,principalmente,primordialmente,sobretudo,a priori,a posteriori,";
            connectives += "entao,enfim, logo ,depois,imediatamente, apos ,a principio,no momento,em que, pouco , antes ,anteriormente,geralmente, posteriormente,em seguida,afinal,por fim,finalmente,agora,atualmente,hoje,frequentemente,constantemente,às vezes,eventualmente,por vezes,ocasionalmente,sempre,raramente,não raro,ao mesmo tempo,simultaneamente,nesse ínterim,nesse,meio tempo,hiato,";
            connectives += "enquanto,quando, que ,depois,sempre, assim, desde, todas, as vezes,cada vez que,apenas, ja , mal ,nem bem,igualmente,da mesma forma,assim também,do mesmo modo,similarmente,semelhantemente,analogamente,por analogia,de maneira,identica,conformidade, com , de acordo , segundo , conforme ,sob o mesmo, ponto de vista,tal qual,tanto quanto, como , assim como , viu ,";
            connectives += " se , por , caso,eventualmente,desde que,ainda,além disso,demais,ademais,outrossim,mais,por cima,por outro lado,também, nem , nao so ,mas tambem,como tambem,nao apenas,bem como, ou , em , do , dos , da , no , ter , seu , sua , seus , suas , das , destes , deste , destas , desta, ao , aos , na , ha ,pra ca, era , eram , um , uma ,";
            connectives += "talvez,provavelmente,possivelmente,exclusivamente,quem sabe, e provavel , nao e certo , se e que , de certo , por certo ,certamente,indubitavelmente,inquestionavelmente,sem duvida,inegavelmente, com certeza ,inesperadamente,inopinadamente,subitamente, de repente ,imprevistamente,surpreendentemente, entanto , tanto , quanto ,";
            connectives += "por exemplo,só para ilustrar,so para exemplificar, isto ,quer dizer,em outras palavras,ou por outra,a saber,ou seja,alias,com o fim de,a fim de,com o proposito de,com a finalidade de,com o intuito de, para que,a fim de que, para, como, ser, em , o , os , e , de , ele , ela , a , as ,Os ,As ,Eles ,Elas ,Ele ,Ela , nas , foi , estao , estava ";
            connectives += " perto de , proximo a , junto a , juntamente , dentro , fora , foram , mais adiante , aqui , além , la , ali , este , esta , isto , esse , essa , isso , aquele , aquela , aquilo , a ,em suma,em sintese,em conclusao,enfim, em resumo ,portanto,assim,dessa forma,dessa maneira,desse modo,logo,pois,dessarte,destarte,assim sendo, entre , vem , vai , vao ,";
            connectives += "pelo contrario,em contraste com,salvo,exceto, menos , mas , contudo ,todavia,entretanto, no entanto , embora, apesar, ainda, que,mesmo que,posto que,posto,conquanto,se bem que, por mais que , por menos que , so que , ao passo que , ou seja , a proporcao , a medida , ao passo ,quanto mais,quanto menos, em meio ao , em meio aquele , o valor de , o valor do , o valor da ";

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

        #endregion

        #region Runtime compilation methods

        public static object ExecuteAsSourceCode(this string sourceCode, string typeName, string methodName, dynamic parameter, Type[] optionalReferences = null)
        {
            var trustedPlatforms = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;

            if (trustedPlatforms == null)
            {
                throw new Exception("Não foi possível obter as referências dos assemblies confiáveis.");
            }

            var referencePaths = trustedPlatforms.Split(Path.PathSeparator);

            var references = referencePaths
                .Where(p => File.Exists(p))  // Garantir que o arquivo exista
                .Distinct()
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            references.Add(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location)); // System.Linq
            references.Add(MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location)); // System.Collections.Generic
            references.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).Assembly.Location)); // Para tipos dinâmicos
            references.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.InteropServices.Marshal).Assembly.Location)); // System.Runtime (para tipos básicos e manipulação de memória)
            references.Add(MetadataReference.CreateFromFile(typeof(System.String).Assembly.Location)); // System.Private.CoreLib (geral)
            references.Add(MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location)); // System.Private.CoreLib (geral)
            var netstandard = referencePaths.FirstOrDefault(p => Path.GetFileName(p).Equals("netstandard.dll", StringComparison.OrdinalIgnoreCase));
            if (netstandard != null)
                references.Add(MetadataReference.CreateFromFile(netstandard));

            if (optionalReferences != null)
                foreach (var optionalRef in optionalReferences)
                    references.Add(MetadataReference.CreateFromFile(optionalRef.Assembly.Location));

            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(),
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                string errors = "Compilation failed:\n";
                foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    var lineSpan = diagnostic.Location.GetLineSpan();
                    errors += $"Line {lineSpan.StartLinePosition.Line + 1}, Column {lineSpan.StartLinePosition.Character + 1}: {diagnostic.GetMessage()}\n";
                }
                throw new Exception(errors);
            }

            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());

            Type dynamicType = assembly.GetType(typeName)
                ?? throw new Exception($"Classe '{typeName}' não encontrada no código compilado.");

            object instance = Activator.CreateInstance(dynamicType)
                ?? throw new Exception($"Não foi possível criar instância da classe '{typeName}'.");

            MethodInfo method = dynamicType.GetMethod(methodName)
                ?? throw new Exception($"Método '{methodName}' não encontrado na classe '{typeName}'.");

            try
            {
                return method.Invoke(instance, new object[] { parameter });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        #endregion

        public static bool IsCPF(this string value)
        {
            int sum = 0;
            int[] multiplex1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplex2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            value = value.Trim().Replace(".", string.Empty).Replace(",", string.Empty).Replace("-", string.Empty);

            if (value.Length != 11)
                return false;

            var tempCPF = value.Substring(0, 9);

            for (int count = 0; count < 9; count++)
                sum += int.Parse(tempCPF[count].ToString()) * multiplex1[count];

            int rest = sum % 11;

            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            string digit = rest.ToString();

            tempCPF += digit;

            sum = 0;

            for (int count = 0; count < 10; count++)
                sum += int.Parse(tempCPF[count].ToString()) * multiplex2[count];

            rest = sum % 11;

            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit += rest.ToString();

            return value.EndsWith(digit);
        }

        public static string Zip(this string value)
        {
            return Compressor.ZipText(value);
        }

        public static string UnZip(this string value)
        {
            return Compressor.UnZipText(value);
        }
    }
}
