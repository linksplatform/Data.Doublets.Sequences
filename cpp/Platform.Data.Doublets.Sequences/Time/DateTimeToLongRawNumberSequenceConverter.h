namespace Platform::Data::Doublets::Time
{
    template <typename ...> class DateTimeToLongRawNumberSequenceConverter;
    template <typename TLink> class DateTimeToLongRawNumberSequenceConverter<TLink> : public IConverter<DateTime, TLink>
    {
        private: readonly IConverter<std::int64_t, TLink> *_int64ToLongRawNumberConverter;

        public: DateTimeToLongRawNumberSequenceConverter(IConverter<std::int64_t, TLink> &int64ToLongRawNumberConverter) { _int64ToLongRawNumberConverter = int64ToLongRawNumberConverter; }

        public: TLink Convert(DateTime source) { return _int64ToLongRawNumberConverter.Convert(source.ToFileTimeUtc()); }
    };
}
