

using TLink = std::uint64_t;

namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(RationalNumbersTests)
    {
        public: ILinks<TLink> CreateLinks() { return CreateLinks<TLink>(IO.TemporaryFile(); });

        public: ILinks<TLink> CreateLinks<TLink>(std::string dataDbFilename)
        {
            auto linksConstants = LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return UnitedMemoryLinks<TLink>(FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public: TEST_METHOD(DecimalMinValueTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
           Assert::AreEqual(decimal, decimalFromRational);
        }
        
        public: TEST_METHOD(DecimalMaxValueTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            Assert::AreEqual(decimal, decimalFromRational);
        }
        
        public: TEST_METHOD(DecimalPositiveHalfTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            Assert::AreEqual(decimal, decimalFromRational);
        }
        
        public: TEST_METHOD(DecimalNegativeHalfTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            Assert::AreEqual(decimal, decimalFromRational);
        }
        
        public: TEST_METHOD(DecimalOneTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            Assert::AreEqual(decimal, decimalFromRational);
        }
        
        public: TEST_METHOD(DecimalMinusOneTest)
        {
            auto links = CreateLinks();
            TLink negativeNumberMarker = links.Create();
            AddressToRawNumberConverter<TLink> addressToRawNumberConverter = new();
            RawNumberToAddressConverter<TLink> numberToAddressConverter = new();
            BalancedVariantConverter<TLink> balancedVariantConverter = new(links);
            BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter = new(links, addressToRawNumberConverter, balancedVariantConverter, negativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter = new(links, numberToAddressConverter, negativeNumberMarker);
            DecimalToRationalConverter<TLink> decimalToRationalConverter = new(links, bigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter<TLink> rationalToDecimalConverter = new(links, rawNumberSequenceToBigIntegerConverter);
            Assert::AreEqual(decimal, decimalFromRational);
        }
    };
}
