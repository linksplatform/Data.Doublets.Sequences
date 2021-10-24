namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(UnicodeConvertersTests)
    {
        public: TEST_METHOD(CharAndUnaryNumberUnicodeSymbolConvertersTest)
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                auto meaningRoot = links.CreatePoint();
                auto one = links.CreateAndUpdate(meaningRoot, links.Constants.Itself);
                auto powerOf2ToUnaryNumberConverter = PowerOf2ToUnaryNumberConverter<std::uint64_t>(links, one);
                auto addressToUnaryNumberConverter = AddressToUnaryNumberConverter<std::uint64_t>(links, powerOf2ToUnaryNumberConverter);
                auto unaryNumberToAddressConverter = UnaryNumberToAddressOrOperationConverter<std::uint64_t>(links, powerOf2ToUnaryNumberConverter);
                TestCharAndUnicodeSymbolConverters(links, meaningRoot, addressToUnaryNumberConverter, unaryNumberToAddressConverter);
            }
        }

        public: TEST_METHOD(CharAndRawNumberUnicodeSymbolConvertersTest)
        {
            using (auto scope = Scope<Types<HeapResizableDirectMemory, UnitedMemoryLinks<std::uint64_t>>>())
            {
                auto links = scope.Use<ILinks<std::uint64_t>>();
                auto meaningRoot = links.CreatePoint();
                auto addressToRawNumberConverter = AddressToRawNumberConverter<std::uint64_t>();
                auto rawNumberToAddressConverter = RawNumberToAddressConverter<std::uint64_t>();
                TestCharAndUnicodeSymbolConverters(links, meaningRoot, addressToRawNumberConverter, rawNumberToAddressConverter);
            }
        }

        private: static void TestCharAndUnicodeSymbolConverters(ILinks<std::uint64_t> &links, std::uint64_t meaningRoot, IConverter<std::uint64_t> &addressToNumberConverter, IConverter<std::uint64_t> &numberToAddressConverter)
        {
            auto unicodeSymbolMarker = links.CreateAndUpdate(meaningRoot, links.Constants.Itself);
            auto charToUnicodeSymbolConverter = CharToUnicodeSymbolConverter<std::uint64_t>(links, addressToNumberConverter, unicodeSymbolMarker);
            auto originalCharacter = 'H';
            auto characterLink = charToUnicodeSymbolConverter.Convert(originalCharacter);
            auto unicodeSymbolCriterionMatcher = TargetMatcher<std::uint64_t>(links, unicodeSymbolMarker);
            auto unicodeSymbolToCharConverter = UnicodeSymbolToCharConverter<std::uint64_t>(links, numberToAddressConverter, unicodeSymbolCriterionMatcher);
            auto resultingCharacter = unicodeSymbolToCharConverter.Convert(characterLink);
            Assert::AreEqual(originalCharacter, resultingCharacter);
        }

        public: TEST_METHOD(StringAndUnicodeSequenceConvertersTest)
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;

                auto itself = links.Constants.Itself;

                auto meaningRoot = links.CreatePoint();
                auto unaryOne = links.CreateAndUpdate(meaningRoot, itself);
                auto unicodeSymbolMarker = links.CreateAndUpdate(meaningRoot, itself);
                auto unicodeSequenceMarker = links.CreateAndUpdate(meaningRoot, itself);
                auto frequencyMarker = links.CreateAndUpdate(meaningRoot, itself);
                auto frequencyPropertyMarker = links.CreateAndUpdate(meaningRoot, itself);

                auto powerOf2ToUnaryNumberConverter = PowerOf2ToUnaryNumberConverter<std::uint64_t>(links, unaryOne);
                auto addressToUnaryNumberConverter = AddressToUnaryNumberConverter<std::uint64_t>(links, powerOf2ToUnaryNumberConverter);
                auto charToUnicodeSymbolConverter = CharToUnicodeSymbolConverter<std::uint64_t>(links, addressToUnaryNumberConverter, unicodeSymbolMarker);

                auto unaryNumberToAddressConverter = UnaryNumberToAddressOrOperationConverter<std::uint64_t>(links, powerOf2ToUnaryNumberConverter);
                auto unaryNumberIncrementer = UnaryNumberIncrementer<std::uint64_t>(links, unaryOne);
                auto frequencyIncrementer = FrequencyIncrementer<std::uint64_t>(links, frequencyMarker, unaryOne, unaryNumberIncrementer);
                auto frequencyPropertyOperator = PropertyOperator<std::uint64_t>(links, frequencyPropertyMarker, frequencyMarker);
                auto index = FrequencyIncrementingSequenceIndex<std::uint64_t>(links, frequencyPropertyOperator, frequencyIncrementer);
                auto linkToItsFrequencyNumberConverter = LinkToItsFrequencyNumberConveter<std::uint64_t>(links, frequencyPropertyOperator, unaryNumberToAddressConverter);
                auto sequenceToItsLocalElementLevelsConverter = SequenceToItsLocalElementLevelsConverter<std::uint64_t>(links, linkToItsFrequencyNumberConverter);
                auto optimalVariantConverter = OptimalVariantConverter<std::uint64_t>(links, sequenceToItsLocalElementLevelsConverter);

                auto stringToUnicodeSequenceConverter = StringToUnicodeSequenceConverter<std::uint64_t>(links, charToUnicodeSymbolConverter, index, optimalVariantConverter, unicodeSequenceMarker);

                auto originalString = "Hello";

                auto unicodeSequenceLink = stringToUnicodeSequenceConverter.Convert(originalString);
                
                auto unicodeSymbolCriterionMatcher = TargetMatcher<std::uint64_t>(links, unicodeSymbolMarker);
                auto unicodeSymbolToCharConverter = UnicodeSymbolToCharConverter<std::uint64_t>(links, unaryNumberToAddressConverter, unicodeSymbolCriterionMatcher);

                auto unicodeSequenceCriterionMatcher = TargetMatcher<std::uint64_t>(links, unicodeSequenceMarker);

                auto sequenceWalker = LeveledSequenceWalker<std::uint64_t>(links, unicodeSymbolCriterionMatcher.IsMatched);

                auto unicodeSequenceToStringConverter = UnicodeSequenceToStringConverter<std::uint64_t>(links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter);

                auto resultingString = unicodeSequenceToStringConverter.Convert(unicodeSequenceLink);

                Assert::AreEqual(originalString, resultingString);
            }
        }
    };
}
