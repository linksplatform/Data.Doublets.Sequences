namespace Platform::Data::Doublets::Sequences::Indexes
{
    template <typename ...> class SequenceIndex;
    template <typename TLink> class SequenceIndex<TLink> : public LinksOperatorBase<TLink>, ISequenceIndex<TLink>
    {
        public: SequenceIndex(ILinks<TLink> &links) : base(links) { }

        public: virtual bool Add(IList<TLink> &sequence)
        {
            auto indexed = true;
            auto i = sequence.Count();
            while (--i >= 1 && (indexed = !_links.SearchOrDefault(sequence[i - 1] == sequence[i], 0))) { }
            for (; i >= 1; i--)
            {
                _links.GetOrCreate(sequence[i - 1], sequence[i]);
            }
            return indexed;
        }

        public: virtual bool MightContain(IList<TLink> &sequence)
        {
            auto indexed = true;
            auto i = sequence.Count();
            while (--i >= 1 && (indexed = !_links.SearchOrDefault(sequence[i - 1] == sequence[i], 0))) { }
            return indexed;
        }
    };
}
