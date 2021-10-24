namespace Platform::Data::Doublets::Sequences
{
    template <typename ...> class SequencesOptions;
    template <typename TLink> class SequencesOptions<TLink>
    {
        public: TLink SequenceMarkerLink
        {
            get;
            set;
        }
        
        public: bool UseCascadeUpdate
        {
            get;
            set;
        }
        
        public: bool UseCascadeDelete
        {
            get;
            set;
        }
        
        public: bool UseIndex
        {
            get;
            set;
        }
        
        public: bool UseSequenceMarker
        {
            get;
            set;
        }

        public: bool UseCompression
        {
            get;
            set;
        }

        public: bool UseGarbageCollection
        {
            get;
            set;
        }

        public: bool EnforceSingleSequenceVersionOnWriteBasedOnExisting
        {
            get;
            set;
        }

        public: bool EnforceSingleSequenceVersionOnWriteBasedOnNew
        {
            get;
            set;
        }

        public: MarkedSequenceCriterionMatcher<TLink> MarkedSequenceMatcher
        {
            get;
            set;
        }

        public: IConverter<IList<TLink>, TLink> LinksToSequenceConverter
        {
            get;
            set;
        }

        public: ISequenceIndex<TLink> Index
        {
            get;
            set;
        }

        public: ISequenceWalker<TLink> Walker
        {
            get;
            set;
        }

        public: bool ReadFullSequence
        {
            get;
            set;
        }

        public: void InitOptions(ISynchronizedLinks<TLink> &links)
        {
            if (UseSequenceMarker)
            {
                if (SequenceMarkerLink == links.Constants.Null)
                {
                    SequenceMarkerLink = links.CreatePoint();
                }
                else
                {
                    if (!links.Exists(SequenceMarkerLink))
                    {
                        auto link = links.CreatePoint();
                        if (!link == SequenceMarkerLink)
                        {
                            throw std::runtime_error("Cannot recreate sequence marker link.");
                        }
                    }
                }
                if (MarkedSequenceMatcher == nullptr)
                {
                    MarkedSequenceMatcher = MarkedSequenceCriterionMatcher<TLink>(links, SequenceMarkerLink);
                }
            }
            auto balancedVariantConverter = BalancedVariantConverter<TLink>(links);
            if (UseCompression)
            {
                if (LinksToSequenceConverter == nullptr)
                {
                    ICounter<TLink, TLink> *totalSequenceSymbolFrequencyCounter;
                    if (UseSequenceMarker)
                    {
                        totalSequenceSymbolFrequencyCounter = TotalMarkedSequenceSymbolFrequencyCounter<TLink>(links, MarkedSequenceMatcher);
                    }
                    else
                    {
                        totalSequenceSymbolFrequencyCounter = TotalSequenceSymbolFrequencyCounter<TLink>(links);
                    }
                    auto doubletFrequenciesCache = LinkFrequenciesCache<TLink>(links, totalSequenceSymbolFrequencyCounter);
                    auto compressingConverter = CompressingConverter<TLink>(links, balancedVariantConverter, doubletFrequenciesCache);
                    LinksToSequenceConverter = compressingConverter;
                }
            }
            else
            {
                if (LinksToSequenceConverter == nullptr)
                {
                    LinksToSequenceConverter = balancedVariantConverter;
                }
            }
            if (UseIndex && Index == nullptr)
            {
                Index = SequenceIndex<TLink>(links);
            }
            if (Walker == nullptr)
            {
                Walker = RightSequenceWalker<TLink>(links, DefaultStack<TLink>());
            }
        }

        public: void ValidateOptions()
        {
            if (UseGarbageCollection && !UseSequenceMarker)
            {
                throw NotSupportedException("To use garbage collection UseSequenceMarker option must be on.");
            }
        }
    };
}
