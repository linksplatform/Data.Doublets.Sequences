using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Numbers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Frequencies.Cache
{
    /// <summary>
    /// <para>
    /// Represents the link frequency.
    /// </para>
    /// <para></para>
    /// </summary>
    public class LinkFrequency<TLinkAddress> where TLinkAddress: struct, IUnsignedNumber<TLinkAddress>
    {
        /// <summary>
        /// <para>
        /// Gets or sets the frequency value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress Frequency { get; set; }
        /// <summary>
        /// <para>
        /// Gets or sets the link value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TLinkAddress Link { get; set; }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="LinkFrequency"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="frequency">
        /// <para>A frequency.</para>
        /// <para></para>
        /// </param>
        /// <param name="link">
        /// <para>A link.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LinkFrequency(TLinkAddress frequency, TLinkAddress link)
        {
            Frequency = frequency;
            Link = link;
        }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="LinkFrequency"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LinkFrequency() { }

        /// <summary>
        /// <para>
        /// Increments the frequency.
        /// </para>
        /// <para></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncrementFrequency() => Frequency = Frequency + TLinkAddress.One;

        /// <summary>
        /// <para>
        /// Decrements the frequency.
        /// </para>
        /// <para></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DecrementFrequency() => Frequency = Frequency - TLinkAddress.One;

        /// <summary>
        /// <para>
        /// Returns the string.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"F: {Frequency}, L: {Link}";
    }
}
