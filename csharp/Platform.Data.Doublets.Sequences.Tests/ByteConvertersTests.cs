using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Platform.Collections.Lists;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Numbers.Raw;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Numbers.Byte;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Numbers.Raw;
using Platform.Memory;
using Platform.Numbers;
using Platform.Reflection;
using Xunit;
using TLinkAddress = System.UInt16;

namespace Platform.Data.Doublets.Sequences.Tests
{
    public class ByteConvertersTests
    {
        private readonly ILinks<TLinkAddress> Storage;
        private static readonly AddressToRawNumberConverter<TLinkAddress> _addressToRawNumberConverter = new();
        private static readonly RawNumberToAddressConverter<TLinkAddress> _rawNumberToAddressConverter = new();
        private readonly BalancedVariantConverter<TLinkAddress> _listToSequenceConverter;
        private readonly BytesToRawNumberSequenceConverter<TLinkAddress> _bytesToRawNumberSequenceConverter;
        private readonly RawNumberSequenceToBytesConverter<TLinkAddress> _rawNumberSequenceToBytesConverter;
        
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
            RightSequenceWalker<TLinkAddress> unicodeSymbolSequenceWalker = new(Storage, new DefaultStack<TLinkAddress>(), unicodeSymbolCriterionMatcher.IsMatched);
            UnicodeSequenceToStringConverter<TLinkAddress> unicodeSequenceToStringConverter = new UnicodeSequenceToStringConverter<TLinkAddress>(Storage, unicodeSequenceCriterionMatcher, unicodeSymbolSequenceWalker, unicodeSymbolToCharConverter, unicodeSequenceType);
            _bytesToRawNumberSequenceConverter = new BytesToRawNumberSequenceConverter<TLinkAddress>(Storage, _addressToRawNumberConverter, _rawNumberToAddressConverter, _listToSequenceConverter, stringToUnicodeSequenceConverter);
            _rawNumberSequenceToBytesConverter = new RawNumberSequenceToBytesConverter<TLinkAddress>(Storage, _rawNumberToAddressConverter, _listToSequenceConverter, stringToUnicodeSequenceConverter, unicodeSequenceToStringConverter);
        }

        private static byte[] GetRandomArray(int length)
        {
            byte[] array = new byte[length];
            new System.Random(61267).NextBytes(array);
            return array;
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
        public void FixedArraysTest(byte[] byteArray)
        {
            Test(byteArray);
        }
        
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(26)]
        [InlineData(27)]
        [InlineData(28)]
        [InlineData(29)]
        [InlineData(30)]
        [InlineData(31)]
        [InlineData(32)]
        [Theory]
        public void RandomArrayTest(int length)
        {
            var byteArray = GetRandomArray(length);
            Test(byteArray);
        }

        public void Test(byte[] byteArray)
        {
            var byteListRawSequence = _bytesToRawNumberSequenceConverter.Convert(byteArray);
            Console.WriteLine();
            var byteListFromConverter = _rawNumberSequenceToBytesConverter.Convert(byteListRawSequence);
            Console.WriteLine("Original");
            foreach (var b in byteArray)
            {
                Console.WriteLine(TestExtensions.PrettifyBinary<byte>(Convert.ToString(b, 2)));
            }
            Console.WriteLine();
            Console.WriteLine("From converter:");
            foreach (var b in byteListFromConverter)
            {
                Console.WriteLine(TestExtensions.PrettifyBinary<byte>(Convert.ToString(b, 2)));
            }
            Assert.Equal(byteArray, byteListFromConverter.ToArray());                
        }
        

    }
}
