namespace Platform::Data::Doublets::Unicode
{
    template <typename ...> class StringToUnicodeSequenceConverter;
    template <typename TLink> class StringToUnicodeSequenceConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<std::string, TLink>
    {
        private: readonly IConverter<std::string, IList<TLink>> _stringToUnicodeSymbolListConverter;
        private: readonly IConverter<IList<TLink>, TLink> _unicodeSymbolListToSequenceConverter;

        public: StringToUnicodeSequenceConverter(ILinks<TLink> &links, IConverter<std::string, IList<TLink>> stringToUnicodeSymbolListConverter, IConverter<IList<TLink>, TLink> unicodeSymbolListToSequenceConverter) : base(links)
        {
            _stringToUnicodeSymbolListConverter = stringToUnicodeSymbolListConverter;
            _unicodeSymbolListToSequenceConverter = unicodeSymbolListToSequenceConverter;
        }

        public: StringToUnicodeSequenceConverter(ILinks<TLink> &links, IConverter<std::string, IList<TLink>> stringToUnicodeSymbolListConverter, ISequenceIndex<TLink> &index, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker)
            : this(links, stringToUnicodeSymbolListConverter, UnicodeSymbolsListToUnicodeSequenceConverter<TLink>(links, index, listToSequenceLinkConverter, unicodeSequenceMarker)) { }

        public: StringToUnicodeSequenceConverter(ILinks<TLink> &links, IConverter<char, TLink> &charToUnicodeSymbolConverter, ISequenceIndex<TLink> &index, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker)
            : this(links, StringToUnicodeSymbolsListConverter<TLink>(charToUnicodeSymbolConverter), index, listToSequenceLinkConverter, unicodeSequenceMarker) { }

        public: StringToUnicodeSequenceConverter(ILinks<TLink> &links, IConverter<char, TLink> &charToUnicodeSymbolConverter, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker)
            : this(links, charToUnicodeSymbolConverter, Unindex<TLink>(), listToSequenceLinkConverter, unicodeSequenceMarker) { }

        public: StringToUnicodeSequenceConverter(ILinks<TLink> &links, IConverter<std::string, IList<TLink>> stringToUnicodeSymbolListConverter, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker)
            : this(links, stringToUnicodeSymbolListConverter, Unindex<TLink>(), listToSequenceLinkConverter, unicodeSequenceMarker) { }

        public: TLink Convert(std::string source)
        {
            auto elements = _stringToUnicodeSymbolListConverter.Convert(source);
            return _unicodeSymbolListToSequenceConverter.Convert(elements);
        }
    };
}
