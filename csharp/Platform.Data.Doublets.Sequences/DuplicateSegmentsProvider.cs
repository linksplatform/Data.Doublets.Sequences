using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform.Interfaces;
using Platform.Collections;
using Platform.Collections.Lists;
using Platform.Collections.Segments;
using Platform.Collections.Segments.Walkers;
using Platform.Singletons;
using Platform.Converters;
using Platform.Data.Doublets.Unicode;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences
{
    /// <summary>
    /// <para>
    /// Represents the duplicate segments provider.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="DictionaryBasedDuplicateSegmentsWalkerBase{TLink}"/>
    /// <seealso cref="IProvider{IList{KeyValuePair{IList{TLink}, IList{TLink}}}}"/>
    public class DuplicateSegmentsProvider<TLink> : DictionaryBasedDuplicateSegmentsWalkerBase<TLink>, IProvider<IList<KeyValuePair<IList<TLink>, IList<TLink>>>>
    {
        private static readonly UncheckedConverter<TLink, long> _addressToInt64Converter = UncheckedConverter<TLink, long>.Default;
        private static readonly UncheckedConverter<TLink, ulong> _addressToUInt64Converter = UncheckedConverter<TLink, ulong>.Default;
        private static readonly UncheckedConverter<ulong, TLink> _uInt64ToAddressConverter = UncheckedConverter<ulong, TLink>.Default;

        private readonly ILinks<TLink> _links;
        private readonly ILinks<TLink> _sequences;
        private HashSet<KeyValuePair<IList<TLink>, IList<TLink>>> _groups;
        private BitString _visited;

        private class ItemEquilityComparer : IEqualityComparer<KeyValuePair<IList<TLink>, IList<TLink>>>
        {
            private readonly IListEqualityComparer<TLink> _listComparer;

            /// <summary>
            /// <para>
            /// Initializes a new <see cref="ItemEquilityComparer"/> instance.
            /// </para>
            /// <para></para>
            /// </summary>
            public ItemEquilityComparer() => _listComparer = Default<IListEqualityComparer<TLink>>.Instance;

            /// <summary>
            /// <para>
            /// Determines whether this instance equals.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="left">
            /// <para>The left.</para>
            /// <para></para>
            /// </param>
            /// <param name="right">
            /// <para>The right.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The bool</para>
            /// <para></para>
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(KeyValuePair<IList<TLink>, IList<TLink>> left, KeyValuePair<IList<TLink>, IList<TLink>> right) => _listComparer.Equals(left.Key, right.Key) && _listComparer.Equals(left.Value, right.Value);

            /// <summary>
            /// <para>
            /// Gets the hash code using the specified pair.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="pair">
            /// <para>The pair.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The int</para>
            /// <para></para>
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(KeyValuePair<IList<TLink>, IList<TLink>> pair) => (_listComparer.GetHashCode(pair.Key), _listComparer.GetHashCode(pair.Value)).GetHashCode();
        }

        private class ItemComparer : IComparer<KeyValuePair<IList<TLink>, IList<TLink>>>
        {
            private readonly IListComparer<TLink> _listComparer;

            /// <summary>
            /// <para>
            /// Initializes a new <see cref="ItemComparer"/> instance.
            /// </para>
            /// <para></para>
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ItemComparer() => _listComparer = Default<IListComparer<TLink>>.Instance;

            /// <summary>
            /// <para>
            /// Compares the left.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="left">
            /// <para>The left.</para>
            /// <para></para>
            /// </param>
            /// <param name="right">
            /// <para>The right.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The intermediate result.</para>
            /// <para></para>
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(KeyValuePair<IList<TLink>, IList<TLink>> left, KeyValuePair<IList<TLink>, IList<TLink>> right)
            {
                var intermediateResult = _listComparer.Compare(left.Key, right.Key);
                if (intermediateResult == 0)
                {
                    intermediateResult = _listComparer.Compare(left.Value, right.Value);
                }
                return intermediateResult;
            }
        }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DuplicateSegmentsProvider"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="sequences">
        /// <para>A sequences.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DuplicateSegmentsProvider(ILinks<TLink> links, ILinks<TLink> sequences)
            : base(minimumStringSegmentLength: 2)
        {
            _links = links;
            _sequences = sequences;
        }

        /// <summary>
        /// <para>
        /// Gets this instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The result list.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IList<KeyValuePair<IList<TLink>, IList<TLink>>> Get()
        {
            _groups = new HashSet<KeyValuePair<IList<TLink>, IList<TLink>>>(Default<ItemEquilityComparer>.Instance);
            var links = _links;
            var count = links.Count();
            _visited = new BitString(_addressToInt64Converter.Convert(count) + 1L);
            links.Each(link =>
            {
                var linkIndex = links.GetIndex(link);
                var linkBitIndex = _addressToInt64Converter.Convert(linkIndex);
                var constants = links.Constants;
                if (!_visited.Get(linkBitIndex))
                {
                    var sequenceElements = new List<TLink>();
                    var filler = new ListFiller<TLink, TLink>(sequenceElements, constants.Break);
                    _sequences.Each(filler.AddSkipFirstAndReturnConstant, new LinkAddress<TLink>(linkIndex));
                    if (sequenceElements.Count > 2)
                    {
                        WalkAll(sequenceElements);
                    }
                }
                return constants.Continue;
            });
            var resultList = _groups.ToList();
            var comparer = Default<ItemComparer>.Instance;
            resultList.Sort(comparer);
#if DEBUG
            foreach (var item in resultList)
            {
                PrintDuplicates(item);
            }
#endif
            return resultList;
        }

        /// <summary>
        /// <para>
        /// Creates the segment using the specified elements.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="elements">
        /// <para>The elements.</para>
        /// <para></para>
        /// </param>
        /// <param name="offset">
        /// <para>The offset.</para>
        /// <para></para>
        /// </param>
        /// <param name="length">
        /// <para>The length.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A segment of t link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Segment<TLink> CreateSegment(IList<TLink> elements, int offset, int length) => new Segment<TLink>(elements, offset, length);

        /// <summary>
        /// <para>
        /// Ons the dublicate found using the specified segment.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="segment">
        /// <para>The segment.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnDublicateFound(Segment<TLink> segment)
        {
            var duplicates = CollectDuplicatesForSegment(segment);
            if (duplicates.Count > 1)
            {
                _groups.Add(new KeyValuePair<IList<TLink>, IList<TLink>>(segment.ToArray(), duplicates));
            }
        }

        /// <summary>
        /// <para>
        /// Collects the duplicates for segment using the specified segment.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="segment">
        /// <para>The segment.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The duplicates.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<TLink> CollectDuplicatesForSegment(Segment<TLink> segment)
        {
            var duplicates = new List<TLink>();
            var readAsElement = new HashSet<TLink>();
            var restrictions = segment.ShiftRight();
            var constants = _links.Constants;
            restrictions[0] = constants.Any;
            _sequences.Each(sequence =>
            {
                var sequenceIndex = sequence[constants.IndexPart];
                duplicates.Add(sequenceIndex);
                readAsElement.Add(sequenceIndex);
                return constants.Continue;
            }, restrictions);
            if (duplicates.Any(x => _visited.Get(_addressToInt64Converter.Convert(x))))
            {
                return new List<TLink>();
            }
            foreach (var duplicate in duplicates)
            {
                var duplicateBitIndex = _addressToInt64Converter.Convert(duplicate);
                _visited.Set(duplicateBitIndex);
            }
            if (_sequences is Sequences sequencesExperiments)
            {
                var partiallyMatched = sequencesExperiments.GetAllPartiallyMatchingSequences4((HashSet<ulong>)(object)readAsElement, (IList<ulong>)segment);
                foreach (var partiallyMatchedSequence in partiallyMatched)
                {
                    var sequenceIndex = _uInt64ToAddressConverter.Convert(partiallyMatchedSequence);
                    duplicates.Add(sequenceIndex);
                }
            }
            duplicates.Sort();
            return duplicates;
        }

        /// <summary>
        /// <para>
        /// Prints the duplicates using the specified duplicates item.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="duplicatesItem">
        /// <para>The duplicates item.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PrintDuplicates(KeyValuePair<IList<TLink>, IList<TLink>> duplicatesItem)
        {
            if (!(_links is ILinks<ulong> ulongLinks))
            {
                return;
            }
            var duplicatesKey = duplicatesItem.Key;
            var keyString = UnicodeMap.FromLinksToString((IList<ulong>)duplicatesKey);
            Console.WriteLine($"> {keyString} ({string.Join(", ", duplicatesKey)})");
            var duplicatesList = duplicatesItem.Value;
            for (int i = 0; i < duplicatesList.Count; i++)
            {
                var sequenceIndex = _addressToUInt64Converter.Convert(duplicatesList[i]);
                var formatedSequenceStructure = ulongLinks.FormatStructure(sequenceIndex, x => Point<ulong>.IsPartialPoint(x), (sb, link) => _ = UnicodeMap.IsCharLink(link.Index) ? sb.Append(UnicodeMap.FromLinkToChar(link.Index)) : sb.Append(link.Index));
                Console.WriteLine(formatedSequenceStructure);
                var sequenceString = UnicodeMap.FromSequenceLinkToString(sequenceIndex, ulongLinks);
                Console.WriteLine(sequenceString);
            }
            Console.WriteLine();
        }
    }
}