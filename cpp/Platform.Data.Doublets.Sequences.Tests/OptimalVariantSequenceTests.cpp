namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(OptimalVariantSequenceTests)
    {
        private: inline static std::string _sequenceExample = "зеленела зелёная зелень";
        private: static readonly std::string _loremIpsumExample = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
Facilisi nullam vehicula ipsum a arcu cursus vitae congue mauris.
Et malesuada fames ac turpis egestas sed.
Eget velit aliquet sagittis id consectetur purus.
Dignissim cras tincidunt lobortis feugiat vivamus.
Vitae aliquet nec ullamcorper sit.
Lectus quam id leo in vitae.
Tortor dignissim convallis aenean et tortor at risus viverra adipiscing.
Sed risus ultricies tristique nulla aliquet enim tortor at auctor.
Integer eget aliquet nibh praesent tristique.
Vitae congue eu consequat ac felis donec et odio.
Tristique et egestas quis ipsum suspendisse.
Suspendisse potenti nullam ac tortor vitae purus faucibus ornare.
Nulla facilisi etiam dignissim diam quis enim lobortis scelerisque.
Imperdiet proin fermentum leo vel orci.
In ante metus dictum at tempor commodo.
Nisi lacus sed viverra tellus in.
Quam vulputate dignissim suspendisse in.
Elit scelerisque mauris pellentesque pulvinar pellentesque habitant morbi tristique senectus.
Gravida cum sociis natoque penatibus et magnis dis parturient.
Risus quis varius quam quisque id diam.
Congue nisi vitae suscipit tellus mauris a diam maecenas.
Eget nunc scelerisque viverra mauris in aliquam sem fringilla.
Pharetra vel turpis nunc eget lorem dolor sed viverra.
Mattis pellentesque id nibh tortor id aliquet.
Purus non enim praesent elementum facilisis leo vel.
Etiam sit amet nisl purus in mollis nunc sed.
Tortor at auctor urna nunc id cursus metus aliquam.
Volutpat odio facilisis mauris sit amet.
Turpis egestas pretium aenean pharetra magna ac placerat.
Fermentum dui faucibus in ornare quam viverra orci sagittis eu.
Porttitor leo a diam sollicitudin tempor id eu.
Volutpat sed cras ornare arcu dui.
Ut aliquam purus sit amet luctus venenatis lectus magna.
Aliquet risus feugiat in ante metus dictum at.
Mattis nunc sed blandit libero.
Elit pellentesque habitant morbi tristique senectus et netus.
Nibh sit amet commodo nulla facilisi nullam vehicula ipsum a.
Enim sit amet venenatis urna cursus eget nunc scelerisque viverra.
Amet venenatis urna cursus eget nunc scelerisque viverra mauris in.
Diam donec adipiscing tristique risus nec feugiat.
Pulvinar mattis nunc sed blandit libero volutpat.
Cras fermentum odio eu feugiat pretium nibh ipsum.
In nulla posuere sollicitudin aliquam ultrices sagittis orci a.
Mauris pellentesque pulvinar pellentesque habitant morbi tristique senectus et.
A iaculis at erat pellentesque.
Morbi blandit cursus risus at ultrices mi tempus imperdiet nulla.
Eget lorem dolor sed viverra ipsum nunc.
Leo a diam sollicitudin tempor id eu.
Interdum consectetur libero id faucibus nisl tincidunt eget nullam non.";

        public: TEST_METHOD(LinksBasedFrequencyStoredOptimalVariantSequenceTest)
        {
            using (auto scope = TempLinksTestScope(useSequences: false))
            {
                auto links = scope.Links;
                auto constants = links.Constants;

                links.UseUnicode();

                auto sequence = UnicodeMap.FromStringToLinkArray(_sequenceExample);

                auto meaningRoot = links.CreatePoint();
                auto unaryOne = links.CreateAndUpdate(meaningRoot, constants.Itself);
                auto frequencyMarker = links.CreateAndUpdate(meaningRoot, constants.Itself);
                auto frequencyPropertyMarker = links.CreateAndUpdate(meaningRoot, constants.Itself);

                auto unaryNumberToAddressConverter = UnaryNumberToAddressAddOperationConverter<std::uint64_t>(links, unaryOne);
                auto unaryNumberIncrementer = UnaryNumberIncrementer<std::uint64_t>(links, unaryOne);
                auto frequencyIncrementer = FrequencyIncrementer<std::uint64_t>(links, frequencyMarker, unaryOne, unaryNumberIncrementer);
                auto frequencyPropertyOperator = PropertyOperator<std::uint64_t>(links, frequencyPropertyMarker, frequencyMarker);
                auto index = FrequencyIncrementingSequenceIndex<std::uint64_t>(links, frequencyPropertyOperator, frequencyIncrementer);
                auto linkToItsFrequencyNumberConverter = LinkToItsFrequencyNumberConveter<std::uint64_t>(links, frequencyPropertyOperator, unaryNumberToAddressConverter);
                auto sequenceToItsLocalElementLevelsConverter = SequenceToItsLocalElementLevelsConverter<std::uint64_t>(links, linkToItsFrequencyNumberConverter);
                auto optimalVariantConverter = OptimalVariantConverter<std::uint64_t>(links, sequenceToItsLocalElementLevelsConverter);

                auto sequences = Sequences(links, SequencesOptions<std::uint64_t>() { Walker = LeveledSequenceWalker<std::uint64_t>(links) });

                ExecuteTest(sequences, sequence, sequenceToItsLocalElementLevelsConverter, index, optimalVariantConverter);
            }
        }

        public: TEST_METHOD(DictionaryBasedFrequencyStoredOptimalVariantSequenceTest)
        {
            using (auto scope = TempLinksTestScope(useSequences: false))
            {
                auto links = scope.Links;

                links.UseUnicode();

                auto sequence = UnicodeMap.FromStringToLinkArray(_sequenceExample);

                auto totalSequenceSymbolFrequencyCounter = TotalSequenceSymbolFrequencyCounter<std::uint64_t>(links);

                auto linkFrequenciesCache = LinkFrequenciesCache<std::uint64_t>(links, totalSequenceSymbolFrequencyCounter);

                auto index = CachedFrequencyIncrementingSequenceIndex<std::uint64_t>(linkFrequenciesCache);
                auto linkToItsFrequencyNumberConverter = FrequenciesCacheBasedLinkToItsFrequencyNumberConverter<std::uint64_t>(linkFrequenciesCache);

                auto sequenceToItsLocalElementLevelsConverter = SequenceToItsLocalElementLevelsConverter<std::uint64_t>(links, linkToItsFrequencyNumberConverter);
                auto optimalVariantConverter = OptimalVariantConverter<std::uint64_t>(links, sequenceToItsLocalElementLevelsConverter);

                auto sequences = Sequences(links, SequencesOptions<std::uint64_t>() { Walker = LeveledSequenceWalker<std::uint64_t>(links) });

                ExecuteTest(sequences, sequence, sequenceToItsLocalElementLevelsConverter, index, optimalVariantConverter);
            }
        }

        private: static void ExecuteTest(Sequences sequences, std::uint64_t sequence[], SequenceToItsLocalElementLevelsConverter<std::uint64_t> sequenceToItsLocalElementLevelsConverter, ISequenceIndex<std::uint64_t> &index, OptimalVariantConverter<std::uint64_t> optimalVariantConverter)
        {
            index.Add(sequence);

            auto optimalVariant = optimalVariantConverter.Convert(sequence);

            auto readSequence1 = sequences.ToList(optimalVariant);

            Assert::IsTrue(sequence.SequenceEqual(readSequence1));
        }

        public: TEST_METHOD(SavedSequencesOptimizationTest)
        {
            LinksConstants<std::uint64_t> constants = LinksConstants<std::uint64_t>((1, std::numeric_limits<std::int64_t>::max()), (std::numeric_limits<std::int64_t>::max() + 1UL, std::numeric_limits<std::uint64_t>::max()));

            using (auto memory = HeapResizableDirectMemory())
            using (auto disposableLinks = UInt64UnitedMemoryLinks(memory, UInt64UnitedMemoryLinks.DefaultLinksSizeStep, constants, IndexTreeType.Default))
            {
                auto links = UInt64Links(disposableLinks);

                auto root = links.CreatePoint();

                auto addressToNumberConverter = AddressToRawNumberConverter<std::uint64_t>();

                auto unicodeSymbolMarker = links.GetOrCreate(root, addressToNumberConverter.Convert(1));
                auto unicodeSequenceMarker = links.GetOrCreate(root, addressToNumberConverter.Convert(2));

                auto totalSequenceSymbolFrequencyCounter = TotalSequenceSymbolFrequencyCounter<std::uint64_t>(links);
                auto linkFrequenciesCache = LinkFrequenciesCache<std::uint64_t>(links, totalSequenceSymbolFrequencyCounter);
                auto index = CachedFrequencyIncrementingSequenceIndex<std::uint64_t>(linkFrequenciesCache);
                auto linkToItsFrequencyNumberConverter = FrequenciesCacheBasedLinkToItsFrequencyNumberConverter<std::uint64_t>(linkFrequenciesCache);
                auto sequenceToItsLocalElementLevelsConverter = SequenceToItsLocalElementLevelsConverter<std::uint64_t>(links, linkToItsFrequencyNumberConverter);
                auto optimalVariantConverter = OptimalVariantConverter<std::uint64_t>(links, sequenceToItsLocalElementLevelsConverter);

                auto walker = RightSequenceWalker<std::uint64_t>(links, DefaultStack<std::uint64_t>(), link { return constants.IsExternalReference(link) || links.IsPartialPoint(link)); }

                auto unicodeSequencesOptions = SequencesOptions<std::uint64_t>()
                {
                    UseSequenceMarker = true,
                    SequenceMarkerLink = unicodeSequenceMarker,
                    UseIndex = true,
                    Index = index,
                    LinksToSequenceConverter = optimalVariantConverter,
                    Walker = walker,
                    UseGarbageCollection = true
                };

                auto unicodeSequences = Sequences(SynchronizedLinks<std::uint64_t>(links), unicodeSequencesOptions);

                auto strings = _loremIpsumExample.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                auto arrays = strings.Select(x => x.Select(y => addressToNumberConverter.Convert(y)).ToArray()).ToArray();
                for (std::int32_t i = 0; i < arrays.Length; i++)
                {
                    unicodeSequences.Create(arrays[i].ShiftRight());
                }

                auto linksCountAfterCreation = links.Count()();

                unicodeSequences.CompactAll();

                auto linksCountAfterCompactification = links.Count()();

                Assert::IsTrue(linksCountAfterCompactification < linksCountAfterCreation);
            }
        }
    };
}
