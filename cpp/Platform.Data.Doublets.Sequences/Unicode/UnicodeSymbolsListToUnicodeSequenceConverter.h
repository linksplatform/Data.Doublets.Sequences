namespace Platform::Data::Doublets::Unicode
{
    template <typename ...> class UnicodeSymbolsListToUnicodeSequenceConverter;
    template <typename TLink> class UnicodeSymbolsListToUnicodeSequenceConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<IList<TLink>, TLink>
    {
        private: ISequenceIndex<TLink> *_index;
        private: readonly IConverter<IList<TLink>, TLink> _listToSequenceLinkConverter;
        private: TLink _unicodeSequenceMarker = 0;

        public: UnicodeSymbolsListToUnicodeSequenceConverter(ILinks<TLink> &links, ISequenceIndex<TLink> &index, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker) : base(links)
        {
            _index = index;
            _listToSequenceLinkConverter = listToSequenceLinkConverter;
            _unicodeSequenceMarker = unicodeSequenceMarker;
        }

        public: UnicodeSymbolsListToUnicodeSequenceConverter(ILinks<TLink> &links, IConverter<IList<TLink>, TLink> listToSequenceLinkConverter, TLink unicodeSequenceMarker)
            : this(links, Unindex<TLink>(), listToSequenceLinkConverter, unicodeSequenceMarker) { }

        public: TLink Convert(IList<TLink> &list)
        {
            _index.Add(list);
            auto sequence = _listToSequenceLinkConverter.Convert(list);
            return _links.GetOrCreate(sequence, _unicodeSequenceMarker);
        }
    };
}
