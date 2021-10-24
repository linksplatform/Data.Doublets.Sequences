namespace Platform::Data::Doublets::Sequences::Indexes
{
    template <typename ...> class SynchronizedSequenceIndex;
    template <typename TLink> class SynchronizedSequenceIndex<TLink> : public ISequenceIndex<TLink>
    {
        private: ISynchronizedLinks<TLink> *_links;

        public: SynchronizedSequenceIndex(ISynchronizedLinks<TLink> &links) { _links = links; }

        public: bool Add(IList<TLink> &sequence)
        {
            auto indexed = true;
            auto i = sequence.Count();
            auto links = _links.Unsync;
            _links.SyncRoot.ExecuteReadOperation(() =>
            {
                while (--i >= 1 && (indexed = !links.SearchOrDefault(sequence[i - 1] == sequence[i], 0))) { }
            });
            if (!indexed)
            {
                _links.SyncRoot.ExecuteWriteOperation(() =>
                {
                    for (; i >= 1; i--)
                    {
                        links.GetOrCreate(sequence[i - 1], sequence[i]);
                    }
                });
            }
            return indexed;
        }

        public: bool MightContain(IList<TLink> &sequence)
        {
            auto links = _links.Unsync;
            return _links.SyncRoot.ExecuteReadOperation(() =>
            {
                auto indexed = true;
                auto i = sequence.Count();
                while (--i >= 1 && (indexed = !links.SearchOrDefault(sequence[i - 1] == sequence[i], 0))) { }
                return indexed;
            });
        }
    };
}
