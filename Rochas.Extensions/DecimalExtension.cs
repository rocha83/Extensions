using System;
using System.Globalization;

namespace Rochas.Extensions
{
    public static class DecimalExtension
    {
        public static string ToCultureString(this decimal value, string culture)
        {
            return value.ToString("N2", CultureInfo.GetCultureInfo(culture));
        }

        public static string ToInFullText(this decimal value)
        {
            if (value <= 0 | value >= 1000000000000000)
                return "Invalid value informed.";
            else
            {
                string strValue = value.ToString("000000000000000.00");
                string inFullValue = string.Empty;

                for (int i = 0; i <= 15; i += 3)
                {
                    inFullValue += WriteInFullTextValue(Convert.ToDecimal(strValue.Substring(i, 3)));

                    if (i == 0 & inFullValue != string.Empty)
                    {
                        if (Convert.ToInt32(strValue.Substring(0, 3)) == 1)
                            inFullValue += " TRILHÃO" + ((Convert.ToDecimal(strValue.Substring(3, 12)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValue.Substring(0, 3)) > 1)
                            inFullValue += " TRILHÕES" + ((Convert.ToDecimal(strValue.Substring(3, 12)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 3 & inFullValue != string.Empty)
                    {
                        if (Convert.ToInt32(strValue.Substring(3, 3)) == 1)
                            inFullValue += " BILHÃO" + ((Convert.ToDecimal(strValue.Substring(6, 9)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValue.Substring(3, 3)) > 1)
                            inFullValue += " BILHÕES" + ((Convert.ToDecimal(strValue.Substring(6, 9)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 6 & inFullValue != string.Empty)
                    {
                        if (Convert.ToInt32(strValue.Substring(6, 3)) == 1)
                            inFullValue += " MILHÃO" + ((Convert.ToDecimal(strValue.Substring(9, 6)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValue.Substring(6, 3)) > 1)
                            inFullValue += " MILHÕES" + ((Convert.ToDecimal(strValue.Substring(9, 6)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 9 & inFullValue != string.Empty)
                        if (Convert.ToInt32(strValue.Substring(9, 3)) > 0)
                            inFullValue += " MIL" + ((Convert.ToDecimal(strValue.Substring(12, 3)) > 0) ? " E " : string.Empty);

                    if (i == 12)
                    {
                        if (inFullValue.Length > 8)
                            if (inFullValue.Substring(inFullValue.Length - 6, 6) == "BILHÃO" | inFullValue.Substring(inFullValue.Length - 6, 6) == "MILHÃO")
                                inFullValue += " DE";
                            else
                                if (inFullValue.Substring(inFullValue.Length - 7, 7) == "BILHÕES" | inFullValue.Substring(inFullValue.Length - 7, 7) == "MILHÕES" | inFullValue.Substring(inFullValue.Length - 8, 7) == "TRILHÕES")
                                inFullValue += " DE";
                            else
                                    if (inFullValue.Substring(inFullValue.Length - 8, 8) == "TRILHÕES")
                                inFullValue += " DE";

                        if (Convert.ToInt64(strValue.Substring(0, 15)) == 1)
                            inFullValue += " REAL";
                        else if (Convert.ToInt64(strValue.Substring(0, 15)) > 1)
                            inFullValue += " REAIS";

                        if (Convert.ToInt32(strValue.Substring(16, 2)) > 0 && inFullValue != string.Empty)
                            inFullValue += " E ";
                    }

                    if (i == 15)
                        if (Convert.ToInt32(strValue.Substring(16, 2)) == 1)
                            inFullValue += " CENTAVO";
                        else if (Convert.ToInt32(strValue.Substring(16, 2)) > 1)
                            inFullValue += " CENTAVOS";
                }
                return inFullValue;
            }

            string WriteInFullTextValue(decimal value)
            {
                if (value <= 0)
                    return string.Empty;
                else
                {
                    string mounting = string.Empty;
                    if (value > 0 & value < 1)
                    {
                        value *= 100;
                    }
                    string strValue = value.ToString("000");
                    int a = Convert.ToInt32(strValue.Substring(0, 1));
                    int b = Convert.ToInt32(strValue.Substring(1, 1));
                    int c = Convert.ToInt32(strValue.Substring(2, 1));

                    if (a == 1) mounting += (b + c == 0) ? "CEM" : "CENTO";
                    else if (a == 2) mounting += "DUZENTOS";
                    else if (a == 3) mounting += "TREZENTOS";
                    else if (a == 4) mounting += "QUATROCENTOS";
                    else if (a == 5) mounting += "QUINHENTOS";
                    else if (a == 6) mounting += "SEISCENTOS";
                    else if (a == 7) mounting += "SETECENTOS";
                    else if (a == 8) mounting += "OITOCENTOS";
                    else if (a == 9) mounting += "NOVECENTOS";

                    if (b == 1)
                    {
                        if (c == 0) mounting += ((a > 0) ? " E " : string.Empty) + "DEZ";
                        else if (c == 1) mounting += ((a > 0) ? " E " : string.Empty) + "ONZE";
                        else if (c == 2) mounting += ((a > 0) ? " E " : string.Empty) + "DOZE";
                        else if (c == 3) mounting += ((a > 0) ? " E " : string.Empty) + "TREZE";
                        else if (c == 4) mounting += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                        else if (c == 5) mounting += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                        else if (c == 6) mounting += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                        else if (c == 7) mounting += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                        else if (c == 8) mounting += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                        else if (c == 9) mounting += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                    }
                    else if (b == 2) mounting += ((a > 0) ? " E " : string.Empty) + "VINTE";
                    else if (b == 3) mounting += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                    else if (b == 4) mounting += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                    else if (b == 5) mounting += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                    else if (b == 6) mounting += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                    else if (b == 7) mounting += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                    else if (b == 8) mounting += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                    else if (b == 9) mounting += ((a > 0) ? " E " : string.Empty) + "NOVENTA";

                    if (strValue.Substring(1, 1) != "1" & c != 0 & mounting != string.Empty) mounting += " E ";

                    if (strValue.Substring(1, 1) != "1")
                        if (c == 1) mounting += "UM";
                        else if (c == 2) mounting += "DOIS";
                        else if (c == 3) mounting += "TRÊS";
                        else if (c == 4) mounting += "QUATRO";
                        else if (c == 5) mounting += "CINCO";
                        else if (c == 6) mounting += "SEIS";
                        else if (c == 7) mounting += "SETE";
                        else if (c == 8) mounting += "OITO";
                        else if (c == 9) mounting += "NOVE";

                    return mounting;
                }
            }
        }
    }
}
