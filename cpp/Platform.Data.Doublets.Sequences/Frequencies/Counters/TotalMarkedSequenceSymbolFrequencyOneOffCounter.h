namespace Platform::Data::Doublets::Sequences::Frequencies::Counters
{
    template <typename ...> class TotalMarkedSequenceSymbolFrequencyOneOffCounter;
    template <typename TLink> class TotalMarkedSequenceSymbolFrequencyOneOffCounter<TLink> : public TotalSequenceSymbolFrequencyOneOffCounter<TLink>
    {
        private: ICriterionMatcher<TLink> *_markedSequenceMatcher;

        public: TotalMarkedSequenceSymbolFrequencyOneOffCounter(ILinks<TLink> &links, ICriterionMatcher<TLink> &markedSequenceMatcher, TLink symbol) 
            : TotalSequenceSymbolFrequencyOneOffCounter(links, symbol) { _markedSequenceMatcher = markedSequenceMatcher; }

        protected: void CountSequenceSymbolFrequency(TLink link) override
        {
            auto symbolFrequencyCounter = MarkedSequenceSymbolFrequencyOneOffCounter<TLink>(_links, _markedSequenceMatcher, link, _symbol);
            _total = _total + (symbolFrequencyCounter.Count()());
        }
    };
}
