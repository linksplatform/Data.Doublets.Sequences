namespace Platform::Data::Doublets::Numbers::Raw
{
    template <typename ...> class BigIntegerToRawNumberSequenceConverter;
    template <typename TLink> class BigIntegerToRawNumberSequenceConverter<TLink> : public LinksDecoratorBase<TLink>, IConverter<BigInteger, TLink>
        where TLink : struct
    {
        public: inline static const TLink MaximumValue = NumericType<TLink>.MaxValue;
        public: inline static const TLink BitMask = Bit.ShiftRight(MaximumValue, 1);
        public: IConverter<TLink> *AddressToNumberConverter;
        public: readonly IConverter<IList<TLink>, TLink> ListToSequenceConverter;
        public: TLink NegativeNumberMarker = 0;

        public: BigIntegerToRawNumberSequenceConverter(ILinks<TLink> &links, IConverter<TLink> &addressToNumberConverter, IConverter<IList<TLink>,TLink> listToSequenceConverter, TLink negativeNumberMarker) : base(links)
        {
            AddressToNumberConverter = addressToNumberConverter;
            ListToSequenceConverter = listToSequenceConverter;
            NegativeNumberMarker = negativeNumberMarker;
        }

        private: List<TLink> GetRawNumberParts(BigInteger bigInteger)
        {
            List<TLink> rawNumbers = new();
            BigInteger currentBigInt = bigInteger;
            do
            {
                auto bigIntBytes = currentBigInt.ToByteArray();
                auto bigIntWithBitMask = Bit.And(bigIntBytes.ToStructure<TLink>(), BitMask);
                auto rawNumber = AddressToNumberConverter.Convert(bigIntWithBitMask);
                rawNumbers.Add(rawNumber);
                currentBigInt >>= 63;
            }
            while (currentBigInt > 0);
            return rawNumbers;
        }

        public: TLink Convert(BigInteger bigInteger)
        {
            auto sign = bigInteger.Sign;
            auto number = this->GetRawNumberParts(sign == -1 ? BigInteger.Negate(bigInteger) : bigInteger);
            auto numberSequence = ListToSequenceConverter.Convert(number);
            return sign == -1 ? _links.GetOrCreate(NegativeNumberMarker, numberSequence) : numberSequence;
        }
    }
}
