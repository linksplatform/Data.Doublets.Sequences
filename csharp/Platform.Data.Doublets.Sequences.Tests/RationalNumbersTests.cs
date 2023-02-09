using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Numbers.Rational;
using Platform.Data.Doublets.Sequences.Numbers.Raw;
using Platform.Data.Numbers.Raw;
using Platform.Memory;
using Xunit;
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Sequences.Tests
{
    public class RationalNumbersTests
    {
        public ILinks<TLinkAddress> CreateLinks() => CreateLinks(new IO.TemporaryFile());

        public ILinks<TLinkAddress> CreateLinks(string dataDbFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        [Fact]
        public void DecimalMinValueTest()
        {
            const decimal @decimal = decimal.MinValue;
            var links = CreateLinks();
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLinkAddress> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLinkAddress> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
           Assert.Equal(@decimal, decimalFromRational);
        }
        
        [Fact]
        public void DecimalMaxValueTest()
        {
            const decimal @decimal = decimal.MaxValue;
            var links = CreateLinks();
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLinkAddress> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLinkAddress> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        [Fact]
        public void DecimalPositiveHalfTest()
        {
            const decimal @decimal = 0.5M;
            var links = CreateLinks();
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLinkAddress> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLinkAddress> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        [Fact]
        public void DecimalNegativeHalfTest()
        {
            const decimal @decimal = -0.5M;
            var links = CreateLinks();
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLinkAddress> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLinkAddress> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        [Fact]
        public void DecimalOneTest()
        {
            const decimal @decimal = 1;
            var links = CreateLinks();
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLinkAddress> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLinkAddress> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
        
        [Fact]
        public void DecimalMinusOneTest()
        {
            const decimal @decimal = -1;
            var links = CreateLinks();
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLinkAddress> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLinkAddress> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            var rationalNumber = decimalToRationalConverter.Convert(@decimal);
            var decimalFromRational = rationalToDecimalConverter.Convert(rationalNumber);
            Assert.Equal(@decimal, decimalFromRational);
        }
    }
}
