namespace Platform::Data::Doublets::Sequences::Walkers
{
    template <typename ...> class LeftSequenceWalker;
    template <typename TLink> class LeftSequenceWalker<TLink> : public SequenceWalkerBase<TLink>
    {
        public: LeftSequenceWalker(ILinks<TLink> &links, IStack<TLink> &stack, Func<TLink, bool> isElement) : SequenceWalkerBase(links, stack, isElement) { }

        public: LeftSequenceWalker(ILinks<TLink> &links, IStack<TLink> &stack) : SequenceWalkerBase(links, stack, links.IsPartialPoint) { }

        protected: TLink GetNextElementAfterPop(TLink element) override { return _links.GetSource(element); }

        protected: TLink GetNextElementAfterPush(TLink element) override { return _links.GetTarget(element); }

        protected: override IEnumerable<TLink> WalkContents(TLink element)
        {
            auto links = _links;
            auto parts = links.GetLink(element);
            auto start = links.Constants.SourcePart;
            for (auto i = parts.Count() - 1; i >= start; i--)
            {
                auto part = parts[i];
                if (IsElement(part))
                {
                    yield return part;
                }
            }
        }
    };
}
