using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Interfaces;
using Platform.Numbers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.HeightProviders
{
    /// <summary>
    /// <para>
    /// Represents the default sequence right height provider.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="ISequenceHeightProvider{TLinkAddress}"/>
    public class DefaultSequenceRightHeightProvider<TLinkAddress> : LinksOperatorBase<TLinkAddress>, ISequenceHeightProvider<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly ICriterionMatcher<TLinkAddress> _elementMatcher;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DefaultSequenceRightHeightProvider"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="elementMatcher">
        /// <para>A element matcher.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DefaultSequenceRightHeightProvider(ILinks<TLinkAddress> links, ICriterionMatcher<TLinkAddress> elementMatcher) : base(links) => _elementMatcher = elementMatcher;

        /// <summary>
        /// <para>
        /// Gets the sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The height.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Get(TLinkAddress sequence)
        {
            var height = TLinkAddress.Zero;
            var pairOrElement = sequence;
            while (!_elementMatcher.IsMatched(pairOrElement))
            {
                pairOrElement = _links.GetTarget(pairOrElement);
                height = Arithmetic.Increment(height);
            }
            return height;
        }
    }
}
