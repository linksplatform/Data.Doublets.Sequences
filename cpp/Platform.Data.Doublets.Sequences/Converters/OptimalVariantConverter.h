namespace Platform::Data::Doublets::Sequences::Converters
{
    template <typename ...> class OptimalVariantConverter;
    template <typename TLink> class OptimalVariantConverter<TLink> : public LinksListToSequenceConverterBase<TLink>
    {
        private: IConverter<IList<TLink>> _sequenceToItsLocalElementLevelsConverter;

        public: OptimalVariantConverter(ILinks<TLink> &links, IConverter<IList<TLink>> sequenceToItsLocalElementLevelsConverter) : LinksListToSequenceConverterBase(links) { return _sequenceToItsLocalElementLevelsConverter = sequenceToItsLocalElementLevelsConverter; }

        public: OptimalVariantConverter(ILinks<TLink> &links, LinkFrequenciesCache<TLink> linkFrequenciesCache) 
            : this(links, SequenceToItsLocalElementLevelsConverter<TLink>(links, FrequenciesCacheBasedLinkToItsFrequencyNumberConverter<TLink>(linkFrequenciesCache))) { }

        public: OptimalVariantConverter(ILinks<TLink> &links)
            : this(links, LinkFrequenciesCache<TLink>(links, TotalSequenceSymbolFrequencyCounter<TLink>(links))) { }

        public: TLink Convert(IList<TLink> &sequence) override
        {
            auto length = sequence.Count();
            if (length == 1)
            {
                return sequence[0] = { {0} };
            }
            if (length == 2)
            {
                return _links.GetOrCreate(sequence[0], sequence[1]);
            }
            sequence = sequence.ToArray();
            auto levels = _sequenceToItsLocalElementLevelsConverter.Convert(sequence);
            while (length > 2)
            {
                auto levelRepeat = 1;
                auto currentLevel = levels[0];
                auto previousLevel = levels[0];
                auto skipOnce = false;
                auto w = 0;
                for (auto i = 1; i < length; i++)
                {
                    if (currentLevel == levels[i])
                    {
                        levelRepeat++;
                        skipOnce = false;
                        if (levelRepeat == 2)
                        {
                            sequence[w] = _links.GetOrCreate(sequence[i - 1], sequence[i]);
                            auto newLevel = i >= length - 1 ?
                                this->GetPreviousLowerThanCurrentOrCurrent(previousLevel, currentLevel) :
                                i < 2 ?
                                this->GetNextLowerThanCurrentOrCurrent(currentLevel, levels[i + 1]) :
                                this->GetGreatestNeigbourLowerThanCurrentOrCurrent(previousLevel, currentLevel, levels[i + 1]);
                            levels[w] = newLevel;
                            previousLevel = currentLevel;
                            w++;
                            levelRepeat = 0;
                            skipOnce = true;
                        }
                        else if (i == length - 1)
                        {
                            sequence[w] = sequence[i];
                            levels[w] = levels[i];
                            w++;
                        }
                    }
                    else
                    {
                        currentLevel = levels[i];
                        levelRepeat = 1;
                        if (skipOnce)
                        {
                            skipOnce = false;
                        }
                        else
                        {
                            sequence[w] = sequence[i - 1];
                            levels[w] = levels[i - 1];
                            previousLevel = levels[w];
                            w++;
                        }
                        if (i == length - 1)
                        {
                            sequence[w] = sequence[i];
                            levels[w] = levels[i];
                            w++;
                        }
                    }
                }
                length = w;
            }
            return _links.GetOrCreate(sequence[0], sequence[1]);
        }

        private: static TLink GetGreatestNeigbourLowerThanCurrentOrCurrent(TLink previous, TLink current, TLink next)
        {
            return previous > next
                ? previous < current ? previous : current
                : next < current ? next : current;
        }

        private: static TLink GetNextLowerThanCurrentOrCurrent(TLink current, TLink next) { return next < current ? next : current; }

        private: static TLink GetPreviousLowerThanCurrentOrCurrent(TLink previous, TLink current) { return previous < current ? previous : current; }
    };
}
