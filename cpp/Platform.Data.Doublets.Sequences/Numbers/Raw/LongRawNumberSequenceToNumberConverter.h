namespace Platform::Data::Doublets::Numbers::Raw
{
    template <typename ...> class LongRawNumberSequenceToNumberConverter;
    template <typename TSource, typename TTarget> class LongRawNumberSequenceToNumberConverter<TSource, TTarget> : public LinksDecoratorBase<TSource>, IConverter<TSource, TTarget>
    {
        private: inline static const std::int32_t _bitsPerRawNumber = NumericType<TSource>.BitsSize - 1;
        private: static readonly UncheckedConverter<TSource, TTarget> _sourceToTargetConverter = UncheckedConverter<TSource, TTarget>.Default;

        private: IConverter<TSource> *_numberToAddressConverter;

        public: LongRawNumberSequenceToNumberConverter(ILinks<TSource> &links, IConverter<TSource> &numberToAddressConverter) : base(links) { return _numberToAddressConverter = numberToAddressConverter; }

        public: TTarget Convert(TSource source)
        {
            auto constants = Links.Constants;
            auto externalReferencesRange = constants.ExternalReferencesRange;
            if (externalReferencesRange.HasValue && externalReferencesRange.Value.Contains(source))
            {
                return _sourceToTargetConverter.Convert(_numberToAddressConverter.Convert(source));
            }
            else
            {
                auto pair = Links.GetLink(source);
                auto walker = LeftSequenceWalker<TSource>(Links, DefaultStack<TSource>(), link { return externalReferencesRange.HasValue && externalReferencesRange.Value.Contains(link)); }
                TTarget result = 0;
                foreach (auto element in walker.Walk(source))
                {
                    result = Bit.Or(Bit.ShiftLeft(result, _bitsPerRawNumber), this->Convert(element));
                }
                return result;
            }
        }
    };
}
