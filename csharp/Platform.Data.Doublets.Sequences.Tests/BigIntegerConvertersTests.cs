using System.Collections.Generic;
using System.Numerics;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Numbers.Raw;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Numbers.Raw;
using Platform.Memory;
using Xunit;
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Sequences.Tests
{
    public class BigIntegerConvertersTests
    {
        public ILinks<TLinkAddress> CreateLinks() => CreateLinks(new IO.TemporaryFile());

        public ILinks<TLinkAddress> CreateLinks(string dataDbFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        
        [Fact]
        public void DecimalMaxValueTest()
        {
            var links = CreateLinks();
            BigInteger bigInteger = new(decimal.MaxValue);
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
        
        [Fact]
        public void DecimalMinValueTest()
        {
            var links = CreateLinks();
            BigInteger bigInteger = new(decimal.MinValue);
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
        
        [Fact]
        public void ZeroValueTest()
        {
            var links = CreateLinks();
            BigInteger bigInteger = new(0);
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
        
        [Fact]
        public void OneValueTest()
        {
            var links = CreateLinks();
            BigInteger bigInteger = new(1);
            TLinkAddress negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> numberToAddressConverter = new();
            BalancedVariantConverter<TLinkAddress> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLinkAddress> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLinkAddress> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
    }
}
