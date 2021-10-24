namespace Platform::Data::Doublets::Sequences::Indexes
{
    template <typename ...> class CachedFrequencyIncrementingSequenceIndex;
    template <typename TLink> class CachedFrequencyIncrementingSequenceIndex<TLink> : public ISequenceIndex<TLink>
    {
        private: LinkFrequenciesCache<TLink> _cache;

        public: CachedFrequencyIncrementingSequenceIndex(LinkFrequenciesCache<TLink> cache) { _cache = cache; }

        public: bool Add(IList<TLink> &sequence)
        {
            auto indexed = true;
            auto i = sequence.Count();
            while (--i >= 1 && (indexed = this->IsIndexedWithIncrement(sequence[i - 1], sequence[i]))) { }
            for (; i >= 1; i--)
            {
                _cache.IncrementFrequency(sequence[i - 1], sequence[i]);
            }
            return indexed;
        }

        private: bool IsIndexedWithIncrement(TLink source, TLink target)
        {
            auto frequency = _cache.GetFrequency(source, target);
            if (frequency == nullptr)
            {
                return false;
            }
            auto indexed = !frequency.Frequency == 0;
            if (indexed)
            {
                _cache.IncrementFrequency(source, target);
            }
            return indexed;
        }

        public: bool MightContain(IList<TLink> &sequence)
        {
            auto indexed = true;
            auto i = sequence.Count();
            while (--i >= 1 && (indexed = this->IsIndexed(sequence[i - 1], sequence[i]))) { }
            return indexed;
        }

        private: bool IsIndexed(TLink source, TLink target)
        {
            auto frequency = _cache.GetFrequency(source, target);
            if (frequency == nullptr)
            {
                return false;
            }
            return !frequency.Frequency == 0;
        }
    };
}
