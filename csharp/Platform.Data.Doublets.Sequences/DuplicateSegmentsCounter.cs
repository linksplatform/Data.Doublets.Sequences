using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences
{
    /// <summary>
    /// <para>
    /// Represents the duplicate segments counter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="ICounter{int}"/>
    public class DuplicateSegmentsCounter<TLink> : ICounter<int>
    {
        private readonly IProvider<IList<KeyValuePair<IList<TLink>?, IList<TLink>>>> _duplicateFragmentsProvider;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DuplicateSegmentsCounter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="duplicateFragmentsProvider">
        /// <para>A duplicate fragments provider.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DuplicateSegmentsCounter(IProvider<IList<KeyValuePair<IList<TLink>, IList<TLink>>>> duplicateFragmentsProvider) => _duplicateFragmentsProvider = duplicateFragmentsProvider;

        /// <summary>
        /// <para>
        /// Counts this instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The int</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => _duplicateFragmentsProvider.Get().Sum(x => x.Value.Count);
    }
}
