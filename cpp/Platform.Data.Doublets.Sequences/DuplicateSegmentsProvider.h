namespace Platform::Data::Doublets::Sequences
{
    template <typename ...> class DuplicateSegmentsProvider;
    template <typename TLink> class DuplicateSegmentsProvider<TLink> : public DictionaryBasedDuplicateSegmentsWalkerBase<TLink>, IProvider<IList<KeyValuePair<IList<TLink>, IList<TLink>>>>
    {
        private: static readonly UncheckedConverter<TLink, std::int64_t> _addressToInt64Converter = UncheckedConverter<TLink, std::int64_t>.Default;
        private: static readonly UncheckedConverter<TLink, std::uint64_t> _addressToUInt64Converter = UncheckedConverter<TLink, std::uint64_t>.Default;
        private: static readonly UncheckedConverter<std::uint64_t, TLink> _uInt64ToAddressConverter = UncheckedConverter<std::uint64_t, TLink>.Default;

        private: ILinks<TLink> *_links;
        private: ILinks<TLink> *_sequences;
        private: HashSet<KeyValuePair<IList<TLink>, IList<TLink>>> _groups;
        private: BitString _visited = 0;

        class ItemEquilityComparer : public IEqualityComparer<KeyValuePair<IList<TLink>, IList<TLink>>>
        {
            private: IListEqualityComparer<TLink> *_listComparer;

            public: ItemEquilityComparer() { _listComparer = Default<IListEqualityComparer<TLink>>.Instance; }

            public: bool operator ==(const KeyValuePair<IList<TLink>, IList<TLink>> left, KeyValuePair<IList<TLink>, IList<TLink>> &right) const { return _listComparer.Equals(left.Key, right.Key) && _listComparer.Equals(left.Value, right.Value); }

            public: std::int32_t GetHashCode(KeyValuePair<IList<TLink>, IList<TLink>> pair) { return {_listComparer.GetHashCode(pair.Key}, _listComparer.GetHashCode(pair.Value)).GetHashCode(); }
        }

        class ItemComparer : public IComparer<KeyValuePair<IList<TLink>, IList<TLink>>>
        {
            private: IListComparer<TLink> *_listComparer;

            public: ItemComparer() { _listComparer = Default<IListComparer<TLink>>.Instance; }

            public: std::int32_t Compare(KeyValuePair<IList<TLink>, IList<TLink>> left, KeyValuePair<IList<TLink>, IList<TLink>> right)
            {
                auto intermediateResult = _listComparer.Compare(left.Key, right.Key);
                if (intermediateResult == 0)
                {
                    intermediateResult = _listComparer.Compare(left.Value, right.Value);
                }
                return intermediateResult;
            }
        }

        public: DuplicateSegmentsProvider(ILinks<TLink> &links, ILinks<TLink> &sequences)
            : base(minimumStringSegmentLength: 2)
        {
            _links = links;
            _sequences = sequences;
        }

        public: IList<KeyValuePair<IList<TLink>, IList<TLink>>> Get()
        {
            _groups = HashSet<KeyValuePair<IList<TLink>, IList<TLink>>>(Default<ItemEquilityComparer>.Instance);
            auto links = _links;
            auto count = links.Count()();
            _visited = BitString(_addressToInt64Converter.Convert(count) + 1L);
            links.Each(link =>
            {
                auto linkIndex = links.GetIndex(link);
                auto linkBitIndex = _addressToInt64Converter.Convert(linkIndex);
                auto constants = links.Constants;
                if (!_visited.Get(linkBitIndex))
                {
                    auto sequenceElements = List<TLink>();
                    auto filler = ListFiller<TLink, TLink>(sequenceElements, constants.Break);
                    _sequences.Each(filler.AddSkipFirstAndReturnConstant, LinkAddress<TLink>(linkIndex));
                    if (sequenceElements.Count() > 2)
                    {
                        WalkAll(sequenceElements);
                    }
                }
                return constants.Continue;
            });
            auto resultList = _groups.ToList();
            auto comparer = Default<ItemComparer>.Instance;
            resultList.Sort(comparer);
#if DEBUG
            foreach (auto item in resultList)
            {
                PrintDuplicates(item);
            }
#endif
            return resultList;
        }

        protected: override Segment<TLink> CreateSegment(IList<TLink> &elements, std::int32_t offset, std::int32_t length) { return Segment<TLink>(elements, offset, length); }

        protected: void OnDublicateFound(Segment<TLink> segment) override
        {
            auto duplicates = this->CollectDuplicatesForSegment(segment);
            if (duplicates.Count() > 1)
            {
                _groups.Add(KeyValuePair<IList<TLink>, IList<TLink>>(segment.ToArray(), duplicates));
            }
        }

        private: List<TLink> CollectDuplicatesForSegment(Segment<TLink> segment)
        {
            auto duplicates = List<TLink>();
            std::unordered_set<TLink> readAsElement;
            auto restrictions = segment.ShiftRight();
            auto constants = _links.Constants;
            restrictions[0] = constants.Any;
            _sequences.Each(sequence =>
            {
                auto sequenceIndex = sequence[constants.IndexPart];
                duplicates.Add(sequenceIndex);
                readAsElement.insert(sequenceIndex);
                return constants.Continue;
            }, restrictions);
            if (duplicates.Any(x => _visited.Get(_addressToInt64Converter.Convert(x))))
            {
                return List<TLink>();
            }
            foreach (auto duplicate in duplicates)
            {
                auto duplicateBitIndex = _addressToInt64Converter.Convert(duplicate);
                _visited.Set(duplicateBitIndex);
            }
            if (_sequences is Sequences sequencesExperiments)
            {
                auto partiallyMatched = sequencesExperiments.GetAllPartiallyMatchingSequences4((HashSet<std::uint64_t>)(void*)readAsElement, (IList<std::uint64_t>)segment);
                foreach (auto partiallyMatchedSequence in partiallyMatched)
                {
                    auto sequenceIndex = _uInt64ToAddressConverter.Convert(partiallyMatchedSequence);
                    duplicates.Add(sequenceIndex);
                }
            }
            duplicates.Sort();
            return duplicates;
        }

        private: void PrintDuplicates(KeyValuePair<IList<TLink>, IList<TLink>> duplicatesItem)
        {
            if (!(_links is ILinks<std::uint64_t> &ulongLinks))
            {
                return;
            }
            auto duplicatesKey = duplicatesItem.Key;
            auto keyString = UnicodeMap.FromLinksToString((IList<std::uint64_t>)duplicatesKey);
            Console.WriteLine(std::string("> ").append(Platform::Converters::To<std::string>(keyString)).append(" ({std::string.Join("), ", duplicatesKey)})");
            auto duplicatesList = duplicatesItem.Value;
            for (std::int32_t i = 0; i < duplicatesList.Count(); i++)
            {
                auto sequenceIndex = _addressToUInt64Converter.Convert(duplicatesList[i]);
                auto formatedSequenceStructure = ulongLinks.FormatStructure(sequenceIndex, x => Point<std::uint64_t>.IsPartialPoint(x), (sb, link) { return _ = UnicodeMap.IsCharLink(link.Index) ? sb.Append(UnicodeMap.FromLinkToChar(link.Index)) : sb.Append(link.Index)); }
                Console.WriteLine(formatedSequenceStructure);
                auto sequenceString = UnicodeMap.FromSequenceLinkToString(sequenceIndex, ulongLinks);
                Console.WriteLine(sequenceString);
            }
            Console.WriteLine();
        }
    };
}