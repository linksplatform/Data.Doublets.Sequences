namespace Platform::Data::Doublets::Numbers::Raw
{
    template <typename ...> class RawNumberSequenceToBigIntegerConverter;
    template <typename TLink> class RawNumberSequenceToBigIntegerConverter<TLink> : public LinksDecoratorBase<TLink>, IConverter<TLink, BigInteger>
        where TLink : struct
    {
        public: readonly IConverter<TLink, TLink> *NumberToAddressConverter;
        public: LeftSequenceWalker<TLink> LeftSequenceWalker;
        public: TLink NegativeNumberMarker = 0;

        public: RawNumberSequenceToBigIntegerConverter(ILinks<TLink> &links, IConverter<TLink, TLink> &numberToAddressConverter, TLink negativeNumberMarker) : base(links)
        {
            NumberToAddressConverter = numberToAddressConverter;
            LeftSequenceWalker = new(links, DefaultStack<TLink>());
            NegativeNumberMarker = negativeNumberMarker;
        }

        public: BigInteger Convert(TLink bigInteger)
        {
            auto sign = 1;
            auto bigIntegerSequence = bigInteger;
            if (_links.GetSource(bigIntegerSequence) == NegativeNumberMarker)
            {
                sign = -1;
                bigIntegerSequence = _links.GetTarget(bigInteger);
            }
            using auto enumerator = LeftSequenceWalker.Walk(bigIntegerSequence)->GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw std::exception("Raw number sequence cannot be empty.");
            }
            auto nextPart = NumberToAddressConverter.Convert(enumerator.Current);
            BigInteger currentBigInt = this->new(nextPart.ToBytes());
            while (enumerator.MoveNext())
            {
                currentBigInt <<= 63;
                nextPart = NumberToAddressConverter.Convert(enumerator.Current);
                currentBigInt |= this->BigInteger(nextPart.ToBytes());
            }
            return sign == -1 ? BigInteger.Negate(currentBigInt) : currentBigInt;
        }      
    }
}
