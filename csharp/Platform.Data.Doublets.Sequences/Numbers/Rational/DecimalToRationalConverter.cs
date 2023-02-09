using System.Numerics;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using System.Globalization;
using Platform.Data.Doublets.Numbers.Raw;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Numbers.Rational
{
    /// <summary>
    /// <para>
    /// Represents the decimal to rational converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksDecoratorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{decimal, TLinkAddress}"/>
    public class DecimalToRationalConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<decimal, TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>, IBitwiseOperators<TLinkAddress, TLinkAddress, TLinkAddress>, IShiftOperators<TLinkAddress, int, TLinkAddress>
    {
        /// <summary>
        /// <para>
        /// The big integer to raw number sequence converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly BigIntegerToRawNumberSequenceConverter<TLinkAddress> BigIntegerToRawNumberSequenceConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DecimalToRationalConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="bigIntegerToRawNumberSequenceConverter">
        /// <para>A big integer to raw number sequence converter.</para>
        /// <para></para>
        /// </param>
        public DecimalToRationalConverter(ILinks<TLinkAddress> links, BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter) : base(links)
        {
            BigIntegerToRawNumberSequenceConverter = bigIntegerToRawNumberSequenceConverter;
        }

        /// <summary>
        /// <para>
        /// Converts the decimal.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="@decimal">
        /// <para>The decimal.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLinkAddress Convert(decimal @decimal)
        {
            var decimalAsString = @decimal.ToString(CultureInfo.InvariantCulture);
            var dotPosition = decimalAsString.IndexOf('.');
            var decimalWithoutDots = decimalAsString;
            int digitsAfterDot = 0;
            if (dotPosition != -1)
            {
                decimalWithoutDots = decimalWithoutDots.Remove(dotPosition, 1);
                digitsAfterDot = decimalAsString.Length - 1 - dotPosition;
            }
            BigInteger denominator = new(System.Math.Pow(10, digitsAfterDot));
            BigInteger numerator = BigInteger.Parse(decimalWithoutDots);
            BigInteger greatestCommonDivisor;
            do
            {
                greatestCommonDivisor = BigInteger.GreatestCommonDivisor(numerator, denominator);
                numerator /= greatestCommonDivisor;
                denominator /= greatestCommonDivisor;
            }
            while (greatestCommonDivisor > 1);
            var numeratorLink = BigIntegerToRawNumberSequenceConverter.Convert(numerator);
            var denominatorLink = BigIntegerToRawNumberSequenceConverter.Convert(denominator);
            return _links.GetOrCreate(numeratorLink, denominatorLink);
        }
    }
}
