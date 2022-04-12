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
    /// <seealso cref="IConverter{string, IList{TLinkAddress}}"/>
    public class StringToUnicodeSymbolsListConverter<TLinkAddress> : IConverter<string, IList<TLinkAddress>?>
    {
        private readonly IConverter<char, TLinkAddress> _charToUnicodeSymbolConverter;

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
        public StringToUnicodeSymbolsListConverter(IConverter<char, TLinkAddress> charToUnicodeSymbolConverter) => _charToUnicodeSymbolConverter = charToUnicodeSymbolConverter;

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
        public IList<TLinkAddress>? Convert(string source)
        {
            var elements = new TLinkAddress[source.Length];
            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] = _charToUnicodeSymbolConverter.Convert(source[i]);
            }
            return elements;
        }
    }
}
