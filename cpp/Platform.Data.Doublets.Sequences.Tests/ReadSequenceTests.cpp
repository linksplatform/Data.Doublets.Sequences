namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(ReadSequenceTests)
    {
        public: TEST_METHOD(ReadSequenceTest)
        {
            inline static const std::int64_t sequenceLength = 2000;

            using (auto scope = TempLinksTestScope(useSequences: false))
            {
                auto links = scope.Links;
                auto sequences = Sequences(links, SequencesOptions<std::uint64_t> { Walker = LeveledSequenceWalker<std::uint64_t>(links) });

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(links);

                auto sw1 = Stopwatch.StartNew();
                auto balancedVariant = balancedVariantConverter.Convert(sequence); sw1.Stop();

                auto sw2 = Stopwatch.StartNew();
                auto readSequence1 = sequences.ToList(balancedVariant); sw2.Stop();

                auto sw3 = Stopwatch.StartNew();
                auto readSequence2 = List<std::uint64_t>();
                SequenceWalker.WalkRight(balancedVariant,
                                         links.GetSource,
                                         links.GetTarget,
                                         links.IsPartialPoint,
                                         readSequence2.Add);
                sw3.Stop();

                Assert::IsTrue(sequence.SequenceEqual(readSequence1));

                Assert::IsTrue(sequence.SequenceEqual(readSequence2));

                printf("Stack-based walker: {sw3.Elapsed}, Level-based reader: {sw2.Elapsed}\n");

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }
    };
}
