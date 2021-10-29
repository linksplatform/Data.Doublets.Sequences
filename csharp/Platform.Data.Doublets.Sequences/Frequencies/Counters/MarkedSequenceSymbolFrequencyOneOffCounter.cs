using System.Runtime.CompilerServices;
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Frequencies.Counters
{
    /// <summary>
    /// <para>
    /// Represents the marked sequence symbol frequency one off counter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="SequenceSymbolFrequencyOneOffCounter{TLink}"/>
    public class MarkedSequenceSymbolFrequencyOneOffCounter<TLink> : SequenceSymbolFrequencyOneOffCounter<TLink>
    {
        /// <summary>
        /// <para>
        /// The marked sequence matcher.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly ICriterionMatcher<TLink> _markedSequenceMatcher;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="MarkedSequenceSymbolFrequencyOneOffCounter"/> instance.
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
        /// <param name="sequenceLink">
        /// <para>A sequence link.</para>
        /// <para></para>
        /// </param>
        /// <param name="symbol">
        /// <para>A symbol.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MarkedSequenceSymbolFrequencyOneOffCounter(ILinks<TLink> links, ICriterionMatcher<TLink> markedSequenceMatcher, TLink sequenceLink, TLink symbol)
            : base(links, sequenceLink, symbol)
            => _markedSequenceMatcher = markedSequenceMatcher;

        /// <summary>
        /// <para>
        /// Counts this instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TLink Count()
        {
            if (!_markedSequenceMatcher.IsMatched(_sequenceLink))
            {
                return default;
            }
            return base.Count();
        }
    }
}
