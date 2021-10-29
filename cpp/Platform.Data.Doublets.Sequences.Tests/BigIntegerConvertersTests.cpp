

using TLink = std::uint64_t;

namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(BigIntegerConvertersTests)
    {
        public: ILinks<TLink> CreateLinks() { return CreateLinks<TLink>(IO.TemporaryFile(); });

        public: ILinks<TLink> CreateLinks<TLink>(std::string dataDbFilename)
        {
            auto linksConstants = LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return UnitedMemoryLinks<TLink>(FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        
        public: TEST_METHOD(DecimalMaxValueTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            auto bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            auto bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert::AreEqual(bigInteger, bigIntFromSequence);
        }
        
        public: TEST_METHOD(DecimalMinValueTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            auto bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            auto bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert::AreEqual(bigInteger, bigIntFromSequence);
        }
        
        public: TEST_METHOD(ZeroValueTest)
        {
            auto links = CreateLinks();
            BigInteger bigInteger = new(0);
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            auto bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            auto bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert::AreEqual(bigInteger, bigIntFromSequence);
        }
        
        public: TEST_METHOD(OneValueTest)
        {
            auto links = CreateLinks();
            BigInteger bigInteger = new(1);
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> listToSequenceConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, listToSequenceConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            auto bigIntSequence = bigIntegerToRawNumberSequenceConverter.Convert(bigInteger);
            auto bigIntFromSequence = rawNumberSequenceToBigIntegerConverter.Convert(bigIntSequence);
            Assert::AreEqual(bigInteger, bigIntFromSequence);
        }
    };
}
