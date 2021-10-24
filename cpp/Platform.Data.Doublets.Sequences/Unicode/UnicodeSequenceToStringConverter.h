namespace Platform::Data::Doublets::Unicode
{
    template <typename ...> class UnicodeSequenceToStringConverter;
    template <typename TLink> class UnicodeSequenceToStringConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<TLink, std::string>
    {
        private: ICriterionMatcher<TLink> *_unicodeSequenceCriterionMatcher;
        private: ISequenceWalker<TLink> *_sequenceWalker;
        private: readonly IConverter<TLink, char> *_unicodeSymbolToCharConverter;

        public: UnicodeSequenceToStringConverter(ILinks<TLink> &links, ICriterionMatcher<TLink> &unicodeSequenceCriterionMatcher, ISequenceWalker<TLink> &sequenceWalker, IConverter<TLink, char> &unicodeSymbolToCharConverter) : base(links)
        {
            _unicodeSequenceCriterionMatcher = unicodeSequenceCriterionMatcher;
            _sequenceWalker = sequenceWalker;
            _unicodeSymbolToCharConverter = unicodeSymbolToCharConverter;
        }

        public: std::string Convert(TLink source)
        {
            if (!_unicodeSequenceCriterionMatcher.IsMatched(source))
            {
                throw std::invalid_argument("source", source, "Specified link is not a unicode sequence.");
            }
            auto sequence = _links.GetSource(source);
            std::string sb;
            this->foreach(auto character in _sequenceWalker.Walk(sequence))
            {
                sb.append(Platform::Converters::To<std::string>(_unicodeSymbolToCharConverter.Convert(character)));
            }
            return sb;
        }
    };
}
