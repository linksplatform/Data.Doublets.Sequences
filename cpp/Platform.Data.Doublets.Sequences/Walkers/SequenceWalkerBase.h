namespace Platform::Data::Doublets::Sequences::Walkers
{
    template <typename ...> class SequenceWalkerBase;
    template <typename TLink> class SequenceWalkerBase<TLink> : public LinksOperatorBase<TLink>, ISequenceWalker<TLink>
    {
        private: IStack<TLink> *_stack;
        private: readonly Func<TLink, bool> _isElement;

        protected: SequenceWalkerBase(ILinks<TLink> &links, IStack<TLink> &stack, Func<TLink, bool> isElement) : base(links)
        {
            _stack = stack;
            _isElement = isElement;
        }

        protected: SequenceWalkerBase(ILinks<TLink> &links, IStack<TLink> &stack) : this(links, stack, links.IsPartialPoint) { }

        public: IEnumerable<TLink> Walk(TLink sequence)
        {
            _stack.Clear();
            auto element = sequence;
            if (IsElement(element))
            {
                yield return element;
            }
            else
            {
                while (true)
                {
                    if (IsElement(element))
                    {
                        if (_stack.IsEmpty)
                        {
                            break;
                        }
                        element = _stack.Pop();
                        foreach (auto output in WalkContents(element))
                        {
                            yield return output;
                        }
                        element = GetNextElementAfterPop(element);
                    }
                    else
                    {
                        _stack.Push(element);
                        element = GetNextElementAfterPush(element);
                    }
                }
            }
        }

        protected: virtual bool IsElement(TLink elementLink) { return _isElement(elementLink); }

        protected: virtual TLink GetNextElementAfterPop(TLink element) = 0;

        protected: virtual TLink GetNextElementAfterPush(TLink element) = 0;

        protected: virtual IEnumerable<TLink> WalkContents(TLink element) = 0;
    };
}
