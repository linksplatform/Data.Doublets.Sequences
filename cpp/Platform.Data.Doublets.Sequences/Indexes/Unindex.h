namespace Platform::Data::Doublets::Sequences::Indexes
{
    template <typename ...> class Unindex;
    template <typename TLink> class Unindex<TLink> : public ISequenceIndex<TLink>
    {
        public: virtual bool Add(IList<TLink> &sequence) { return false; }

        public: virtual bool MightContain(IList<TLink> &sequence) { return true; }
    };
}
