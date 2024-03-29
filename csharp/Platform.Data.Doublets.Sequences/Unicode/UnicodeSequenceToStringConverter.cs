using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Platform.Converters;
using Platform.Data.Doublets.Sequences.CriterionMatchers;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Unicode
{
    /// <summary>
    /// <para>
    /// Represents the unicode sequence to string converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{TLinkAddress, string}"/>
    public class UnicodeSequenceToStringConverter<TLinkAddress> : LinksOperatorBase<TLinkAddress>, IConverter<TLinkAddress, string> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly ICriterionMatcher<TLinkAddress> _unicodeSequenceCriterionMatcher;
        private readonly ISequenceWalker<TLinkAddress> _sequenceWalker;
        private readonly IConverter<TLinkAddress, char> _unicodeSymbolToCharConverter;
        private readonly TLinkAddress _unicodeSequenceMarker;


        /// <summary>
        /// <para>
        /// Initializes a new <see cref="UnicodeSequenceToStringConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="unicodeSequenceCriterionMatcher">
        /// <para>A unicode sequence criterion matcher.</para>
        /// <para></para>
        /// </param>
        /// <param name="sequenceWalker">
        /// <para>A sequence walker.</para>
        /// <para></para>
        /// </param>
        /// <param name="unicodeSymbolToCharConverter">
        /// <para>A unicode symbol to char converter.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnicodeSequenceToStringConverter(ILinks<TLinkAddress> links, ICriterionMatcher<TLinkAddress> unicodeSequenceCriterionMatcher, ISequenceWalker<TLinkAddress> sequenceWalker, IConverter<TLinkAddress, char> unicodeSymbolToCharConverter, TLinkAddress unicodeSequenceMarker) : base(links)
        {
            _unicodeSequenceCriterionMatcher = unicodeSequenceCriterionMatcher;
            _sequenceWalker = sequenceWalker;
            _unicodeSymbolToCharConverter = unicodeSymbolToCharConverter;
            _unicodeSequenceMarker = unicodeSequenceMarker;
        }

        public UnicodeSequenceToStringConverter(ILinks<TLinkAddress> links, ISequenceWalker<TLinkAddress> sequenceWalker, IConverter<TLinkAddress, char> unicodeSymbolToCharConverter, TLinkAddress unicodeSequenceMarker): this(links, new UnicodeSequenceMatcher<TLinkAddress>(links, unicodeSequenceMarker), sequenceWalker, unicodeSymbolToCharConverter, unicodeSequenceMarker){}


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
        /// <para>Specified link is not a unicode sequence.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Convert(TLinkAddress source)
        {
            if (!_unicodeSequenceCriterionMatcher.IsMatched(source))
            {
                throw new ArgumentOutOfRangeException(nameof(source), source, "Specified link is not a unicode sequence.");
            }
            if((_unicodeSequenceMarker == source))
            {
                return String.Empty;
            }
            var sequence = _links.GetSource(source);
            var sb = new StringBuilder();
            foreach(var character in _sequenceWalker.Walk(sequence))
            {
                sb.Append(_unicodeSymbolToCharConverter.Convert(character));
            }
            return sb.ToString();
        }
    }
}
