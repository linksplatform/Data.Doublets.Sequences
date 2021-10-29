namespace Platform::Data::Doublets::Numbers::Unary
{
    template <typename ...> class UnaryNumberToAddressOrOperationConverter;
    template <typename TLink> class UnaryNumberToAddressOrOperationConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<TLink>
    {
        private: inline static const TLink _zero = 0;
        private: inline static const TLink _one = _zero + 1;

        private: readonly IDictionary<TLink, std::int32_t> *_unaryNumberPowerOf2Indicies;

        public: UnaryNumberToAddressOrOperationConverter(ILinks<TLink> &links, IConverter<std::int32_t, TLink> &powerOf2ToUnaryNumberConverter) : base(links) { return _unaryNumberPowerOf2Indicies = CreateUnaryNumberPowerOf2IndiciesDictionary(powerOf2ToUnaryNumberConverter); }

        public: TLink Convert(TLink sourceNumber)
        {
            auto links = _links;
            auto nullConstant = links.Constants.Null;
            auto source = sourceNumber;
            auto target = nullConstant;
            if (!source == nullConstant)
            {
                while (true)
                {
                    if (_unaryNumberPowerOf2Indicies.TryGetValue(source, out std::int32_t powerOf2Index))
                    {
                        this->SetBit(target, powerOf2Index);
                        break;
                    }
                    else
                    {
                        powerOf2Index = _unaryNumberPowerOf2Indicies[links.GetSource(source)];
                        this->SetBit(target, powerOf2Index);
                        source = links.GetTarget(source);
                    }
                }
            }
            return target;
        }

        private: static Dictionary<TLink, std::int32_t> CreateUnaryNumberPowerOf2IndiciesDictionary(IConverter<std::int32_t, TLink> &powerOf2ToUnaryNumberConverter)
        {
            auto unaryNumberPowerOf2Indicies = Dictionary<TLink, std::int32_t>();
            for (std::int32_t i = 0; i < NumericType<TLink>.BitsSize; i++)
            {
                unaryNumberPowerOf2Indicies.Add(powerOf2ToUnaryNumberConverter.Convert(i), i);
            }
            return unaryNumberPowerOf2Indicies;
        }

        private: static void SetBit(TLink* target, std::int32_t powerOf2Index) { *target = Bit.Or(*target, Bit.ShiftLeft(_one, powerOf2Index)); }
    };
}
