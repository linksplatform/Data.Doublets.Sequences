using System.Collections.Generic;
using System.Numerics;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Numbers.Raw;
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
        
        [Fact]
        public void UintBitShiftTest()
        {
            var links = new UnitedMemoryLinks<uint>(new HeapResizableDirectMemory());
            BigInteger bigInteger = new(uint.MaxValue);
            uint negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<uint> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<uint> numberToAddressConverter = new();
            BalancedVariantConverter<uint> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<uint> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<uint> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
        
        [Fact]
        public void UshortBitShiftTest()
        {
            var links = new UnitedMemoryLinks<ushort>(new HeapResizableDirectMemory());
            BigInteger bigInteger = new(ushort.MaxValue);
            ushort negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<ushort> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<ushort> numberToAddressConverter = new();
            BalancedVariantConverter<ushort> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<ushort> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<ushort> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
        
        [Fact]
        public void ByteBitShiftTest()
        {
            var links = new UnitedMemoryLinks<byte>(new HeapResizableDirectMemory());
            BigInteger bigInteger = new(byte.MaxValue);
            byte negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<byte> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<byte> numberToAddressConverter = new();
            BalancedVariantConverter<byte> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<byte> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<byte> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            var bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            var bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert.Equal(bigInteger, bigIntFromSequence);
        }
    }
}
