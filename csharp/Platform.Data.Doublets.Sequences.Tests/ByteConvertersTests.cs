using System;
using System.Collections.Generic;
using System.Numerics;
using Platform.Collections.Lists;
using Platform.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Numbers.Raw;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Numbers.Byte;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Numbers.Raw;
using Platform.Memory;
using Platform.Numbers;
using Xunit;
using TLinkAddress = System.UInt32;

namespace Platform.Data.Doublets.Sequences.Tests
{
    public class ByteConvertersTests
    {
        private readonly ILinks<TLinkAddress> Storage;
        private static readonly AddressToRawNumberConverter<TLinkAddress> _addressToRawNumberConverter = new();
        private static readonly RawNumberToAddressConverter<TLinkAddress> _rawNumberToAddressConverter = new();
        private readonly BalancedVariantConverter<TLinkAddress> _listToSequenceConverter;
        private readonly ByteListToRawSequenceConverter<TLinkAddress> _byteListToRawSequenceConverter;
        private readonly RawSequenceToByteListConverter<TLinkAddress> _rawSequenceToByteListConverter;

        public ByteConvertersTests()
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            var storageFileName = new IO.TemporaryFile().Filename;
            var storageMemory = new FileMappedResizableDirectMemory(storageFileName);
            Storage = new UnitedMemoryLinks<TLinkAddress>(storageMemory, UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
            _listToSequenceConverter = new BalancedVariantConverter<TLinkAddress>(Storage);
            TLinkAddress zero = default;
            TLinkAddress one = Arithmetic.Increment(zero);
            var type = Storage.GetOrCreate(one, one);
            var typeIndex = type;
            var unicodeSymbolType = Storage.GetOrCreate(type, Arithmetic.Increment(ref typeIndex));
            var unicodeSequenceType = Storage.GetOrCreate(type, Arithmetic.Increment(ref typeIndex));
            BalancedVariantConverter<TLinkAddress> balancedVariantConverter = new(Storage);
            AddressToRawNumberConverter<TLinkAddress> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLinkAddress> rawNumberToAddressConverter = new();
            TargetMatcher<TLinkAddress> unicodeSymbolCriterionMatcher = new(Storage, unicodeSymbolType);
            TargetMatcher<TLinkAddress> unicodeSequenceCriterionMatcher = new(Storage, unicodeSequenceType);
            CharToUnicodeSymbolConverter<TLinkAddress> charToUnicodeSymbolConverter =
                new(Storage, addressToRawNumberConverter, unicodeSymbolType);
            UnicodeSymbolToCharConverter<TLinkAddress> unicodeSymbolToCharConverter =
                new(Storage, rawNumberToAddressConverter, unicodeSymbolCriterionMatcher);
            var stringToUnicodeSequenceConverter = new StringToUnicodeSequenceConverter<TLinkAddress>(Storage, charToUnicodeSymbolConverter,
                balancedVariantConverter, unicodeSequenceType);
            _byteListToRawSequenceConverter = new ByteListToRawSequenceConverter<TLinkAddress>(Storage, _addressToRawNumberConverter, _rawNumberToAddressConverter, _listToSequenceConverter, stringToUnicodeSequenceConverter);
            _rawSequenceToByteListConverter = new RawSequenceToByteListConverter<TLinkAddress>(Storage, _rawNumberToAddressConverter, _listToSequenceConverter, stringToUnicodeSequenceConverter);
        }
        
        // [InlineData(new byte[]{})]
        [InlineData(new byte[]{0})]
        [InlineData(new byte[]{0, 0})]
        [InlineData(new byte[]{0, 0, 0, 0})]
        [InlineData(new byte[]{1})]
        [InlineData(new byte[]{1, 1})]
        [InlineData(new byte[]{1, 1, 1, 1})]
        [InlineData(new byte[]{1, 1, 1, 1, 1, 1})]
        [InlineData(new byte[]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1})]
        [InlineData(new byte[]{255, 255})]
        [InlineData(new byte[]{255, 255, 255, 255, 255})]
        [Theory]
        public void Test(byte[] byteArray)
        {
            var byteListRawSequence = _byteListToRawSequenceConverter.Convert(byteArray);
            var byteListFromConverter = _rawSequenceToByteListConverter.Convert(byteListRawSequence);
            Assert.Equal(byteArray, byteListFromConverter.ToArray());
        }
    }
}
