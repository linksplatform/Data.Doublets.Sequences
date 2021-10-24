namespace Platform::Data::Doublets::Numbers::Rational
{
    template <typename ...> class DecimalToRationalConverter;
        where TLink: struct
    {
        public: BigIntegerToRawNumberSequenceConverter<TLink> BigIntegerToRawNumberSequenceConverter;

        public: DecimalToRationalConverter(ILinks<TLink> &links, BigIntegerToRawNumberSequenceConverter<TLink> bigIntegerToRawNumberSequenceConverter) : base(links)
        {
            BigIntegerToRawNumberSequenceConverter = bigIntegerToRawNumberSequenceConverter;
        }

            auto dotPosition = decimalAsString.IndexOf('.');
            auto decimalWithoutDots = decimalAsString;
            std::int32_t digitsAfterDot = 0;
            if (dotPosition != -1)
            {
                decimalWithoutDots = decimalWithoutDots.Remove(dotPosition, 1);
                digitsAfterDot = decimalAsString.Length - 1 - dotPosition;
            }
            BigInteger denominator = this->new(System::Math::Pow(10, digitsAfterDot));
            BigInteger numerator = BigInteger.Parse(decimalWithoutDots);
            BigInteger greatestCommonDivisor = 0;
            do
            {
                greatestCommonDivisor = BigInteger.GreatestCommonDivisor(numerator, denominator);
                numerator /= greatestCommonDivisor;
                denominator /= greatestCommonDivisor;
            }
            while (greatestCommonDivisor > 1);
            auto numeratorLink = BigIntegerToRawNumberSequenceConverter.Convert(numerator);
            auto denominatorLink = BigIntegerToRawNumberSequenceConverter.Convert(denominator);
            return _links.GetOrCreate(numeratorLink, denominatorLink);
        }
    }
}
