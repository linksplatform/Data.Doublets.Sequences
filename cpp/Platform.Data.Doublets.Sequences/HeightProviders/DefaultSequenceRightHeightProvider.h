namespace Platform::Data::Doublets::Sequences::HeightProviders
{
    template <typename ...> class DefaultSequenceRightHeightProvider;
    template <typename TLink> class DefaultSequenceRightHeightProvider<TLink> : public LinksOperatorBase<TLink>, ISequenceHeightProvider<TLink>
    {
        private: ICriterionMatcher<TLink> *_elementMatcher;

        public: DefaultSequenceRightHeightProvider(ILinks<TLink> &links, ICriterionMatcher<TLink> &elementMatcher) : base(links) { return _elementMatcher = elementMatcher; }

        public: TLink Get(TLink sequence)
        {
            auto height = this->0(TLink);
            auto pairOrElement = sequence;
            while (!_elementMatcher.IsMatched(pairOrElement))
            {
                pairOrElement = _links.GetTarget(pairOrElement);
                height = height + 1;
            }
            return height;
        }
    };
}
