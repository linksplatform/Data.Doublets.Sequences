using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform.Converters;
using Platform.Data.Doublets.Sequences.Indexes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Unicode
{
    /// <summary>
    /// <para>
    /// Represents the unicode symbols list to unicode sequence converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLink}"/>
    /// <seealso cref="IConverter{IList{TLink}, TLink}"/>
    public class UnicodeSymbolsListToUnicodeSequenceConverter<TLink> : LinksOperatorBase<TLink>, IConverter<IList<TLink>, TLink>
    {
        /// <summary>
        /// <para>
        /// The index.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly ISequenceIndex<TLink> _index;
        /// <summary>
        /// <para>
        /// The list to sequence link converter.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly IConverter<IList<TLink>, TLink> _listToSequenceLinkConverter;
        /// <summary>
        /// <para>
        /// The unicode sequence marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly TLink _unicodeSequenceMarker;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="UnicodeSymbolsListToUnicodeSequenceConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="index">
        /// <para>A index.</para>
        /// <para></para>
        /// </param>
        /// <param name="listToSequenceLinkConverter">
        /// <para>A list to sequence link converter.</para>
        /// <para></para>
        /// </param>
        /// <param name="unicodeSequenceMarker">
        /// <para>A unicode sequence marker.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnicodeSymbolsListToUnicodeSequenceConverter(ILinks<TLink> links, ISequenceIndex<TLink> index, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker) : base(links)
        {
            _index = index;
            _listToSequenceLinkConverter = listToSequenceLinkConverter;
            _unicodeSequenceMarker = unicodeSequenceMarker;
        }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="UnicodeSymbolsListToUnicodeSequenceConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="listToSequenceLinkConverter">
        /// <para>A list to sequence link converter.</para>
        /// <para></para>
        /// </param>
        /// <param name="unicodeSequenceMarker">
        /// <para>A unicode sequence marker.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnicodeSymbolsListToUnicodeSequenceConverter(ILinks<TLink> links, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker)
            : this(links, new Unindex<TLink>(), listToSequenceLinkConverter, unicodeSequenceMarker) { }

        /// <summary>
        /// <para>
        /// Converts the list.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="list">
        /// <para>The list.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLink Convert(IList<TLink> list)
        {
            _index.Add(list);
            var sequence = _listToSequenceLinkConverter.Convert(list);
            return _links.GetOrCreate(sequence, _unicodeSequenceMarker);
        }
    }
}
