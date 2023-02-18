using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Data.Doublets.Sequences.Unicode;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences
{
    /// <summary>
    /// <para>
    /// Represents the int 64 links extensions.
    /// </para>
    /// <para></para>
    /// </summary>
    public static class UInt64LinksExtensions
    {
        /// <summary>
        /// <para>
        /// Uses the unicode using the specified links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>The links.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UseUnicode<TLinkAddress>(this ILinks<TLinkAddress> links) where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
        {
            UnicodeMap<TLinkAddress>.InitNew(links);
        }
        
        #region Garbage Collection
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGarbage<TLinkAddress>(this ILinks<TLinkAddress> links, TLinkAddress link) where  TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool> 
        {
            return !links.IsPartialPoint(link) && links.Count(links.Constants.Any, link) == TLinkAddress.Zero && links.Count(links.Constants.Any, links.Constants.Any, link) == TLinkAddress.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearGarbage<TLinkAddress>(this ILinks<TLinkAddress> links, TLinkAddress link) where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
        {
            ClearGarbage(links, link, links.IsGarbage);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearGarbage<TLinkAddress>(this ILinks<TLinkAddress> links, TLinkAddress link, Func<TLinkAddress, bool> getIsGarbage) where  TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool> 
        {
            if (getIsGarbage(link))
            {
                var contents = new Link<TLinkAddress>(links.GetLink(link));
                links.Delete(link);
                links.ClearGarbage(contents.Source);
                links.ClearGarbage(contents.Target);
            }
        }

        #endregion
    }
}
