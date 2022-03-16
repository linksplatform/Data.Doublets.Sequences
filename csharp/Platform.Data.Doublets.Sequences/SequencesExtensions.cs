using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform.Collections.Lists;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences
{
    /// <summary>
    /// <para>
    /// Represents the sequences extensions.
    /// </para>
    /// <para></para>
    /// </summary>
    public static class SequencesExtensions
    {
        /// <summary>
        /// <para>
        /// Creates the sequences.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <typeparam name="TLinkAddress">
        /// <para>The link.</para>
        /// <para></para>
        /// </typeparam>
        /// <param name="sequences">
        /// <para>The sequences.</para>
        /// <para></para>
        /// </param>
        /// <param name="groupedSequence">
        /// <para>The grouped sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TLinkAddress Create<TLinkAddress>(this ILinks<TLinkAddress> sequences, IList<TLinkAddress[]> groupedSequence)
        {
            var finalSequence = new TLinkAddress[groupedSequence.Count];
            for (var i = 0; i < finalSequence.Length; i++)
            {
                var part = groupedSequence[i];
                finalSequence[i] = part.Length == 1 ? part[0] : sequences.Create(part.ShiftRight());
            }
            return sequences.Create(finalSequence.ShiftRight());
        }

        /// <summary>
        /// <para>
        /// Returns the list using the specified sequences.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <typeparam name="TLinkAddress">
        /// <para>The link.</para>
        /// <para></para>
        /// </typeparam>
        /// <param name="sequences">
        /// <para>The sequences.</para>
        /// <para></para>
        /// </param>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The list.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<TLinkAddress> ToList<TLinkAddress>(this ILinks<TLinkAddress> sequences, TLinkAddress sequence)
        {
            var list = new List<TLinkAddress>();
            var filler = new ListFiller<TLinkAddress, TLinkAddress>(list, sequences.Constants.Break);
            sequences.Each(filler.AddSkipFirstAndReturnConstant, new LinkAddress<TLinkAddress>(sequence));
            return list;
        }
    }
}
