namespace Platform::Data::Doublets::Sequences::Walkers
{
    template <typename ...> class RightSequenceWalker;
    template <typename TLink> class RightSequenceWalker<TLink> : public SequenceWalkerBase<TLink>
    {
        public: RightSequenceWalker(ILinks<TLink> &links, IStack<TLink> &stack, Func<TLink, bool> isElement) : SequenceWalkerBase(links, stack, isElement) { }

        public: RightSequenceWalker(ILinks<TLink> &links, IStack<TLink> &stack) : SequenceWalkerBase(links, stack, links.IsPartialPoint) { }

        protected: TLink GetNextElementAfterPop(TLink element) override { return _links.GetTarget(element); }

        protected: TLink GetNextElementAfterPush(TLink element) override { return _links.GetSource(element); }

        protected: override IEnumerable<TLink> WalkContents(TLink element)
        {
            auto parts = _links.GetLink(element);
            for (auto i = _links.Constants.SourcePart; i < parts.Count(); i++)
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
