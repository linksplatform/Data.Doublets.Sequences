namespace Platform::Data::Doublets::Sequences::Frequencies::Cache
{
    template <typename ...> class LinkFrequenciesCache;
    template <typename TLink> class LinkFrequenciesCache<TLink> : public LinksOperatorBase<TLink>
    {
        private: inline static const TLink _zero = 0;
        private: inline static const TLink _one = _zero + 1;

        private: readonly Dictionary<Doublet<TLink>, LinkFrequency<TLink>> _doubletsCache;
        private: readonly ICounter<TLink, TLink> *_frequencyCounter;

        public: LinkFrequenciesCache(ILinks<TLink> &links, ICounter<TLink, TLink> &frequencyCounter)
            : LinksOperatorBase(links)
        {
            _doubletsCache = Dictionary<Doublet<TLink>, LinkFrequency<TLink>>(4096, DoubletComparer<TLink>.Default);
            _frequencyCounter = frequencyCounter;
        }

        public: LinkFrequency<TLink> GetFrequency(TLink source, TLink target)
        {
            auto doublet = Doublet<TLink>(source, target);
            return GetFrequency(doublet);
        }

        public: LinkFrequency<TLink> GetFrequency(ref Doublet<TLink> doublet)
        {
            _doubletsCache.TryGetValue(doublet, out LinkFrequency<TLink> data);
            return data;
        }

        public: void IncrementFrequencies(IList<TLink> &sequence)
        {
            for (auto i = 1; i < sequence.Count(); i++)
            {
                this->IncrementFrequency(sequence[i - 1], sequence[i]);
            }
        }

        public: LinkFrequency<TLink> IncrementFrequency(TLink source, TLink target)
        {
            auto doublet = Doublet<TLink>(source, target);
            return IncrementFrequency(doublet);
        }

        public: void PrintFrequencies(IList<TLink> &sequence)
        {
            for (auto i = 1; i < sequence.Count(); i++)
            {
                this->PrintFrequency(sequence[i - 1], sequence[i]);
            }
        }

        public: void PrintFrequency(TLink source, TLink target)
        {
            auto number = this->GetFrequency(source, target).Frequency;
            Console.WriteLine("({0},{1}) - {2}", source, target, number);
        }

        public: LinkFrequency<TLink> IncrementFrequency(ref Doublet<TLink> doublet)
        {
            if (_doubletsCache.TryGetValue(doublet, out LinkFrequency<TLink> data))
            {
                data.IncrementFrequency();
            }
            else
            {
                auto link = _links.SearchOrDefault(doublet.Source, doublet.Target);
                data = LinkFrequency<TLink>(_one, link);
                if (!link == 0)
                {
                    data.Frequency = (data.Frequency) + (_frequencyCounter.Count()(link));
                }
                _doubletsCache.Add(doublet, data);
            }
            return data;
        }

        public: void ValidateFrequencies()
        {
            foreach (auto entry in _doubletsCache)
            {
                auto value = entry.Value;
                auto linkIndex = value.Link;
                if (!linkIndex == 0)
                {
                    auto frequency = value.Frequency;
                    auto count = _frequencyCounter.Count()(linkIndex);
                    if (((frequency > count) && (_comparer.Compare(Arithmetic.Subtract(frequency, count), _one) > 0))
                     || ((count > frequency) && (_comparer.Compare(Arithmetic.Subtract(count, frequency), _one) > 0)))
                    {
                        throw std::runtime_error("Frequencies validation failed.");
                    }
                }
            }
        }
    };
}
