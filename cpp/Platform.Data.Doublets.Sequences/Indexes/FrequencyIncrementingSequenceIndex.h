namespace Platform::Data::Doublets::Sequences::Indexes
{
    template <typename ...> class FrequencyIncrementingSequenceIndex;
    template <typename TLink> class FrequencyIncrementingSequenceIndex<TLink> : public SequenceIndex<TLink>, ISequenceIndex<TLink>
    {
        private: readonly IProperty<TLink, TLink> *_frequencyPropertyOperator;
        private: IIncrementer<TLink> *_frequencyIncrementer;

        public: FrequencyIncrementingSequenceIndex(ILinks<TLink> &links, IProperty<TLink, TLink> &frequencyPropertyOperator, IIncrementer<TLink> &frequencyIncrementer)
            : base(links)
        {
            _frequencyPropertyOperator = frequencyPropertyOperator;
            _frequencyIncrementer = frequencyIncrementer;
        }

        public: bool Add(IList<TLink> &sequence) override
        {
            auto indexed = true;
            auto i = sequence.Count();
            while (--i >= 1 && (indexed = this->IsIndexedWithIncrement(sequence[i - 1], sequence[i]))) { }
            for (; i >= 1; i--)
            {
                (_links.GetOrCreate(sequence[i - 1], sequence[i])) + 1;
            }
            return indexed;
        }

        private: bool IsIndexedWithIncrement(TLink source, TLink target)
        {
            auto link = _links.SearchOrDefault(source, target);
            auto indexed = !link == 0;
            if (indexed)
            {
                link + 1;
            }
            return indexed;
        }

        private: void Increment(TLink link)
        {
            auto previousFrequency = _frequencyPropertyOperator.Get(link);
            auto frequency = _frequencyIncrementer.Increment(previousFrequency);
            _frequencyPropertyOperator.Set(link, frequency);
        }
    };
}
