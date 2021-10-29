namespace Platform::Data::Doublets::Sequences::Frequencies::Counters
{
    template <typename ...> class SequenceSymbolFrequencyOneOffCounter;
    template <typename TLink> class SequenceSymbolFrequencyOneOffCounter<TLink> : public ICounter<TLink>
    {
        protected: ILinks<TLink> *_links;
        protected: TLink _sequenceLink = 0;
        protected: TLink _symbol = 0;
        protected: TLink _total = 0;

        public: SequenceSymbolFrequencyOneOffCounter(ILinks<TLink> &links, TLink sequenceLink, TLink symbol)
        {
            _links = links;
            _sequenceLink = sequenceLink;
            _symbol = symbol;
            _total = 0;
        }

        public: virtual TLink Count()
        {
            if (_total > 0)
            {
                return _total;
            }
            StopableSequenceWalker.WalkRight(_sequenceLink, _links.GetSource, _links.GetTarget, IsElement, VisitElement);
            return _total;
        }

        private: bool IsElement(TLink x) { return x == _symbol || _links.IsPartialPoint(x); }

        private: bool VisitElement(TLink element)
        {
            if (element == _symbol)
            {
                _total = _total + 1;
            }
            return true;
        }
    };
}
