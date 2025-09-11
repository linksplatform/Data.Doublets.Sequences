using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Collections.Stacks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Walkers
{
    /// <summary>
    /// <para>
    /// Represents the cached sequence walker base.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="SequenceWalkerBase{TLinkAddress}"/>
    public abstract class CachedSequenceWalkerBase<TLinkAddress> : SequenceWalkerBase<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly Dictionary<TLinkAddress, bool> _isElementCache;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="CachedSequenceWalkerBase"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="stack">
        /// <para>A stack.</para>
        /// <para></para>
        /// </param>
        /// <param name="isElement">
        /// <para>A is element.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected CachedSequenceWalkerBase(ILinks<TLinkAddress> links, IStack<TLinkAddress> stack, Func<TLinkAddress, bool> isElement) : base(links, stack, isElement)
        {
            _isElementCache = new Dictionary<TLinkAddress, bool>();
        }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="CachedSequenceWalkerBase"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="stack">
        /// <para>A stack.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected CachedSequenceWalkerBase(ILinks<TLinkAddress> links, IStack<TLinkAddress> stack) : base(links, stack, links.IsPartialPoint)
        {
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
        protected override bool IsElement(TLinkAddress elementLink)
        {
            if (_isElementCache.TryGetValue(elementLink, out var cachedResult))
            {
                return cachedResult;
            }
            
            var result = base.IsElement(elementLink);
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
    }
}