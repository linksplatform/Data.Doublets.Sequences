using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform.Converters;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Unicode
{
    /// <summary>
    /// <para>
    /// Represents the string to unicode symbols list converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="IConverter{string, IList{TLink}}"/>
    public class StringToUnicodeSymbolsListConverter<TLink> : IConverter<string, IList<TLink>>
    {
        /// <summary>
        /// <para>
        /// The char to unicode symbol converter.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly IConverter<char, TLink> _charToUnicodeSymbolConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="StringToUnicodeSymbolsListConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="charToUnicodeSymbolConverter">
        /// <para>A char to unicode symbol converter.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringToUnicodeSymbolsListConverter(IConverter<char, TLink> charToUnicodeSymbolConverter) => _charToUnicodeSymbolConverter = charToUnicodeSymbolConverter;

        /// <summary>
        /// <para>
        /// Converts the source.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="source">
        /// <para>The source.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The elements.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IList<TLink> Convert(string source)
        {
            var elements = new TLink[source.Length];
            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] = _charToUnicodeSymbolConverter.Convert(source[i]);
            }
            return elements;
        }
    }
}
