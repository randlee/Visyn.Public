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

namespace Visyn.Types
{
    /// <summary>
    /// Interface IFieldConverter
    /// </summary>
    /// <seealso cref="Visyn.Types.IType" />
    public interface IFieldConverter : IType
    {
        /// <summary>
        /// Gets a value indicating whether [custom null handling].
        /// </summary>
        /// <value><c>true</c> if [custom null handling]; otherwise, <c>false</c>.</value>
        bool CustomNullHandling { get; }

        /// <summary>
        /// Fields to string.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns>System.String.</returns>
        string FieldToString(object from);
        /// <summary>
        /// Strings to field.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.Object.</returns>
        object StringToField(string text);
    }
}
