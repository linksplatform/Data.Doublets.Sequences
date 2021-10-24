namespace Platform::Data::Doublets::Unicode
{
    template <typename ...> class CharToUnicodeSymbolConverter;
    template <typename TLink> class CharToUnicodeSymbolConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<char, TLink>
    {
        private: static readonly UncheckedConverter<char, TLink> _charToAddressConverter = UncheckedConverter<char, TLink>.Default;

        private: IConverter<TLink> *_addressToNumberConverter;
        private: TLink _unicodeSymbolMarker = 0;

        public: CharToUnicodeSymbolConverter(ILinks<TLink> &links, IConverter<TLink> &addressToNumberConverter, TLink unicodeSymbolMarker) : base(links)
        {
            _addressToNumberConverter = addressToNumberConverter;
            _unicodeSymbolMarker = unicodeSymbolMarker;
        }

        public: TLink Convert(char source)
        {
            auto unaryNumber = _addressToNumberConverter.Convert(_charToAddressConverter.Convert(source));
            return _links.GetOrCreate(unaryNumber, _unicodeSymbolMarker);
        }
    };
}
