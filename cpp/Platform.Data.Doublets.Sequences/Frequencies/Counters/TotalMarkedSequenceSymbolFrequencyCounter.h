namespace Platform::Data::Doublets::Sequences::Frequencies::Counters
{
    template <typename ...> class TotalMarkedSequenceSymbolFrequencyCounter;
    template <typename TLink> class TotalMarkedSequenceSymbolFrequencyCounter<TLink> : public ICounter<TLink, TLink>
    {
        private: ILinks<TLink> *_links;
        private: ICriterionMatcher<TLink> *_markedSequenceMatcher;

        public: TotalMarkedSequenceSymbolFrequencyCounter(ILinks<TLink> &links, ICriterionMatcher<TLink> &markedSequenceMatcher)
        {
            _links = links;
            _markedSequenceMatcher = markedSequenceMatcher;
        }

        public: TLink Count(TLink argument) { return TotalMarkedSequenceSymbolFrequencyOneOffCounter<TLink>(_links, _markedSequenceMatcher, argument).Count(); }
    };
}
