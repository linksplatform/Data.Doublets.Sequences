namespace Platform::Data::Doublets::Numbers::Unary
{
    template <typename ...> class AddressToUnaryNumberConverter;
    template <typename TLink> class AddressToUnaryNumberConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<TLink>
    {
        private: inline static const TLink _zero = 0;
        private: inline static const TLink _one = _zero + 1;

        private: readonly IConverter<std::int32_t, TLink> *_powerOf2ToUnaryNumberConverter;

        public: AddressToUnaryNumberConverter(ILinks<TLink> &links, IConverter<std::int32_t, TLink> &powerOf2ToUnaryNumberConverter) : base(links) { return _powerOf2ToUnaryNumberConverter = powerOf2ToUnaryNumberConverter; }

        public: TLink Convert(TLink number)
        {
            auto links = _links;
            auto nullConstant = links.Constants.Null;
            auto target = nullConstant;
            for (auto i = 0; !number == _zero && i < NumericType<TLink>.BitsSize; i++)
            {
                if (Bit.And(number == _one, _one))
                {
                    target = target == nullConstant
                        ? _powerOf2ToUnaryNumberConverter.Convert(i)
                        : links.GetOrCreate(_powerOf2ToUnaryNumberConverter.Convert(i), target);
                }
                number = Bit.ShiftRight(number, 1);
            }
            return target;
        }
    };
}
