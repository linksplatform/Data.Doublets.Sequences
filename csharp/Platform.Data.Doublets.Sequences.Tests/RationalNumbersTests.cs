using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Numbers.Rational;
using Platform.Data.Doublets.Numbers.Raw;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Numbers.Raw;
using Platform.Memory;
using Xunit;
using TLink = System.UInt64;

namespace Platform.Data.Doublets.Sequences.Tests
{
    /// <summary>
    /// <para>
    /// Represents the rational numbers tests.
    /// </para>
    /// <para></para>
    /// </summary>
    public class RationalNumbersTests
    {
        /// <summary>
        /// <para>
        /// Creates the links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new IO.TemporaryFile());

        /// <summary>
        /// <para>
        /// Creates the links using the specified data db filename.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <typeparam name="TLink">
        /// <para>The link.</para>
        /// <para></para>
        /// </typeparam>
        /// <param name="dataDbFilename">
        /// <para>The data db filename.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public ILinks<TLink> CreateLinks<TLink>(string dataDbFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        /// <summary>
        /// <para>
        /// Tests that decimal min value test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void DecimalMinValueTest()
        {
            const decimal @decimal = decimal.MinValue;
            var links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
           Assert.Equal(@decimal, decimalFromRational);
        }
        
        /// <summary>
        /// <para>
        /// Tests that decimal max value test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void DecimalMaxValueTest()
        {
            const decimal @decimal = decimal.MaxValue;
            var links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        /// <summary>
        /// <para>
        /// Tests that decimal positive half test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void DecimalPositiveHalfTest()
        {
            const decimal @decimal = 0.5M;
            var links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        /// <summary>
        /// <para>
        /// Tests that decimal negative half test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void DecimalNegativeHalfTest()
        {
            const decimal @decimal = -0.5M;
            var links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        /// <summary>
        /// <para>
        /// Tests that decimal one test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void DecimalOneTest()
        {
            const decimal @decimal = 1;
            var links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        /// <summary>
        /// <para>
        /// Tests that decimal minus one test.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void DecimalMinusOneTest()
        {
            const decimal @decimal = -1;
            var links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
    }
}
