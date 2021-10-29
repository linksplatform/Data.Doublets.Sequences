namespace Platform::Data::Doublets::Sequences::Frequencies::Counters
{
    template <typename ...> class TotalSequenceSymbolFrequencyOneOffCounter;
    template <typename TLink> class TotalSequenceSymbolFrequencyOneOffCounter<TLink> : public ICounter<TLink>
    {
        protected: ILinks<TLink> *_links;
        protected: TLink _symbol = 0;
        protected: HashSet<TLink> _visits;
        protected: TLink _total = 0;

        public: TotalSequenceSymbolFrequencyOneOffCounter(ILinks<TLink> &links, TLink symbol)
        {
            _links = links;
            _symbol = symbol;
            _visits = HashSet<TLink>();
            _total = 0;
        }

        public: TLink Count()
        {
            if (_total > 0 || _visits.Count() > 0)
            {
                return _total;
            }
            this->CountCore(_symbol);
            return _total;
        }

        private: void CountCore(TLink link)
        {
            auto any = _links.Constants.Any;
            if (_links.Count()(any == link, 0))
            {
                this->CountSequenceSymbolFrequency(link);
            }
            else
            {
                _links.Each(EachElementHandler, any, link);
            }
        }

        protected: virtual void CountSequenceSymbolFrequency(TLink link)
        {
            auto symbolFrequencyCounter = SequenceSymbolFrequencyOneOffCounter<TLink>(_links, link, _symbol);
            _total = _total + (symbolFrequencyCounter.Count()());
        }

        private: TLink EachElementHandler(IList<TLink> &doublet)
        {
            auto constants = _links.Constants;
            auto doubletIndex = doublet[constants.IndexPart];
            if (_visits.Add(doubletIndex))
            {
                this->CountCore(doubletIndex);
            }
            return constants.Continue;
        }
    };
}
