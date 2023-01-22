using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.HeightProviders;
using Platform.Data.Sequences;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences
{
    /// <summary>
    /// <para>
    /// Represents the default sequence appender.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="ISequenceAppender{TLinkAddress}"/>
    public class DefaultSequenceAppender<TLinkAddress> : LinksOperatorBase<TLinkAddress>, ISequenceAppender<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly IStack<TLinkAddress> _stack;
        private readonly ISequenceHeightProvider<TLinkAddress> _heightProvider;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DefaultSequenceAppender"/> instance.
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
        /// <param name="heightProvider">
        /// <para>A height provider.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DefaultSequenceAppender(ILinks<TLinkAddress> links, IStack<TLinkAddress> stack, ISequenceHeightProvider<TLinkAddress> heightProvider)
            : base(links)
        {
            _stack = stack;
            _heightProvider = heightProvider;
        }

        /// <summary>
        /// <para>
        /// Appends the sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <param name="appendant">
        /// <para>The appendant.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Append(TLinkAddress sequence, TLinkAddress appendant)
        {
            var cursor = sequence;
            var links = _links;
            while ((_heightProvider.Get(cursor) != TLinkAddress.Zero))
            {
                var source = links.GetSource(cursor);
                var target = links.GetTarget(cursor);
                if ((_heightProvider.Get(source) == _heightProvider.Get(target)))
                {
                    break;
                }
                else
                {
                    _stack.Push(source);
                    cursor = target;
                }
            }
            var left = cursor;
            var right = appendant;
            while (((cursor = _stack.PopOrDefault()) != links.Constants.Null))
            {
                right = links.GetOrCreate(left, right);
                left = cursor;
            }
            return links.GetOrCreate(left, right);
        }
    }
}
