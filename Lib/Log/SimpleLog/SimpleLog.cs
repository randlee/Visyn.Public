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
using System.Collections;

namespace Visyn.Log.SimpleLog
{
    public class SimpleLog<TEventLevel> : SimpleLogBase<TEventLevel,SimpleLogEntry<TEventLevel>> where TEventLevel : struct, IComparable
    {
        private TEventLevel ErrorLevel { get; }

        public SimpleLog(TEventLevel errorLevel)
        {
            ErrorLevel = errorLevel;
        }

        #region Overrides of SimpleLogBase<SeverityLevel,SimpleLogEntry>

        public override void Log(object source, string message, TEventLevel level)
        {
            LogItem(new SimpleLogEntry<TEventLevel>(source.ToString(), message, level));
        }
    

        public override void Log(object source, ICollection logItems, TEventLevel level, string prefix = null)
        {
            foreach (var item in logItems)
            {
                LogItem(new SimpleLogEntry<TEventLevel>(source.ToString(), item.ToString(), level));
            }
        }
  
        /// <summary>
        /// Handles the exception
        /// If false is returned, sender should throw the exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <returns><c>true</c> if exception was handled, <c>false</c> otherwise.</returns>
        public override bool HandleException(object sender, Exception exception)
        {
            if (typeof(TEventLevel) == typeof(SeverityLevel))
                LogItem(new SimpleLogEntry<TEventLevel>(sender?.ToString(), exception?.Message, ErrorLevel));
            return true;
        }

        #endregion
    }
}