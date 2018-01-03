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

namespace Visyn.Exceptions
{
    /// <summary>
    /// Indicates that a string value can't be converted to a dest type.
    /// </summary>
 //   [Serializable]
    public sealed class ConvertException : FileHelpersException
    {
        #region "  Fields & Property  "

        /// <summary>The destination type.</summary>
        public Type FieldType { get; private set; }

        /// <summary>The value that can't be converted. (null for unknown)</summary>
        public string FieldStringValue { get; private set; }

        /// <summary>Extra info about the error.</summary>
        public string MessageExtra { get; private set; }

        /// <summary>The message without the Line, Column and FieldName.</summary>
        public string MessageOriginal { get; private set; }

        /// <summary>The name of the field related to the exception. (null for unknown)</summary>
        public string FieldName { get; set; }

        /// <summary>The line where the error was found. (-1 is unknown)</summary>
        public int LineNumber { get; set; }

        /// <summary>The estimate column where the error was found. (-1 is unknown)</summary>
        public int ColumnNumber { get; set; }

        #endregion

        #region "  Constructors  "

        /// <summary>
        /// Create a new ConvertException object
        /// </summary>
        /// <param name="origValue">The value to convert.</param>
        /// <param name="destType">The destination Type.</param>
        public ConvertException(string origValue, Type destType)
            : this(origValue, destType, string.Empty) {}


        /// <summary>
        /// Create a new ConvertException object
        /// </summary>
        /// <param name="origValue">The value to convert.</param>
        /// <param name="destType">The destination Type.</param>
        /// <param name="extraInfo">Additional info of the error.</param>
        public ConvertException(string origValue, Type destType, string extraInfo)
            : this(origValue, destType, string.Empty, -1, -1, extraInfo, null) {}

        /// <summary>
        /// Create a new ConvertException object
        /// </summary>
        /// <param name="origValue">The value to convert.</param>
        /// <param name="destType">The destination Type.</param>
        /// <param name="extraInfo">Additional info of the error.</param>
        /// <param name="columnNumber">The estimated column number.</param>
        /// <param name="lineNumber">The line where the error was found.</param>
        /// <param name="fieldName">The name of the field with the error</param>
        /// <param name="innerEx">The Inner Exception</param>
        public ConvertException(string origValue,
            Type destType,
            string fieldName,
            int lineNumber,
            int columnNumber,
            string extraInfo,
            Exception innerEx)
            : base(MessageBuilder(origValue, destType, fieldName, lineNumber, columnNumber, extraInfo), innerEx)
        {
            MessageOriginal = string.Empty;
            FieldStringValue = origValue;
            FieldType = destType;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            FieldName = fieldName;
            MessageExtra = extraInfo;

            if (origValue != null &&
                destType != null)
                MessageOriginal = "Error Converting '" + origValue + "' to type: '" + destType.Name + "'. ";
        }

        private static string MessageBuilder(string origValue,
            Type destType,
            string fieldName,
            int lineNumber,
            int columnNumber,
            string extraInfo)
        {
            var res = string.Empty;
            if (lineNumber >= 0)
                res += "Line: " + lineNumber.ToString() + ". ";

            if (columnNumber >= 0)
                res += "Column: " + columnNumber.ToString() + ". ";

            if (!string.IsNullOrEmpty(fieldName))
                res += "Field: " + fieldName + ". ";

            if (origValue != null &&
                destType != null)
                res += "Error Converting '" + origValue + "' to type: '" + destType.Name + "'. ";

            res += extraInfo;

            return res;
        }

        #endregion
    }
}
