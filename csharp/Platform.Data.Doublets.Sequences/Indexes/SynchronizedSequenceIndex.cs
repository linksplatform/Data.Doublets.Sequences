using System.Collections.Generic;
using System.Runtime.CompilerServices;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Indexes
{
    /// <summary>
    /// <para>
    /// Represents the synchronized sequence index.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="ISequenceIndex{TLinkAddress}"/>
    public class SynchronizedSequenceIndex<TLinkAddress> : ISequenceIndex<TLinkAddress>
    {
        private static readonly EqualityComparer<TLinkAddress> _equalityComparer = EqualityComparer<TLinkAddress>.Default;
        private readonly ISynchronizedLinks<TLinkAddress> _links;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="SynchronizedSequenceIndex"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SynchronizedSequenceIndex(ISynchronizedLinks<TLinkAddress> links) => _links = links;

        /// <summary>
        /// <para>
        /// Determines whether this instance add.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The indexed.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(IList<TLinkAddress>? sequence)
        {
            var indexed = true;
            var i = sequence.Count;
            var links = _links.Unsync;
            _links.SyncRoot.DoRead(() =>
            {
                while (--i >= 1 && (indexed = !_equalityComparer.Equals(links.SearchOrDefault(sequence[i - 1], sequence[i]), default))) { }
            });
            if (!indexed)
            {
                _links.SyncRoot.DoWrite(() =>
                {
                    for (; i >= 1; i--)
                    {
                        links.GetOrCreate(sequence[i - 1], sequence[i]);
                    }
                });
            }
            return indexed;
        }

        /// <summary>
        /// <para>
        /// Determines whether this instance might contain.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The bool</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MightContain(IList<TLinkAddress>? sequence)
        {
            var links = _links.Unsync;
            return _links.SyncRoot.DoRead(() =>
            {
                var indexed = true;
                var i = sequence.Count;
                while (--i >= 1 && (indexed = !_equalityComparer.Equals(links.SearchOrDefault(sequence[i - 1], sequence[i]), default))) { }
                return indexed;
            });
        }
    }
}
