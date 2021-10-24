namespace Platform::Data::Doublets::Sequences::Indexes
{
    template <typename ...> class ISequenceIndex;
    template <typename TLink> class ISequenceIndex<TLink>
    {
    public:
        virtual bool Add(IList<TLink> &sequence) = 0;

        virtual bool MightContain(IList<TLink> &sequence) = 0;
    };
}
