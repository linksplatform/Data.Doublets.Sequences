using System.Numerics;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Numbers.Raw;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Numbers.Rational
{
    /// <summary>
    /// <para>
    /// Represents the rational to decimal converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksDecoratorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{TLinkAddress, decimal}"/>
    public class RationalToDecimalConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<TLinkAddress, decimal> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        /// <summary>
        /// <para>
        /// The raw number sequence to big integer converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly RawNumberSequenceToBigIntegerConverter<TLinkAddress> RawNumberSequenceToBigIntegerConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="RationalToDecimalConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="rawNumberSequenceToBigIntegerConverter">
        /// <para>A raw number sequence to big integer converter.</para>
        /// <para></para>
        /// </param>
        public RationalToDecimalConverter(ILinks<TLinkAddress> links, RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter) : base(links)
        {
            RawNumberSequenceToBigIntegerConverter = rawNumberSequenceToBigIntegerConverter;
        }

        /// <summary>
        /// <para>
        /// Converts the rational number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="rationalNumber">
        /// <para>The rational number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The decimal</para>
        /// <para></para>
        /// </returns>
        public decimal Convert(TLinkAddress rationalNumber)
        {
            var numerator = (decimal)RawNumberSequenceToBigIntegerConverter.Convert(_links.GetSource(rationalNumber));
            var denominator = (decimal)RawNumberSequenceToBigIntegerConverter.Convert(_links.GetTarget(rationalNumber));
            return numerator / denominator;
        }
    }
}
