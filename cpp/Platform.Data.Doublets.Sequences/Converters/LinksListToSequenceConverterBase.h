namespace Platform::Data::Doublets::Sequences::Converters
{
    template <typename ...> class LinksListToSequenceConverterBase;
    template <typename TLink> class LinksListToSequenceConverterBase<TLink> : public LinksOperatorBase<TLink>, IConverter<IList<TLink>, TLink>
    {
        protected: LinksListToSequenceConverterBase(ILinks<TLink> &links) : base(links) { }

        public: virtual TLink Convert(IList<TLink> &source) = 0;
    };
}
