

using TLink = std::uint64_t;

namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(DefaultSequenceAppenderTests)
    {
        private: ITestOutputHelper _output = 0;

        public: DefaultSequenceAppenderTests(ITestOutputHelper &output)
        {
            _output = output;
        }
        public: static ILinks<TLink> CreateLinks() { return CreateLinks<TLink>(IO.TemporaryFile(); });

        public: static ILinks<TLink> CreateLinks<TLink>(std::string dataDBFilename)
        {
            auto linksConstants = LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return UnitedMemoryLinks<TLink>(FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        
        template <typename ...> class ValueCriterionMatcher;
        template <typename TLink> class ValueCriterionMatcher<TLink> : public ICriterionMatcher<TLink>
        {
            public: ILinks<TLink> *Links;
            public: TLink Marker = 0;
            public: ValueCriterionMatcher(ILinks<TLink> &links, TLink marker)
            {
                Links = links;
                Marker = marker;
            }

            public: bool IsMatched(TLink link) { return (Links.GetSource(link)) == Marker; }
        }

        public: TEST_METHOD(AppendArrayBug)
        {
            ILinks<TLink> links = CreateLinks();
            TLink zero = 0;
            auto markerIndex = zero + 1;
            auto meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            auto sequence = links.Create();
            sequence = links.Update(sequence, meaningRoot, sequence);
            auto appendant = links.Create();
            appendant = links.Update(appendant, meaningRoot, appendant);
            ValueCriterionMatcher<TLink> valueCriterionMatcher = new(links, meaningRoot);
            DefaultSequenceRightHeightProvider<std::uint64_t> defaultSequenceRightHeightProvider = new(links, valueCriterionMatcher);
            DefaultSequenceAppender<TLink> defaultSequenceAppender = new(links, DefaultStack<std::uint64_t>(), defaultSequenceRightHeightProvider);
            auto newArray = defaultSequenceAppender.Append(sequence, appendant);
            auto output = links.FormatStructure(newArray, link => link.IsFullPoint(), true);
            Assert::AreEqual("(4:(2:1 2) (3:1 3))", output);
        }
    };
}
