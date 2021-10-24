namespace Platform::Data::Doublets::Numbers::Raw
{
    template <typename ...> class NumberToLongRawNumberSequenceConverter;
    template <typename TSource, typename TTarget> class NumberToLongRawNumberSequenceConverter<TSource, TTarget> : public LinksDecoratorBase<TTarget>, IConverter<TSource, TTarget>
    {
        private: inline static const TSource _maximumValue = NumericType<TSource>.MaxValue;
        private: inline static const std::int32_t _bitsPerRawNumber = NumericType<TTarget>.BitsSize - 1;
        private: inline static const TSource _bitMask = Bit.ShiftRight(_maximumValue, NumericType<TTarget>.BitsSize + 1);
        private: inline static const TSource _maximumConvertableAddress = CheckedConverter<TTarget, TSource>.Default.Convert((Hybrid<TTarget>.ExternalZero) - 1);
        private: static readonly UncheckedConverter<TSource, TTarget> _sourceToTargetConverter = UncheckedConverter<TSource, TTarget>.Default;

        private: IConverter<TTarget> *_addressToNumberConverter;

        public: NumberToLongRawNumberSequenceConverter(ILinks<TTarget> &links, IConverter<TTarget> &addressToNumberConverter) : base(links) { return _addressToNumberConverter = addressToNumberConverter; }

        public: TTarget Convert(TSource source)
        {
            if (source > _maximumConvertableAddress)
            {
                auto numberPart = Bit.And(source, _bitMask);
                auto convertedNumber = _addressToNumberConverter.Convert(_sourceToTargetConverter.Convert(numberPart));
                return Links.GetOrCreate(convertedNumber, this->Convert(Bit.ShiftRight(source, _bitsPerRawNumber)));
            }
            else
            {
                return _addressToNumberConverter.Convert(_sourceToTargetConverter.Convert(source));
            }
        }
    };
}
