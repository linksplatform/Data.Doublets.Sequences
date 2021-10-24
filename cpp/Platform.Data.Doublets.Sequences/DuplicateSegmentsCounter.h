namespace Platform::Data::Doublets::Sequences
{
    template <typename ...> class DuplicateSegmentsCounter;
    template <typename TLink> class DuplicateSegmentsCounter<TLink> : public ICounter<std::int32_t>
    {
        private: readonly IProvider<IList<KeyValuePair<IList<TLink>, IList<TLink>>>> _duplicateFragmentsProvider;

        public: DuplicateSegmentsCounter(IProvider<IList<KeyValuePair<IList<TLink>, IList<TLink>>>> duplicateFragmentsProvider) { _duplicateFragmentsProvider = duplicateFragmentsProvider; }

        public: std::int32_t Count() { return _duplicateFragmentsProvider.Get().Sum(x => x.Value.Count()); }
    };
}
