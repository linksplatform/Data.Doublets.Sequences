namespace Platform::Data::Doublets::Sequences::Converters
{
    template <typename ...> class SequenceToItsLocalElementLevelsConverter;
    template <typename TLink> class SequenceToItsLocalElementLevelsConverter<TLink> : public LinksOperatorBase<TLink>, IConverter<IList<TLink>>
    {
        private: readonly IConverter<Doublet<TLink>, TLink> _linkToItsFrequencyToNumberConveter;

        public: SequenceToItsLocalElementLevelsConverter(ILinks<TLink> &links, IConverter<Doublet<TLink>, TLink> linkToItsFrequencyToNumberConveter) : base(links) { return _linkToItsFrequencyToNumberConveter = linkToItsFrequencyToNumberConveter; }

        public: IList<TLink> Convert(IList<TLink> &sequence)
        {
            auto levels = TLink[sequence.Count()];
            levels[0] = GetFrequencyNumber(sequence[0], sequence[1]);
            for (auto i = 1; i < sequence.Count() - 1; i++)
            {
                auto previous = GetFrequencyNumber(sequence[i - 1], sequence[i]);
                auto next = GetFrequencyNumber(sequence[i], sequence[i + 1]);
                levels[i] = previous > next ? previous : next;
            }
            levels[levels.Length - 1] = GetFrequencyNumber(sequence[sequence.Count() - 2], sequence[sequence.Count() - 1]);
            return levels;
        }

        public: TLink GetFrequencyNumber(TLink source, TLink target) { return _linkToItsFrequencyToNumberConveter.Convert(Doublet<TLink>(source, target)); }
    };
}
