namespace Platform::Data::Doublets::Sequences::Frequencies::Cache
{
    template <typename ...> class FrequenciesCacheBasedLinkToItsFrequencyNumberConverter;
    template <typename TLink> class FrequenciesCacheBasedLinkToItsFrequencyNumberConverter<TLink> : public IConverter<Doublet<TLink>, TLink>
    {
        private: LinkFrequenciesCache<TLink> _cache;

        public: FrequenciesCacheBasedLinkToItsFrequencyNumberConverter(LinkFrequenciesCache<TLink> cache) { _cache = cache; }

        public: TLink Convert(Doublet<TLink> source) { return _cache.GetFrequency(source)->Frequency; }
    };
}
