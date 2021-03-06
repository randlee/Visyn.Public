#region Copyright (c) 2015-2018 Visyn
// The MIT License(MIT)
// 
// Copyright (c) 2015-2018 Visyn
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Visyn.Serialize
{
    /// <summary>
    /// Helper classes for strings
    /// </summary>
    internal static class StringHelper
    {
        #region "  CreateQuotedString  "

        /// <summary>
        /// Convert a string to a string with quotes around it,
        /// if the quote appears within the string it is doubled
        /// </summary>
        /// <param name="sb">Where string is added</param>
        /// <param name="source">String to be added</param>
        /// <param name="quoteChar">quote character to use, eg "</param>
        internal static void CreateQuotedString(StringBuilder sb, string source, char quoteChar)
        {
            if (source == null) source = string.Empty;

            var quotedCharStr = quoteChar.ToString();
            var escapedString = source.Replace(quotedCharStr, quotedCharStr + quotedCharStr);

            sb.Append(quoteChar);
            sb.Append(escapedString);
            sb.Append(quoteChar);
        }

        #endregion

        #region "  RemoveBlanks  "

        /// <summary>
        /// Remove leading blanks and blanks after the plus or minus sign from a string
        /// to allow it to be parsed by ToInt or other converters
        /// </summary>
        /// <param name="source">source to trim</param>
        /// <returns>String without blanks</returns>
        /// <remarks>
        /// This logic is used to handle strings line " +  21 " from
        /// input data (returns "+21 "). The integer convert would fail
        /// because of the extra blanks so this logic trims them
        /// </remarks>
        internal static string RemoveBlanks(string source)
        {
            var i = 0;

            while (i < source.Length && char.IsWhiteSpace(source[i]))
                i++;

            // Only whitespace return an empty string
            if (i >= source.Length) return string.Empty;

            // we are looking for a gap after the sign, if not found then
            // trim off the front of the string and return
            if (source[i] != '+' && source[i] != '-') return source;
            i++;
            if (!char.IsWhiteSpace(source[i])) return source; //  sign is followed by text so just return it

            // start out with the sign
            var sb = new StringBuilder(source[i - 1].ToString(), source.Length - i);

            i++; // I am on whitepsace so skip it
            while (i < source.Length && char.IsWhiteSpace(source[i]))
                i++;

            if (i < source.Length) sb.Append(source.Substring(i));

            return sb.ToString();
        }

        #endregion

        private static CultureInfo mCulture;

        /// <summary>
        /// Create an invariant culture comparison operator
        /// </summary>
        /// <returns>Comparison operations</returns>
        internal static CompareInfo CreateComparer()
        {
            if (mCulture == null) mCulture = CultureInfo.InvariantCulture;

            return mCulture.CompareInfo;
        }
        /// <summary>
        /// Replace string with another ignoring the case of the string
        /// </summary>
        /// <param name="original">Original string</param>
        /// <param name="oldValue">string to replace</param>
        /// <param name="newValue">string to insert</param>
        /// <returns>string with values replaced</returns>
        public static string ReplaceIgnoringCase(string original, string oldValue, string newValue)
        {
            return Replace(original, oldValue, newValue, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// replace the one string with another, and keep doing it
        /// </summary>
        /// <param name="original">Original string</param>
        /// <param name="oldValue">Value to replace</param>
        /// <param name="newValue">value to replace with</param>
        /// <returns>String with all multiple occurrences replaced</returns>
        internal static string ReplaceRecursive(string original, string oldValue, string newValue)
        {
            const int maxTries = 1000;

            string ante, res;

            res = original.Replace(oldValue, newValue);

            var i = 0;
            do
            {
                i++;
                ante = res;
                res = ante.Replace(oldValue, newValue);
            } while (ante != res ||
                        i > maxTries);

            return res;
        }

        /// <summary>
        /// convert a string into a valid identifier
        /// </summary>
        /// <param name="original">Original string</param>
        /// <returns>valid identifier  Original_string</returns>
        internal static string ToValidIdentifier(string original)
        {
            if (original.Length == 0) return string.Empty;

            var res = new StringBuilder(original.Length + 1);
            if (!char.IsLetter(original[0]) && original[0] != '_')
                res.Append('_');

            foreach (var c in original)
            {
                if (char.IsLetterOrDigit(c) || c == '_') res.Append(c);
                else res.Append('_');
            }

            var identifier = ReplaceRecursive(res.ToString(), "__", "_").Trim('_');
            if (identifier.Length == 0) return "_";
            if (char.IsDigit(identifier[0])) identifier = "_" + identifier;

            return identifier;
        }


        /// <summary>
        /// String replace with a comparison function, eg OridinalIgnoreCase
        /// </summary>
        /// <param name="original">Original string</param>
        /// <param name="oldValue">Value to be replaced</param>
        /// <param name="newValue">value to replace with</param>
        /// <param name="comparisionType">Comparison type (enum)</param>
        /// <returns>String with values replaced</returns>
        public static string Replace(string original, string oldValue, string newValue, StringComparison comparisionType = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(oldValue)) return original;

            var buffer = new StringBuilder(original.Length);
            var index = -1;
            var lastIndex = 0;

            while ((index = original.IndexOf(oldValue, index + 1, comparisionType)) >= 0)
            {
                buffer.Append(original, lastIndex, index - lastIndex);
                buffer.Append(newValue);

                lastIndex = index + oldValue.Length;
            }
            buffer.Append(original, lastIndex, original.Length - lastIndex);

            return buffer.ToString();
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the parameter is null, empty, or consists only of white-space characters.</returns>
        [Obsolete("Use string.IsNullOrWhitespace()", true)]
        public static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || value.Cast<char>().All(char.IsWhiteSpace);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string ignoring white spaces at the start.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="value">The string to compare.</param>
        /// <param name="comparisonType">string comparison type.</param>
        /// <returns></returns>
        public static bool StartsWithIgnoringWhiteSpaces(string source, string value, StringComparison comparisonType)
        {
            if (value == null) return false;

            // find lower bound
            var i = 0;
            var sz = source.Length;
            while (i < sz && char.IsWhiteSpace(source[i])) { i++; }
            // adjust upper bound
            sz = sz - i;
            if (sz < value.Length) return false;
            sz = value.Length;
            // search
            return source.IndexOf(value, i, sz, comparisonType) == i;
        }
    }
}

