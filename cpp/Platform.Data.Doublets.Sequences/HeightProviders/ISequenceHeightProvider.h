namespace Platform::Data::Doublets::Sequences::HeightProviders
{
    template <typename ...> class ISequenceHeightProvider;
    template <typename TLink> class ISequenceHeightProvider<TLink> : public IProvider<TLink, TLink>
    {
    public:
    };
}
