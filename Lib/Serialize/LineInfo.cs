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
using System.Diagnostics;
using System.Globalization;

namespace Visyn.Serialize
{
    /// <summary>
    /// Record read from the file for processing
    /// </summary>
    /// <remarks>
    /// The data inside the LIneInfo may be reset during processing,
    /// for example on read next line.  Do not rely on this class
    /// containing all data from a record for all time.  It is designed
    /// to read data sequentially.
    /// </remarks>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayStr) + "()}")]
    public sealed class LineInfo
    {
        #region "  Constructor  "

        //static readonly char[] mEmptyChars = new char[] {};
        /// <summary>
        /// Create a line info with data from line
        /// </summary>
        /// <param name="line"></param>
        public LineInfo(string line)
        {
            mReader = null;
            mLineStr = line;
            mCurrentPos = 0;
        }

        #endregion

        /// <summary>
        /// Return part of line,  Substring
        /// </summary>
        /// <param name="from">Start position (zero offset)</param>
        /// <param name="count">Number of characters to extract</param>
        /// <returns>substring from line</returns>
        public string Substring(int from, int count) => mLineStr.Substring(@from, count);

        #region "  Internal Fields  "

        //internal string  mLine;
        //internal char[] mLine;

        /// <summary>
        /// Record read from reader
        /// </summary>
        public string mLineStr { get; internal set; }

        /// <summary>
        /// Reader that got the string
        /// </summary>
        public ForwardReader mReader { get; set; }

        /// <summary>
        /// Where we are processing records from
        /// </summary>
        public int mCurrentPos { get; set; }

        /// <summary>
        /// List of whitespace characters that we want to skip
        /// </summary>
       
        public static readonly char[] WhitespaceChars = new char[] {
            '\t', '\n', '\v', '\f', '\r', ' ', '\x00a0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005',
            '\u2006', '\u2007', '\u2008',
            '\u2009', '\u200a', '\u200b', '\u3000', '\ufeff'
        };

        #endregion

        /// <summary>
        /// Debugger display string
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplayStr() => IsEOL() ? "<EOL>" : CurrentString;

        /// <summary>
        /// Extract a single field from the system
        /// </summary>
        public string CurrentString => mLineStr.Substring(mCurrentPos, mLineStr.Length - mCurrentPos);

        /// <summary>
        /// If we have extracted more that the field contains.
        /// </summary>
        /// <returns>True if end of line</returns>
        public bool IsEOL() => mCurrentPos >= mLineStr.Length;

        /// <summary>
        /// Amount of data left to process
        /// </summary>
        public int CurrentLength => mLineStr.Length - mCurrentPos;

        /// <summary>
        /// Is there only whitespace left in the record?
        /// </summary>
        /// <returns>True if only whitespace</returns>
        public bool EmptyFromPos()
        {
            // Check if the chars at pos or right are empty ones
            var length = mLineStr.Length;
            var pos = mCurrentPos;
            while (pos < length && Array.BinarySearch(WhitespaceChars, mLineStr[pos]) >= 0)
                pos++;

            return pos >= length;
        }

        /// <summary>
        /// delete leading whitespace from record
        /// </summary>
        public void TrimStart()
        {
            TrimStartSorted(WhitespaceChars);
        }

        /// <summary>
        /// Delete any of these characters from beginning of the record
        /// </summary>
        /// <param name="toTrim"></param>
        public void TrimStart(char[] toTrim)
        {
            Array.Sort(toTrim);
            TrimStartSorted(toTrim);
        }

        /// <summary>
        /// Move the record pointer along skipping these characters
        /// </summary>
        /// <param name="toTrim">Sorted array of character to skip</param>
        private void TrimStartSorted(char[] toTrim)
        {
            // Move the pointer to the first non to Trim char
            var length = mLineStr.Length;

            while (mCurrentPos < length &&  Array.BinarySearch(toTrim, mLineStr[mCurrentPos]) >= 0)
                mCurrentPos++;
        }

        public bool StartsWith(string str)
        {
            // Returns true if the string begin with str
            if (mCurrentPos >= mLineStr.Length)  return false;
            return
                mCompare.Compare(mLineStr,
                    mCurrentPos,
                    str.Length,
                    str,
                    0,
                    str.Length,
                    CompareOptions.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Check that the record begins with a value ignoring whitespace
        /// </summary>
        /// <param name="str">String to check for</param>
        /// <returns>True if record begins with</returns>
        public bool StartsWithTrim(string str)
        {
            var length = mLineStr.Length;
            var pos = mCurrentPos;

            while (pos < length && Array.BinarySearch(WhitespaceChars, mLineStr[pos]) >= 0)
                pos++;

            return mCompare.Compare(mLineStr, pos, str, 0, CompareOptions.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// get The next line from the system and reset the line pointer to zero
        /// </summary>
        public void ReadNextLine()
        {
            mLineStr = mReader.ReadNextLine();

            mCurrentPos = 0;
        }

        private static readonly CompareInfo mCompare = StringHelper.CreateComparer();

        /// <summary>
        /// Find the location of the next string in record
        /// </summary>
        /// <param name="foundThis">String we are looking for</param>
        /// <returns>Position of the next one</returns>
        public int IndexOf(string foundThis) => mCompare.IndexOf(mLineStr, foundThis, mCurrentPos, CompareOptions.Ordinal);

        /// <summary>
        /// Reset the string back to the original line and reset the line pointer
        /// </summary>
        /// <remarks>If the input is multi line, this will read next record and remove the original data</remarks>
        /// <param name="line">Line to use</param>
        public void ReLoad(string line)
        {
            mLineStr = line;
            mCurrentPos = 0;
        }
    }
}
