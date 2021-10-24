namespace Platform::Data::Doublets::Sequences::CriterionMatchers
{
    template <typename ...> class DefaultSequenceElementCriterionMatcher;
    template <typename TLink> class DefaultSequenceElementCriterionMatcher<TLink> : public LinksOperatorBase<TLink>, ICriterionMatcher<TLink>
    {
        public: DefaultSequenceElementCriterionMatcher(ILinks<TLink> &links) : base(links) { }

        public: bool IsMatched(TLink argument) { return _links.IsPartialPoint(argument); }
    };
}
