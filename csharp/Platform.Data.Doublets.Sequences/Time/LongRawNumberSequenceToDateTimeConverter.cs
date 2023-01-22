using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Converters;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Time
{
    /// <summary>
    /// <para>
    /// Represents the long raw number sequence to date time converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="IConverter{TLinkAddress, DateTime}"/>
    public class LongRawNumberSequenceToDateTimeConverter<TLinkAddress> : IConverter<TLinkAddress, DateTime> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly IConverter<TLinkAddress, long> _longRawNumberConverterToInt64;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="LongRawNumberSequenceToDateTimeConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="longRawNumberConverterToInt64">
        /// <para>A long raw number converter to int 64.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LongRawNumberSequenceToDateTimeConverter(IConverter<TLinkAddress, long> longRawNumberConverterToInt64) => _longRawNumberConverterToInt64 = longRawNumberConverterToInt64;

        /// <summary>
        /// <para>
        /// Converts the source.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="source">
        /// <para>The source.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The date time</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime Convert(TLinkAddress source) => DateTime.FromFileTimeUtc(_longRawNumberConverterToInt64.Convert(source));
    }
}
