namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(UnaryNumberConvertersTests)
    {
        public: TEST_METHOD(ConvertersTest)
        {
            using (auto scope = TempLinksTestScope())
            {
                inline static const std::int32_t N = 10;
                auto links = scope.Links;
                auto meaningRoot = links.CreatePoint();
                auto one = links.CreateAndUpdate(meaningRoot, links.Constants.Itself);
                auto powerOf2ToUnaryNumberConverter = PowerOf2ToUnaryNumberConverter<std::uint64_t>(links, one);
                auto toUnaryNumberConverter = AddressToUnaryNumberConverter<std::uint64_t>(links, powerOf2ToUnaryNumberConverter);
                std::srand(0);
                std::uint64_t numbers[] = std::uint64_t[N];
                std::uint64_t unaryNumbers[] = std::uint64_t[N];
                for (std::int32_t i = 0; i < N; i++)
                {
                    numbers[i] = random.NextUInt64();
                    unaryNumbers[i] = toUnaryNumberConverter.Convert(numbers[i]);
                }
                auto fromUnaryNumberConverterUsingOrOperation = UnaryNumberToAddressOrOperationConverter<std::uint64_t>(links, powerOf2ToUnaryNumberConverter);
                auto fromUnaryNumberConverterUsingAddOperation = UnaryNumberToAddressAddOperationConverter<std::uint64_t>(links, one);
                for (std::int32_t i = 0; i < N; i++)
                {
                    Assert::AreEqual(numbers[i], fromUnaryNumberConverterUsingOrOperation.Convert(unaryNumbers[i]));
                    Assert::AreEqual(numbers[i], fromUnaryNumberConverterUsingAddOperation.Convert(unaryNumbers[i]));
                }
            }
        }
    };
}
