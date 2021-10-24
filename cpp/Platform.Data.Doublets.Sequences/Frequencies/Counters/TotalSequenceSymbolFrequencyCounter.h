namespace Platform::Data::Doublets::Sequences::Frequencies::Counters
{
    template <typename ...> class TotalSequenceSymbolFrequencyCounter;
    template <typename TLink> class TotalSequenceSymbolFrequencyCounter<TLink> : public ICounter<TLink, TLink>
    {
        private: ILinks<TLink> *_links;

        public: TotalSequenceSymbolFrequencyCounter(ILinks<TLink> &links) { _links = links; }

        public: TLink Count(TLink symbol) { return TotalSequenceSymbolFrequencyOneOffCounter<TLink>(_links, symbol).Count(); }
    };
}
