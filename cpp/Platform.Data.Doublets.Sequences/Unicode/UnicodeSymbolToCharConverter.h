namespace Platform::Data::Doublets::Unicode
{
    template <typename ...> class UnicodeSymbolToCharConverter;
    template <typename TLink> class UnicodeSymbolToCharConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<TLink, char>
    {
        private: static readonly UncheckedConverter<TLink, char> _addressToCharConverter = UncheckedConverter<TLink, char>.Default;

        private: IConverter<TLink> *_numberToAddressConverter;
        private: ICriterionMatcher<TLink> *_unicodeSymbolCriterionMatcher;

        public: UnicodeSymbolToCharConverter(ILinks<TLink> &links, IConverter<TLink> &numberToAddressConverter, ICriterionMatcher<TLink> &unicodeSymbolCriterionMatcher) : base(links)
        {
            _numberToAddressConverter = numberToAddressConverter;
            _unicodeSymbolCriterionMatcher = unicodeSymbolCriterionMatcher;
        }

        public: char Convert(TLink source)
        {
            if (!_unicodeSymbolCriterionMatcher.IsMatched(source))
            {
                throw std::invalid_argument("source", source, "Specified link is not a unicode symbol.");
            }
            return _addressToCharConverter.Convert(_numberToAddressConverter.Convert(_links.GetSource(source)));
        }
    };
}
