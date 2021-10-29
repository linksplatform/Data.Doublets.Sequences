namespace Platform::Data::Doublets::Time
{
    template <typename ...> class LongRawNumberSequenceToDateTimeConverter;
    template <typename TLink> class LongRawNumberSequenceToDateTimeConverter<TLink> : public IConverter<TLink, DateTime>
    {
        private: readonly IConverter<TLink, std::int64_t> *_longRawNumberConverterToInt64;

        public: LongRawNumberSequenceToDateTimeConverter(IConverter<TLink, std::int64_t> &longRawNumberConverterToInt64) { _longRawNumberConverterToInt64 = longRawNumberConverterToInt64; }

        public: DateTime Convert(TLink source) { return DateTime.FromFileTimeUtc(_longRawNumberConverterToInt64.Convert(source)); }
    };
}
