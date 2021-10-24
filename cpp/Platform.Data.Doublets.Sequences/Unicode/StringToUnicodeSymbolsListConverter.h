namespace Platform::Data::Doublets::Unicode
{
    template <typename ...> class StringToUnicodeSymbolsListConverter;
    template <typename TLink> class StringToUnicodeSymbolsListConverter<TLink> : public IConverter<std::string, IList<TLink>>
    {
        private: readonly IConverter<char, TLink> *_charToUnicodeSymbolConverter;

        public: StringToUnicodeSymbolsListConverter(IConverter<char, TLink> &charToUnicodeSymbolConverter) { _charToUnicodeSymbolConverter = charToUnicodeSymbolConverter; }

        public: IList<TLink> Convert(std::string source)
        {
            auto elements = TLink[source.Length];
            for (auto i = 0; i < elements.Length; i++)
            {
                elements[i] = _charToUnicodeSymbolConverter.Convert(source[i]);
            }
            return elements;
        }
    };
}
