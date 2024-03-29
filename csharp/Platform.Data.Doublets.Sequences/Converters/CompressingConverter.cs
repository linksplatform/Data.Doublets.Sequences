using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Collections;
using Platform.Converters;
using Platform.Singletons;
using Platform.Numbers;
using Platform.Data.Doublets.Sequences.Frequencies.Cache;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Converters
{
    /// <remarks>
    /// TODO: Возможно будет лучше если алгоритм будет выполняться полностью изолированно от Links на этапе сжатия.
    ///     А именно будет создаваться временный список пар необходимых для выполнения сжатия, в таком случае тип значения элемента массива может быть любым, как char так и ulong.
    ///     Как только список/словарь пар был выявлен можно разом выполнить создание всех этих пар, а так же разом выполнить замену.
    /// </remarks>
    public class CompressingConverter<TLinkAddress> : LinksListToSequenceConverterBase<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>

    {
    private static readonly LinksConstants<TLinkAddress> _constants = Default<LinksConstants<TLinkAddress>>.Instance;
    private static readonly EqualityComparer<TLinkAddress> _equalityComparer = EqualityComparer<TLinkAddress>.Default;
    private static readonly Comparer<TLinkAddress> _comparer = Comparer<TLinkAddress>.Default;
    private readonly IConverter<IList<TLinkAddress>, TLinkAddress> _baseConverter;
    private readonly LinkFrequenciesCache<TLinkAddress> _doubletFrequenciesCache;
    private readonly TLinkAddress _minFrequencyToCompress;
    private readonly bool _doInitialFrequenciesIncrement;
    private Doublet<TLinkAddress> _maxDoublet;
    private LinkFrequency<TLinkAddress> _maxDoubletData;

    private struct HalfDoublet
    {
        /// <summary>
        /// <para>
        /// The element.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress Element;

        /// <summary>
        /// <para>
        /// The doublet data.
        /// </para>
        /// <para></para>
        /// </summary>
        public LinkFrequency<TLinkAddress> DoubletData;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="HalfDoublet"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="element">
        /// <para>A element.</para>
        /// <para></para>
        /// </param>
        /// <param name="doubletData">
        /// <para>A doublet data.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HalfDoublet(TLinkAddress element, LinkFrequency<TLinkAddress> doubletData)
        {
            Element = element;
            DoubletData = doubletData;
        }

        /// <summary>
        /// <para>
        /// Returns the string.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        public override string ToString() => $"{Element}: ({DoubletData})";
    }

    /// <summary>
    /// <para>
    /// Initializes a new <see cref="CompressingConverter"/> instance.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <param name="links">
    /// <para>A links.</para>
    /// <para></para>
    /// </param>
    /// <param name="baseConverter">
    /// <para>A base converter.</para>
    /// <para></para>
    /// </param>
    /// <param name="doubletFrequenciesCache">
    /// <para>A doublet frequencies cache.</para>
    /// <para></para>
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompressingConverter(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>, TLinkAddress> baseConverter, LinkFrequenciesCache<TLinkAddress> doubletFrequenciesCache) : this(links, baseConverter, doubletFrequenciesCache, TLinkAddress.One, true)
    {
    }

    /// <summary>
    /// <para>
    /// Initializes a new <see cref="CompressingConverter"/> instance.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <param name="links">
    /// <para>A links.</para>
    /// <para></para>
    /// </param>
    /// <param name="baseConverter">
    /// <para>A base converter.</para>
    /// <para></para>
    /// </param>
    /// <param name="doubletFrequenciesCache">
    /// <para>A doublet frequencies cache.</para>
    /// <para></para>
    /// </param>
    /// <param name="doInitialFrequenciesIncrement">
    /// <para>A do initial frequencies increment.</para>
    /// <para></para>
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompressingConverter(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>, TLinkAddress> baseConverter, LinkFrequenciesCache<TLinkAddress> doubletFrequenciesCache, bool doInitialFrequenciesIncrement) : this(links, baseConverter, doubletFrequenciesCache, TLinkAddress.One, doInitialFrequenciesIncrement)
    {
    }

    /// <summary>
    /// <para>
    /// Initializes a new <see cref="CompressingConverter"/> instance.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <param name="links">
    /// <para>A links.</para>
    /// <para></para>
    /// </param>
    /// <param name="baseConverter">
    /// <para>A base converter.</para>
    /// <para></para>
    /// </param>
    /// <param name="doubletFrequenciesCache">
    /// <para>A doublet frequencies cache.</para>
    /// <para></para>
    /// </param>
    /// <param name="minFrequencyToCompress">
    /// <para>A min frequency to compress.</para>
    /// <para></para>
    /// </param>
    /// <param name="doInitialFrequenciesIncrement">
    /// <para>A do initial frequencies increment.</para>
    /// <para></para>
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CompressingConverter(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>, TLinkAddress> baseConverter, LinkFrequenciesCache<TLinkAddress> doubletFrequenciesCache, TLinkAddress minFrequencyToCompress, bool doInitialFrequenciesIncrement) : base(links)
    {
        _baseConverter = baseConverter;
        _doubletFrequenciesCache = doubletFrequenciesCache;
        if (minFrequencyToCompress < TLinkAddress.One)
        {
            minFrequencyToCompress = TLinkAddress.One;
        }
        _minFrequencyToCompress = minFrequencyToCompress;
        _doInitialFrequenciesIncrement = doInitialFrequenciesIncrement;
        ResetMaxDoublet();
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
    /// <returns>
    /// <para>The link</para>
    /// <para></para>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override TLinkAddress Convert(IList<TLinkAddress>? source) => _baseConverter.Convert(Compress(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IList<TLinkAddress>? Compress(IList<TLinkAddress>? sequence)
    {
        if (sequence.IsNullOrEmpty())
        {
            return null;
        }
        if (sequence.Count == 1)
        {
            return sequence;
        }
        if (sequence.Count == 2)
        {
            return new[] { _links.GetOrCreate(sequence[0], sequence[1]) };
        }
        // TODO: arraypool with min size (to improve cache locality) or stackallow with Sigil
        var copy = new HalfDoublet[sequence.Count];
        Doublet<TLinkAddress> doublet = default;
        for (var i = 1; i < sequence.Count; i++)
        {
            doublet = new Doublet<TLinkAddress>(sequence[i - 1], sequence[i]);
            LinkFrequency<TLinkAddress> data;
            if (_doInitialFrequenciesIncrement)
            {
                data = _doubletFrequenciesCache.IncrementFrequency(ref doublet);
            }
            else
            {
                data = _doubletFrequenciesCache.GetFrequency(ref doublet);
                if (data == null)
                {
                    throw new NotSupportedException("If you ask not to increment frequencies, it is expected that all frequencies for the sequence are prepared.");
                }
            }
            copy[i - 1].Element = sequence[i - 1];
            copy[i - 1].DoubletData = data;
            UpdateMaxDoublet(ref doublet, data);
        }
        copy[sequence.Count - 1].Element = sequence[sequence.Count - 1];
        copy[sequence.Count - 1].DoubletData = new LinkFrequency<TLinkAddress>();
        if (_maxDoubletData.Frequency > TLinkAddress.Zero)
        {
            var newLength = ReplaceDoublets(copy);
            sequence = new TLinkAddress[newLength];
            for (int i = 0; i < newLength; i++)
            {
                sequence[i] = copy[i].Element;
            }
        }
        return sequence;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ReplaceDoublets(HalfDoublet[] copy)
    {
        var oldLength = copy.Length;
        var newLength = copy.Length;
        while (_maxDoubletData.Frequency > TLinkAddress.Zero)
        {
            var maxDoubletSource = _maxDoublet.Source;
            var maxDoubletTarget = _maxDoublet.Target;
            if (_maxDoubletData.Link == _constants.Null)
            {
                _maxDoubletData.Link = _links.GetOrCreate(maxDoubletSource, maxDoubletTarget);
            }
            var maxDoubletReplacementLink = _maxDoubletData.Link;
            oldLength--;
            var oldLengthMinusTwo = oldLength - 1;
            // Substitute all usages
            int w = 0, r = 0; // (r == read, w == write)
            for (; r < oldLength; r++)
            {
                if (copy[r].Element == maxDoubletSource && copy[r + 1].Element == maxDoubletTarget)
                {
                    if (r > 0)
                    {
                        var previous = copy[w - 1].Element;
                        copy[w - 1].DoubletData.DecrementFrequency();
                        copy[w - 1].DoubletData = _doubletFrequenciesCache.IncrementFrequency(previous, maxDoubletReplacementLink);
                    }
                    if (r < oldLengthMinusTwo)
                    {
                        var next = copy[r + 2].Element;
                        copy[r + 1].DoubletData.DecrementFrequency();
                        copy[w].DoubletData = _doubletFrequenciesCache.IncrementFrequency(maxDoubletReplacementLink, next);
                    }
                    copy[w++].Element = maxDoubletReplacementLink;
                    r++;
                    newLength--;
                }
                else
                {
                    copy[w++] = copy[r];
                }
            }
            if (w < newLength)
            {
                copy[w] = copy[r];
            }
            oldLength = newLength;
            ResetMaxDoublet();
            UpdateMaxDoublet(copy, newLength);
        }
        return newLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetMaxDoublet()
    {
        _maxDoublet = new Doublet<TLinkAddress>();
        _maxDoubletData = new LinkFrequency<TLinkAddress>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMaxDoublet(HalfDoublet[] copy, int length)
    {
        Doublet<TLinkAddress> doublet = default;
        for (var i = 1; i < length; i++)
        {
            doublet = new Doublet<TLinkAddress>(copy[i - 1].Element, copy[i].Element);
            UpdateMaxDoublet(ref doublet, copy[i - 1].DoubletData);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMaxDoublet(ref Doublet<TLinkAddress> doublet, LinkFrequency<TLinkAddress> data)
    {
        var frequency = data.Frequency;
        var maxFrequency = _maxDoubletData.Frequency;
        //if (frequency > _minFrequencyToCompress && (maxFrequency < frequency || (maxFrequency == frequency && doublet.Source + doublet.Target < /* gives better compression string data (and gives collisions quickly) */ _maxDoublet.Source + _maxDoublet.Target))) 
        if (frequency > _minFrequencyToCompress && (maxFrequency > frequency) || (maxFrequency == frequency && (doublet.Source + doublet.Target > _maxDoublet.Source + _maxDoublet.Target))) /* gives better stability and better compression on sequent data and even on rundom numbers data (but gives collisions anyway) */
        {
            _maxDoublet = doublet;
            _maxDoubletData = data;
        }
    }
    }
}
