namespace Platform::Data::Doublets::Sequences
{
    class SequencesExtensions
    {
        public: template <typename TLink> static TLink Create(ILinks<TLink> &sequences, IList<TLink[]> &groupedSequence)
        {
            auto finalSequence = TLink[groupedSequence.Count()];
            for (auto i = 0; i < finalSequence.Length; i++)
            {
                auto part = groupedSequence[i];
                finalSequence[i] = part.Length == 1 ? part[0] : sequences.Create(part.ShiftRight());
            }
            return sequences.Create(finalSequence.ShiftRight());
        }

        public: static IList<TLink> ToList<TLink>(ILinks<TLink> &sequences, TLink sequence)
        {
            auto list = List<TLink>();
            auto filler = ListFiller<TLink, TLink>(list, sequences.Constants.Break);
            sequences.Each(filler.AddSkipFirstAndReturnConstant, LinkAddress<TLink>(sequence));
            return list;
        }
    };
}
