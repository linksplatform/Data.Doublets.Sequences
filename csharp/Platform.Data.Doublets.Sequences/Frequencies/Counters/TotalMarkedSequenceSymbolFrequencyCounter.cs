using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Frequencies.Counters
{
    /// <summary>
    /// <para>
    /// Represents the total marked sequence symbol frequency counter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="ICounter{TLinkAddress, TLinkAddress}"/>
    public class TotalMarkedSequenceSymbolFrequencyCounter<TLinkAddress> : ICounter<TLinkAddress, TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly ILinks<TLinkAddress> _links;
        private readonly ICriterionMatcher<TLinkAddress> _markedSequenceMatcher;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="TotalMarkedSequenceSymbolFrequencyCounter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="markedSequenceMatcher">
        /// <para>A marked sequence matcher.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TotalMarkedSequenceSymbolFrequencyCounter(ILinks<TLinkAddress> links, ICriterionMatcher<TLinkAddress> markedSequenceMatcher)
        {
            _links = links;
            _markedSequenceMatcher = markedSequenceMatcher;
        }

        /// <summary>
        /// <para>
        /// Counts the argument.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="argument">
        /// <para>The argument.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Count(TLinkAddress argument) => new TotalMarkedSequenceSymbolFrequencyOneOffCounter<TLinkAddress>(_links, _markedSequenceMatcher, argument).Count();
    }
}
