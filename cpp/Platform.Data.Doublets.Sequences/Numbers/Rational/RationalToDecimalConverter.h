namespace Platform::Data::Doublets::Numbers::Rational
{
    template <typename ...> class RationalToDecimalConverter;
    {
        public: RawNumberSequenceToBigIntegerConverter<TLink> RawNumberSequenceToBigIntegerConverter;

        public: RationalToDecimalConverter(ILinks<TLink> &links, RawNumberSequenceToBigIntegerConverter<TLink> rawNumberSequenceToBigIntegerConverter) : base(links)
        {
            RawNumberSequenceToBigIntegerConverter = rawNumberSequenceToBigIntegerConverter;
        }

        {
            auto denominator = (decimal)RawNumberSequenceToBigIntegerConverter.Convert(_links.GetTarget(rationalNumber));
            return numerator / denominator;
        }
    }
}
