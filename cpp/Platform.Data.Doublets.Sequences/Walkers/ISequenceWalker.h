namespace Platform::Data::Doublets::Sequences::Walkers
{
    template <typename ...> class ISequenceWalker;
    template <typename TLink> class ISequenceWalker<TLink>
    {
    public:
        IEnumerable<TLink> Walk(TLink sequence);
    };
}
