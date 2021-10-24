namespace Platform::Data::Doublets::Sequences::CriterionMatchers
{
    template <typename ...> class MarkedSequenceCriterionMatcher;
    template <typename TLink> class MarkedSequenceCriterionMatcher<TLink> : public ICriterionMatcher<TLink>
    {
        private: ILinks<TLink> *_links;
        private: TLink _sequenceMarkerLink = 0;

        public: MarkedSequenceCriterionMatcher(ILinks<TLink> &links, TLink sequenceMarkerLink)
        {
            _links = links;
            _sequenceMarkerLink = sequenceMarkerLink;
        }

        public: bool IsMatched(TLink sequenceCandidate)
            => _links.GetSource(sequenceCandidate) == _sequenceMarkerLink
            || !_links.SearchOrDefault(_sequenceMarkerLink == sequenceCandidate, _links.Constants.Null);
    };
}
