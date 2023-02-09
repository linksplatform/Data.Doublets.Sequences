using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Converters;
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Unicode
{
    /// <summary>
    /// <para>
    /// Represents the unicode symbol to char converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{TLinkAddress, char}"/>
    public class UnicodeSymbolToCharConverter<TLinkAddress> : LinksOperatorBase<TLinkAddress>, IConverter<TLinkAddress, char> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private static readonly UncheckedConverter<TLinkAddress, char> _addressToCharConverter = UncheckedConverter<TLinkAddress, char>.Default;
        private readonly IConverter<TLinkAddress> _numberToAddressConverter;
        private readonly ICriterionMatcher<TLinkAddress> _unicodeSymbolCriterionMatcher;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="UnicodeSymbolToCharConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="numberToAddressConverter">
        /// <para>A number to address converter.</para>
        /// <para></para>
        /// </param>
        /// <param name="unicodeSymbolCriterionMatcher">
        /// <para>A unicode symbol criterion matcher.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnicodeSymbolToCharConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> numberToAddressConverter, ICriterionMatcher<TLinkAddress> unicodeSymbolCriterionMatcher) : base(links)
        {
            _numberToAddressConverter = numberToAddressConverter;
            _unicodeSymbolCriterionMatcher = unicodeSymbolCriterionMatcher;
        }

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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>Specified link is not a unicode symbol.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The char</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Convert(TLinkAddress source)
        {
            if (!_unicodeSymbolCriterionMatcher.IsMatched(source))
            {
                throw new ArgumentOutOfRangeException(nameof(source), source, "Specified link is not a unicode symbol.");
            }
            return _addressToCharConverter.Convert(_numberToAddressConverter.Convert(_links.GetSource(source)));
        }
    }
}
