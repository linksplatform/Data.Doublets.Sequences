namespace Platform::Data::Doublets::Incrementers
{
    template <typename ...> class FrequencyIncrementer;
    template <typename TLink> class FrequencyIncrementer<TLink> : public LinksOperatorBase<TLink>, IIncrementer<TLink>
    {
        private: TLink _frequencyMarker = 0;
        private: TLink _unaryOne = 0;
        private: IIncrementer<TLink> *_unaryNumberIncrementer;

        public: FrequencyIncrementer(ILinks<TLink> &links, TLink frequencyMarker, TLink unaryOne, IIncrementer<TLink> &unaryNumberIncrementer)
            : base(links)
        {
            _frequencyMarker = frequencyMarker;
            _unaryOne = unaryOne;
            _unaryNumberIncrementer = unaryNumberIncrementer;
        }

        public: TLink Increment(TLink frequency)
        {
            auto links = _links;
            if (frequency == 0)
            {
                return links.GetOrCreate(_unaryOne, _frequencyMarker);
            }
            auto incrementedSource = _unaryNumberIncrementer.Increment(links.GetSource(frequency));
            return links.GetOrCreate(incrementedSource, _frequencyMarker);
        }
    };
}
