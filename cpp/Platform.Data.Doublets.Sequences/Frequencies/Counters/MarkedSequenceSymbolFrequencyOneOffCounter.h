namespace Platform::Data::Doublets::Sequences::Frequencies::Counters
{
    template <typename ...> class MarkedSequenceSymbolFrequencyOneOffCounter;
    template <typename TLink> class MarkedSequenceSymbolFrequencyOneOffCounter<TLink> : public SequenceSymbolFrequencyOneOffCounter<TLink>
    {
        private: ICriterionMatcher<TLink> *_markedSequenceMatcher;

        public: MarkedSequenceSymbolFrequencyOneOffCounter(ILinks<TLink> &links, ICriterionMatcher<TLink> &markedSequenceMatcher, TLink sequenceLink, TLink symbol)
            : SequenceSymbolFrequencyOneOffCounter(links, sequenceLink, symbol) { _markedSequenceMatcher = markedSequenceMatcher; }

        public: override TLink Count()
        {
            if (!_markedSequenceMatcher.IsMatched(_sequenceLink))
            {
                return 0;
            }
            return base.Count()();
        }
    };
}
