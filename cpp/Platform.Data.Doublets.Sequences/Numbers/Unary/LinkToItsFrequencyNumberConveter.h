namespace Platform::Data::Doublets::Numbers::Unary
{
    template <typename ...> class LinkToItsFrequencyNumberConveter;
    template <typename TLink> class LinkToItsFrequencyNumberConveter<TLink> : public LinksOperatorBase<TLink>, IConverter<Doublet<TLink>, TLink>
    {
        private: readonly IProperty<TLink, TLink> *_frequencyPropertyOperator;
        private: IConverter<TLink> *_unaryNumberToAddressConverter;

        public: LinkToItsFrequencyNumberConveter(
            ILinks<TLink> &links,
            IProperty<TLink, TLink> &frequencyPropertyOperator,
            IConverter<TLink> &unaryNumberToAddressConverter)
            : base(links)
        {
            _frequencyPropertyOperator = frequencyPropertyOperator;
            _unaryNumberToAddressConverter = unaryNumberToAddressConverter;
        }

        public: TLink Convert(Doublet<TLink> doublet)
        {
            auto links = _links;
            auto link = links.SearchOrDefault(doublet.Source, doublet.Target);
            if (link == 0)
            {
                throw std::invalid_argument(std::string("Link (").append(Platform::Converters::To<std::string>(doublet)).append(") not found."), "doublet");
            }
            auto frequency = _frequencyPropertyOperator.Get(link);
            if (frequency == 0)
            {
                return 0;
            }
            auto frequencyNumber = links.GetSource(frequency);
            return _unaryNumberToAddressConverter.Convert(frequencyNumber);
        }
    };
}
