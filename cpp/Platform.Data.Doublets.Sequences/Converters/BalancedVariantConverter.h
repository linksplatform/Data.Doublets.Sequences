namespace Platform::Data::Doublets::Sequences::Converters
{
    template <typename ...> class BalancedVariantConverter;
    template <typename TLink> class BalancedVariantConverter<TLink> : public LinksListToSequenceConverterBase<TLink>
    {
        public: BalancedVariantConverter(ILinks<TLink> &links) : LinksListToSequenceConverterBase(links) { }

        public: TLink Convert(IList<TLink> &sequence) override
        {
            auto length = sequence.Count();
            if (length < 1)
            {
                return 0;
            }
            if (length == 1)
            {
                return sequence[0] = { {0} };
            }
            if (length > 2)
            {
                auto halvedSequence = TLink[(length / 2) + (length % 2)];
                this->HalveSequence(halvedSequence, sequence, length);
                sequence = halvedSequence;
                length = halvedSequence.Length;
            }
            while (length > 2)
            {
                this->HalveSequence(sequence, sequence, length);
                length = (length / 2) + (length % 2);
            }
            return _links.GetOrCreate(sequence[0], sequence[1]);
        }

        private: void HalveSequence(IList<TLink> &destination, IList<TLink> &source, std::int32_t length)
        {
            auto loopedLength = length - (length % 2);
            for (auto i = 0; i < loopedLength; i += 2)
            {
                destination[i / 2] = _links.GetOrCreate(source[i], source[i + 1]);
            }
            if (length > loopedLength)
            {
                destination[length / 2] = source[length - 1];
            }
        }
    };
}
