namespace Platform::Data::Doublets::Numbers::Unary
{
    template <typename ...> class PowerOf2ToUnaryNumberConverter;
    template <typename TLink> class PowerOf2ToUnaryNumberConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<std::int32_t, TLink>
    {
        private: TLink _unaryNumberPowersOf2[N] = { {0} };

        public: PowerOf2ToUnaryNumberConverter(ILinks<TLink> &links, TLink one) : base(links)
        {
            _unaryNumberPowersOf2 = TLink[64];
            _unaryNumberPowersOf2[0] = one;
        }

        public: TLink Convert(std::int32_t power)
        {
            Platform::Ranges::EnsureExtensions::ArgumentInRange(Platform::Exceptions::Ensure::Always, power, Range<std::int32_t>(0, _unaryNumberPowersOf2.Length - 1), "power");
            if (!_unaryNumberPowersOf2[power] == 0)
            {
                return _unaryNumberPowersOf2[power] = { {0} };
            }
            auto previousPowerOf2 = this->Convert(power - 1);
            auto powerOf2 = _links.GetOrCreate(previousPowerOf2, previousPowerOf2);
            _unaryNumberPowersOf2[power] = powerOf2;
            return powerOf2;
        }
    };
}
