using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using Rochas.Extensions.Helpers;

namespace Rochas.Extensions
{
    public static class StringExtension
    {
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

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        public static byte[] FromBase64String(this string value)
        {
            return Convert.FromBase64String(value);
        }

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
