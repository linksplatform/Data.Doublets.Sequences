namespace Platform::Data::Doublets::Sequences
{
    template <typename ...> class DefaultSequenceAppender;
    template <typename TLink> class DefaultSequenceAppender<TLink> : public LinksOperatorBase<TLink>, ISequenceAppender<TLink>
    {
        private: IStack<TLink> *_stack;
        private: ISequenceHeightProvider<TLink> *_heightProvider;

        public: DefaultSequenceAppender(ILinks<TLink> &links, IStack<TLink> &stack, ISequenceHeightProvider<TLink> &heightProvider)
            : base(links)
        {
            _stack = stack;
            _heightProvider = heightProvider;
        }

        public: TLink Append(TLink sequence, TLink appendant)
        {
            auto cursor = sequence;
            auto links = _links;
            while (!_heightProvider.Get(cursor) == 0)
            {
                auto source = links.GetSource(cursor);
                auto target = links.GetTarget(cursor);
                if (_heightProvider.Get(source) == _heightProvider.Get(target))
                {
                    break;
                }
                else
                {
                    _stack.Push(source);
                    cursor = target;
                }
            }
            auto left = cursor;
            auto right = appendant;
            while (!cursor = _stack.PopOrDefault() == links.Constants.Null)
            {
                right = links.GetOrCreate(left, right);
                left = cursor;
            }
            return links.GetOrCreate(left, right);
        }
    };
}
