using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

//#define USEARRAYPOOL
#if USEARRAYPOOL
using Platform.Collections;
#endif

namespace Platform.Data.Doublets.Sequences.Walkers
{
    /// <summary>
    /// <para>
    /// Represents the cached leveled sequence walker.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="ISequenceWalker{TLinkAddress}"/>
    public class CachedLeveledSequenceWalker<TLinkAddress> : LinksOperatorBase<TLinkAddress>, ISequenceWalker<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly Func<TLinkAddress, bool> _isElement;
        private readonly Dictionary<TLinkAddress, bool> _isElementCache;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="CachedLeveledSequenceWalker"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="isElement">
        /// <para>A is element.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CachedLeveledSequenceWalker(ILinks<TLinkAddress> links, Func<TLinkAddress, bool> isElement) : base(links)
        {
            _isElement = isElement;
            _isElementCache = new Dictionary<TLinkAddress, bool>();
        }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="CachedLeveledSequenceWalker"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CachedLeveledSequenceWalker(ILinks<TLinkAddress> links) : base(links)
        {
            _isElement = _links.IsPartialPoint;
            _isElementCache = new Dictionary<TLinkAddress, bool>();
        }

        /// <summary>
        /// <para>
        /// Determines whether this instance is element with caching.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="elementLink">
        /// <para>The element link.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The bool</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsElement(TLinkAddress elementLink)
        {
            if (_isElementCache.TryGetValue(elementLink, out var cachedResult))
            {
                return cachedResult;
            }
            
            var result = _isElement(elementLink);
            _isElementCache[elementLink] = result;
            return result;
        }

        /// <summary>
        /// <para>
        /// Clears the element cache.
        /// </para>
        /// <para></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearCache() => _isElementCache.Clear();

        /// <summary>
        /// <para>
        /// Gets the current cache size.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The cache size.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCacheSize() => _isElementCache.Count;

        /// <summary>
        /// <para>
        /// Walks the sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>An enumerable of t link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TLinkAddress> Walk(TLinkAddress sequence) => ToArray(sequence);

        /// <summary>
        /// <para>
        /// Returns the array using the specified sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link array</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress[] ToArray(TLinkAddress sequence)
        {
            var length = 1;
            var array = new TLinkAddress[length];
            array[0] = sequence;
            if (IsElement(sequence))
            {
                return array;
            }
            bool hasElements;
            do
            {
                length *= 2;
#if USEARRAYPOOL
                var nextArray = ArrayPool.Allocate<ulong>(length);
#else
                var nextArray = new TLinkAddress[length];
#endif
                hasElements = false;
                for (var i = 0; i < array.Length; i++)
                {
                    var candidate = array[i];
                    if ((array[i] == TLinkAddress.One))
                    {
                        continue;
                    }
                    var doubletOffset = i * 2;
                    if (IsElement(candidate))
                    {
                        nextArray[doubletOffset] = candidate;
                    }
                    else
                    {
                        var links = _links;
                        var link = links.GetLink(candidate);
                        var linkSource = links.GetSource(link);
                        var linkTarget = links.GetTarget(link);
                        nextArray[doubletOffset] = linkSource;
                        nextArray[doubletOffset + 1] = linkTarget;
                        if (!hasElements)
                        {
                            hasElements = !(IsElement(linkSource) && IsElement(linkTarget));
                        }
                    }
                }
#if USEARRAYPOOL
                if (array.Length > 1)
                {
                    ArrayPool.Free(array);
                }
#endif
                array = nextArray;
            }
            while (hasElements);
            var filledElementsCount = CountFilledElements(array);
            if (filledElementsCount == array.Length)
            {
                return array;
            }
            else
            {
                return CopyFilledElements(array, filledElementsCount);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TLinkAddress[] CopyFilledElements(TLinkAddress[] array, int filledElementsCount)
        {
            var finalArray = new TLinkAddress[filledElementsCount];
            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if ((array[i] != TLinkAddress.Zero))
                {
                    finalArray[j] = array[i];
                    j++;
                }
            }
#if USEARRAYPOOL
                ArrayPool.Free(array);
#endif
            return finalArray;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountFilledElements(TLinkAddress[] array)
        {
            var count = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if ((array[i] != TLinkAddress.Zero))
                {
                    count++;
                }
            }
            return count;
        }
    }
}