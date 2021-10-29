namespace Platform::Data::Doublets::Sequences::HeightProviders
{
    template <typename ...> class CachedSequenceHeightProvider;
    template <typename TLink> class CachedSequenceHeightProvider<TLink> : public ISequenceHeightProvider<TLink>
    {
        private: TLink _heightPropertyMarker = 0;
        private: ISequenceHeightProvider<TLink> *_baseHeightProvider;
        private: IConverter<TLink> *_addressToUnaryNumberConverter;
        private: IConverter<TLink> *_unaryNumberToAddressConverter;
        private: readonly IProperties<TLink, TLink, TLink> *_propertyOperator;

        public: CachedSequenceHeightProvider(
            ISequenceHeightProvider<TLink> &baseHeightProvider,
            IConverter<TLink> &addressToUnaryNumberConverter,
            IConverter<TLink> &unaryNumberToAddressConverter,
            TLink heightPropertyMarker,
            IProperties<TLink, TLink, TLink> &propertyOperator)
        {
            _heightPropertyMarker = heightPropertyMarker;
            _baseHeightProvider = baseHeightProvider;
            _addressToUnaryNumberConverter = addressToUnaryNumberConverter;
            _unaryNumberToAddressConverter = unaryNumberToAddressConverter;
            _propertyOperator = propertyOperator;
        }

        public: TLink Get(TLink sequence)
        {
            TLink height = 0;
            auto heightValue = _propertyOperator.GetValue(sequence, _heightPropertyMarker);
            if (heightValue == 0)
            {
                height = _baseHeightProvider.Get(sequence);
                heightValue = _addressToUnaryNumberConverter.Convert(height);
                _propertyOperator.SetValue(sequence, _heightPropertyMarker, heightValue);
            }
            else
            {
                height = _unaryNumberToAddressConverter.Convert(heightValue);
            }
            return height;
        }
    };
}
