// ===============================================================================
// This file is part of Ecopath with Ecosim (EwE)
//
// EwE is free software: you can redistribute it and/or modify it under the terms
// of the GNU General Public License version 2 as published by the Free Software 
// Foundation.
//
// EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
// PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with EwE.
// If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
//
//
// Copyright 1991- 
//    Ecopath International Initiative, Barcelona, Spain
// ===============================================================================

#region  Imports 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace ValueChain
{

    #endregion

    /// ---------------------------------------------------------------------------
/// <summary>
/// Class offering string utilities.
/// </summary>
/// ---------------------------------------------------------------------------
    internal class cStringUtils
    {

        /// <summary><para>If true, CSV formatting is more restrictive than usual.
    /// <list type="bullet"><item>headers will 
    /// only be allowed to contain characters, numbers and underscores. All 
    /// characters not matching this criteria will be replaced by underscores. 
    /// Tools such as ArcGIS require this type of CSV formatting.</item>
    /// </list>
    /// </para>
    /// </summary>
        public static bool StrictCSVFormatting { get; set; } = false;

        /// ---------------------------------------------------------------------------
    /// <summary>
    /// Split function that supports text qualifiers. Code adapted from Larry Steinly,
    /// http://www.codeproject.com/Articles/15361/Split-Function-that-Supports-Text-Qualifiers.
    /// </summary>
    /// <param name="strExpression">String to split.</param>
    /// <param name="strDelimiter">Delimiting character to split by.</param>
    /// <param name="strQualifier">String qualifier, such as single or double quotes. Qualified string
    /// segments will not be subdivided by delimiting characters.</param>
    /// <returns>An array of strings.</returns>
    /// ---------------------------------------------------------------------------
        public static string[] SplitQualified(string strExpression, string strDelimiter, string strQualifier = "\"")
        {

            // Sanity check
            if (string.IsNullOrEmpty(strExpression))
                return new string[] { string.Empty };

            // Ensure defaults. A whitespace delimiter is allowed!
            if (string.IsNullOrEmpty(strDelimiter))
                strDelimiter = ",";
            if (string.IsNullOrWhiteSpace(strQualifier))
                strQualifier = "\"";

            bool bQualifier = false;
            int iStart = 0;
            var lValues = new List<string>();
            int iQL = strQualifier.Length;
            int iDL = strDelimiter.Length;
            string strVal = "";

            for (int iChar = 0, loopTo = strExpression.Length - 1; iChar <= loopTo; iChar++)
            {
                if (string.Compare(strExpression.Substring(iChar, iQL), strQualifier, true) == 0)
                {
                    bQualifier = !bQualifier;
                }
                else if (!bQualifier & string.Compare(strExpression.Substring(iChar, strDelimiter.Length), strDelimiter, true) == 0)
                {
                    // Crop leading and trainling delimiter
                    strVal = strExpression.Substring(iStart, iChar - iStart);
                    if (strVal.StartsWith(strQualifier))
                        strVal = strVal.Substring(iQL);
                    if (strVal.EndsWith(strQualifier))
                        strVal = strVal.Substring(0, strVal.Length - iQL);
                    lValues.Add(strVal);
                    iStart = iChar + 1;
                }
            }

            if (iStart < strExpression.Length)
            {
                // Crop leading and trainling delimiter
                strVal = strExpression.Substring(iStart);
                if (strVal.StartsWith(strQualifier))
                    strVal = strVal.Substring(iQL);
                if (strVal.EndsWith(strQualifier))
                    strVal = strVal.Substring(0, strVal.Length - iQL);
                lValues.Add(strVal);
            }

            return lValues.ToArray();

        }

        /// ---------------------------------------------------------------------------
    /// <summary>
    /// Split function that supports text qualifiers.
    /// </summary>
    /// <param name="strExpression">String to split.</param>
    /// <param name="cDelimiter">Delimiting character to split by.</param>
    /// <param name="cQualifier">String qualifier, such as single or double quotes. Qualified string
    /// segments will not be subdivided by delimiting characters.</param>
    /// <returns>An array of strings.</returns>
    /// <remarks>
    /// <para>REgEx splitting is too slow. Replaced by a self-written, much faster method.</para>
    /// <para>Support for "" to indicate " is needed!</para>
    /// </remarks>
    /// ---------------------------------------------------------------------------
        public static string[] SplitQualified(string strExpression, char cDelimiter, char cQualifier = '"')
        {
            return SplitQualified(strExpression, cDelimiter.ToString(), cQualifier.ToString());
        }


        /// -------------------------------------------------------------------
    /// <summary>
    /// Truncate a string to make sure that it does not exceed a given number
    /// of characters.
    /// </summary>
    /// <param name="strIn">The string to truncate.</param>
    /// <param name="iStart">The start index to extract data from</param>
    /// <param name="iNumChars">The maximum length of the output string.</param>
    /// <returns>A string of no more than <paramref name="iNumChars"/> 
    /// characters in length.</returns>
    /// -------------------------------------------------------------------
        public static string SubString(string strIn, int iStart, int iNumChars)
        {
            return strIn.Substring(iStart, Math.Max(0, Math.Min(strIn.Length - iStart, iNumChars - iStart)));
        }


        private static string CheckNaN(string strNumber)
        {
            switch (strNumber.Trim().ToLower() ?? "")
            {
                case "-":
                case "_":
                case "nan":
                case "nan(ind)":
                case "-nan":
                case "-nan(ind)":
                    {
                        return "";
                    }
            }
            return strNumber;
        }


        /// -------------------------------------------------------------------
        /// <summary>
        /// Generic conversion helper, converts a decimal value into a string with
        /// a given number of releveant decimal digits, and custom decimal and
        /// thousand separators.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// -------------------------------------------------------------------
        public static string FormatNumber(object value, string strDecimalSeparator = ".", string strThousandsSeparator = "", int iNumDigits = -9999)
        {

            if (Convert.IsDBNull(value))
                return "";

            if (value is float)
            {
                return FormatSingle(Convert.ToSingle(value), strDecimalSeparator, strThousandsSeparator, iNumDigits);
            }
            else if (value is double)
            {
                return FormatDouble(Convert.ToDouble(value), strDecimalSeparator, strThousandsSeparator, iNumDigits);
            }
            else if (value is decimal)
            {
                return FormatDecimal(Convert.ToDecimal(value), strDecimalSeparator, strThousandsSeparator, iNumDigits);
            }
            return FormatInteger(Convert.ToInt32(value), strDecimalSeparator, strThousandsSeparator);

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Generic conversion helper, converts an integer value into a string using
    /// the fixed EwE number format of decimal points, using custom decimal and
    /// thousands separators.
    /// </summary>
    /// <param name="iValue">The integer to format into a string.</param>
    /// <param name="strDecimalSeparator">Decimal separator to use. Default is 
    /// a point.</param>
    /// <param name="strThousandsSeparator">Thousands separator to use. By default
    /// this separator is not used.</param>
    /// <returns>A formatted value.</returns>
    /// -------------------------------------------------------------------
        public static string FormatInteger(int iValue, string strDecimalSeparator = ".", string strThousandsSeparator = "")
        {

            var ci = CultureInfo.CurrentCulture;
            NumberFormatInfo ni = (NumberFormatInfo)ci.NumberFormat.Clone();

            ni.NumberDecimalSeparator = strDecimalSeparator;
            ni.NumberGroupSeparator = strThousandsSeparator;

            return Convert.ToString(iValue, ni);

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Generic conversion helper, converts a decimal value into a string with
    /// a given number of releveant decimal digits, and custom decimal and
    /// thousand separators.
    /// <seealso cref="FormatSingle"/>
    /// <seealso cref="FormatDouble"/>
    /// <seealso cref="FormatNumber"/>
    /// </summary>
    /// <param name="decValue">The decimal to format into a string.</param>
    /// <param name="strDecimalSeparator">Decimal separator to use. Default is 
    /// a point.</param>
    /// <param name="strThousandsSeparator">Thousands separator to use. By default
    /// this separator is not used.</param>
    /// <param name="iNumDigits">Number of decimal digits to use, or zero if
    /// formatting should show as many digits as needed.</param>
    /// <returns>A formatted value.</returns>
    /// -------------------------------------------------------------------
        public static string FormatDecimal(decimal decValue, string strDecimalSeparator = ".", string strThousandsSeparator = "", int iNumDigits = -9999)
        {

            var ci = CultureInfo.CurrentCulture;
            NumberFormatInfo ni = (NumberFormatInfo)ci.NumberFormat.Clone();

            ni.NumberDecimalSeparator = strDecimalSeparator;
            ni.NumberGroupSeparator = strThousandsSeparator;

            // PLEASE DO NOT USE Convert.Format below!!! Convert.ToString will use ni.NumberDecimalDigits
            // to determine the number of relevant digits (which is what we want) while Decimal.Format 
            // rounds to ni.NumberDecimalDigits (which is what we DO NOT want)

            if (iNumDigits > 0)
            {
                ni.NumberDecimalDigits = iNumDigits;
                // Cast to double as Decimal formatting ignores the requested NumberDecimalDigits
                return Convert.ToString((double)decValue, ni);
            }

            return Convert.ToString(decValue, ni);

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Generic conversion helper, converts a decimal value into a string with
    /// a given number of releveant decimal digits, and custom decimal and
    /// thousand separators.
    /// <seealso cref="FormatDecimal"/>
    /// <seealso cref="FormatDouble"/>
    /// <seealso cref="FormatNumber"/>
    /// </summary>
    /// <param name="sValue">The single to format into a string.</param>
    /// <param name="strDecimalSeparator">Decimal separator to use. Default is 
    /// a point.</param>
    /// <param name="strThousandsSeparator">Thousands separator to use. By default
    /// this separator is not used.</param>
    /// <param name="iNumDigits">Number of decimal digits to use, or zero if
    /// formatting should show as many digits as needed.</param>
    /// <returns>A formatted value.</returns>
    /// -------------------------------------------------------------------
        public static string FormatSingle(float sValue, string strDecimalSeparator = ".", string strThousandsSeparator = "", int iNumDigits = -9999)
        {

            var ci = CultureInfo.CurrentCulture;
            NumberFormatInfo ni = (NumberFormatInfo)ci.NumberFormat.Clone();

            ni.NumberDecimalSeparator = strDecimalSeparator;
            ni.NumberGroupSeparator = strThousandsSeparator;

            if (iNumDigits > 0)
                ni.NumberDecimalDigits = iNumDigits;

            // PLEASE DO NOT USE Convert.Format below!!! Convert.ToString will use ni.NumberDecimalDigits
            // to determine the number of relevant digits (which is what we want) while Single.Format 
            // rounds to ni.NumberDecimalDigits (which is what we DO NOT want)
            return Convert.ToString(sValue, ni);

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Generic conversion helper, converts a double value into a string with
    /// a given number of releveant decimal digits, and custom decimal and
    /// thousand separators.
    /// <seealso cref="FormatDecimal"/>
    /// <seealso cref="FormatSingle"/>
    /// <seealso cref="FormatNumber"/>
    /// </summary>
    /// <param name="dValue">The double to format into a string.</param>
    /// <param name="strDecimalSeparator">Decimal separator to use. Default is 
    /// a point.</param>
    /// <param name="strThousandsSeparator">Thousands separator to use. By default
    /// this separator is not used.</param>
    /// <param name="iNumDigits">Number of decimal digits to use, or zero if
    /// formatting should show as many digits as needed.</param>
    /// <seealso cref="cNumberUtils.NumRelevantDecimals"/>
    /// <returns>A formatted value.</returns>
    /// -------------------------------------------------------------------
        public static string FormatDouble(double dValue, string strDecimalSeparator = ".", string strThousandsSeparator = "", int iNumDigits = -9999)
        {

            var ci = CultureInfo.CurrentCulture;
            NumberFormatInfo ni = (NumberFormatInfo)ci.NumberFormat.Clone();

            ni.NumberDecimalSeparator = strDecimalSeparator;
            ni.NumberGroupSeparator = strThousandsSeparator;

            if (iNumDigits >= 0)
                ni.NumberDecimalDigits = iNumDigits;

            // PLEASE DO NOT USE Convert.Format below!!! Convert.ToString will use ni.NumberDecimalDigits
            // to determine the number of relevant digits (which is what we want) while Double.Format 
            // rounds to ni.NumberDecimalDigits (which is what we DO NOT want)
            return Convert.ToString(dValue, ni);

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Format a date for persistent storage.
    /// </summary>
    /// <param name="dtValue">The date to format.</param>
    /// <param name="strFormat">Optional date formatting flag (http://msdn.microsoft.com/en-us/library/zdtaw1bw%28v=vs.110%29.aspx)</param>
    /// <returns>A formatted date.</returns>
    /// <remarks>
    /// http://www.w3.org/TR/NOTE-datetime
    /// </remarks>
    /// -------------------------------------------------------------------
        public static string FormatDate(DateTime dtValue, string strFormat = "yyyy-MM-dd")
        {
            return dtValue.ToString(strFormat);
        }

        private static char[] CSV_SEPARATORCHARS = new char[] { ',', ' ', '\t' };

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Format a value for use in a CSV file.
    /// </summary>
    /// <param name="objValue">The value to format.</param>
    /// <param name="cQuote">Optional quote character to use for wrapping the value.</param>
    /// <param name="iNumDigits">Optional number of decimal digits to limit formatting to.</param>
    /// <returns>A field fit for display in a CSV file.</returns>
    /// <remarks>
    /// <para>Numbers will be en-US formatted.</para>
    /// <para>Double quotes will be removed.</para>
    /// <para>Values containing potential CSV separator characters will be encapsulated in double quotes.</para>
    /// </remarks>
    /// -----------------------------------------------------------------------
        public static string ToCSVField(object objValue, char cQuote = '"', int iNumDigits = -9999)
        {

            string strValue = "";

            if (objValue is null)
                return strValue;
            if (Convert.IsDBNull(objValue))
                return strValue;

            if (objValue is string | objValue is char)
            {
                strValue = Convert.ToString(objValue);
                if (StrictCSVFormatting)
                {
                    var sb = new StringBuilder();
                    for (int i = 0, loopTo = strValue.Length - 1; i <= loopTo; i++)
                    {
                        char c = strValue[i];
                        if (!char.IsNumber(c) & !char.IsLetter(c) & !(c == '_'))
                        {
                            sb.Append('_');
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    strValue = sb.ToString();
                }
            }
            else if (objValue is DateTime)
            {
                strValue = FormatDate((DateTime)objValue);
            }
            else
            {
                strValue = FormatNumber(objValue, iNumDigits: iNumDigits);
            }

            if (strValue.IndexOf('"') > 0)
            {
                strValue = strValue.Replace("\"", "");
            }
            if (strValue.IndexOfAny(CSV_SEPARATORCHARS) > 0)
            {
                strValue = cQuote + strValue + cQuote;
            }

            return strValue;

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Converts an incoming string to UTF-8 encoding.
    /// </summary>
    /// <param name="strIn">The string to convert.</param>
    /// <param name="encIn">The current encoding of <paramref name="strIn"/>.</param>
    /// <returns>A UTF-8 encoded version of the string.</returns>
    /// -------------------------------------------------------------------
        public static string ToUTF8(string strIn, Encoding encIn)
        {
            // Special cases
            strIn = strIn.Replace('²', '2');
            strIn = strIn.Replace('³', '3');
            // Shazaam
            byte[] data = encIn.GetBytes(strIn);
            return Encoding.UTF8.GetString(data);
        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Converts an incoming string to UTF-8 encoding, assuming that the
    /// incoming string encoded as ASCII (.NET default).
    /// </summary>
    /// <param name="strIn">The string to convert.</param>
    /// <returns>A UTF-8 encoded version of the string.</returns>
    /// -------------------------------------------------------------------
        public static string ToUTF8(string strIn)
        {
            return ToUTF8(strIn, Encoding.ASCII);
        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Convert a column number to an Excel-style column name. The resulting 
    /// column name will always be upper case.
    /// </summary>
    /// <param name="iColumn">The one-based column number to convert.</param>
    /// <returns>A character-based, Excel-style column name.</returns>
    /// -------------------------------------------------------------------
        public static string ToExcelColumnName(int iColumn)
        {

            Debug.Assert(iColumn >= 1);

            int iDiv = iColumn;
            int iMod;
            var sb = new StringBuilder();

            while (iDiv > 0)
            {
                iMod = (iDiv - 1) % 26;
                sb.Insert(0, Convert.ToChar(65 + iMod));
                iDiv = (int)Math.Round((iDiv - iMod) / 26d);
            }

            return sb.ToString();

        }


        /// <summary>Default string split delimiters, in order of decreasing relevance.</summary>
        public static char[] c_DELIMITERS = new char[] { Convert.ToChar(9), ';', ' ', ',' };

        /// -------------------------------------------------------------------
    /// <summary>
    /// Returns the most likely delimiter character in a string.
    /// </summary>
    /// <param name="strIn">The string to explore.</param>
    /// <param name="cQualifier">Qualifier character for enveloping non-splittable strings.</param>
    /// <param name="candidates">An array of possible delimiter characters. If 
    /// an empty array is provided or this parameter is omitted, the default 
    /// array <see cref="c_DELIMITERS"/> is used.</param>
    /// <returns>The most likely character used to split a string. If no
    /// candidate can be found the default comma (,) is returned.</returns>
    /// <remarks><para>This method splits <paramref name="strIn"/> by each 
    /// delimiter character in <paramref name="candidates"/> in order. If a 
    /// split returns more than one sub-string the split character is returned.
    /// If no split was possible the default comma character is returned.</para>
    /// </remarks>
    /// -------------------------------------------------------------------
        public static char FindStringDelimiter(string strIn, char cQualifier = '"', char[] candidates = null)
        {

            // Ensure that there are candidate delimiters
            if (candidates is null)
            {
                candidates = c_DELIMITERS;
            }

            if (candidates.Length == 0)
            {
                candidates = c_DELIMITERS;
            }

            // Did receive any data to split? 
            // NB: Do NOT use IsNullOrWhitespace here; all whitespace lines may contain valid split chars
            if (!string.IsNullOrEmpty(strIn))
            {
                // #Yes: find most relevant split character
                foreach (char c in candidates)
                {
                    // Does candidate occur in string?
                    if (strIn.IndexOf(c) >= 0)
                    {
                        // #Yes: Does split yield more than one substring?
                        if (SplitQualified(strIn, c, cQualifier).Length > 1)
                        {
                            // #Yes: return this character
                            return c;
                        }
                    }
                }
            }

            // Return default
            return ',';

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get the current time as a string to be used in file names.
    /// </summary>
    /// <remarks>The time stamp is formatted as 'year-month-day hour-minute-second'.</remarks>
    /// -----------------------------------------------------------------------
        public static string Now()
        {
            return FormatDate(DateTime.Now, "yyyy-MM-dd HH-mm-ss");
        }


    }
}