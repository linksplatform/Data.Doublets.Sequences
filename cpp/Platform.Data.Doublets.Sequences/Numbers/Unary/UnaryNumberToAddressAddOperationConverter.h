namespace Platform::Data::Doublets::Numbers::Unary
{
    template <typename ...> class UnaryNumberToAddressAddOperationConverter;
    template <typename TLink> class UnaryNumberToAddressAddOperationConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<TLink>
    {
        private: static readonly UncheckedConverter<TLink, std::uint64_t> _addressToUInt64Converter = UncheckedConverter<TLink, std::uint64_t>.Default;
        private: static readonly UncheckedConverter<std::uint64_t, TLink> _uInt64ToAddressConverter = UncheckedConverter<std::uint64_t, TLink>.Default;
        private: inline static const TLink _zero = 0;
        private: inline static const TLink _one = _zero + 1;

        private: readonly Dictionary<TLink, TLink> _unaryToUInt64;
        private: TLink _unaryOne = 0;

        public: UnaryNumberToAddressAddOperationConverter(ILinks<TLink> &links, TLink unaryOne)
            : base(links)
        {
            _unaryOne = unaryOne;
            _unaryToUInt64 = this->CreateUnaryToUInt64Dictionary(links, unaryOne);
        }

        public: TLink Convert(TLink unaryNumber)
        {
            if (unaryNumber == 0)
            {
                return 0;
            }
            if (unaryNumber == _unaryOne)
            {
                return _one;
            }
            auto links = _links;
            auto source = links.GetSource(unaryNumber);
            auto target = links.GetTarget(unaryNumber);
            if (source == target)
            {
                return _unaryToUInt64[unaryNumber] = { {0} };
            }
            else
            {
                auto result = _unaryToUInt64[source];
                TLink lastValue = 0;
                while (!_unaryToUInt64.TryGetValue(target, out lastValue))
                {
                    source = links.GetSource(target);
                    result = Arithmetic<TLink>.Add(result, _unaryToUInt64[source]);
                    target = links.GetTarget(target);
                }
                result = Arithmetic<TLink>.Add(result, lastValue);
                return result;
            }
        }

        private: static Dictionary<TLink, TLink> CreateUnaryToUInt64Dictionary(ILinks<TLink> &links, TLink unaryOne)
        {
            auto unaryToUInt64 = Dictionary<TLink, TLink>
            {
                { unaryOne, _one }
            };
            auto unary = unaryOne;
            auto number = _one;
            for (auto i = 1; i < 64; i++)
            {
                unary = links.GetOrCreate(unary, unary);
                number = Double(number);
                unaryToUInt64.Add(unary, number);
            }
            return unaryToUInt64;
        }

        private: static TLink Double(TLink number) { return _uInt64ToAddressConverter.Convert(_addressToUInt64Converter.Convert(number) * 2UL); }
    };
}
