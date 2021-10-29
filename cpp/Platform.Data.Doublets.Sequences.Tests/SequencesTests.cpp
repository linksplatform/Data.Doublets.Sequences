namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(SequencesTests)
    {
        private: static readonly LinksConstants<std::uint64_t> _constants = Default<LinksConstants<std::uint64_t>>.Instance;

        static SequencesTests()
        {
            _ = BitString.GetBitMaskFromIndex(1);
        }

        public: TEST_METHOD(CreateAllVariantsTest)
        {
            inline static const std::int64_t sequenceLength = 8;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto sw1 = Stopwatch.StartNew();
                auto results1 = sequences.CreateAllVariants1(sequence); sw1.Stop();

                auto sw2 = Stopwatch.StartNew();
                auto results2 = sequences.CreateAllVariants2(sequence); sw2.Stop();

                Assert::IsTrue(results1.Count() > results2.Length);
                Assert::IsTrue(sw1.Elapsed > sw2.Elapsed);

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }

                Assert::IsTrue(links.Count()() == 0);
            }
        }

        public: TEST_METHOD(AllVariantsSearchTest)
        {
            inline static const std::int64_t sequenceLength = 8;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto createResults = sequences.CreateAllVariants2(sequence)->Distinct().ToArray();

                auto sw0 = Stopwatch.StartNew();
                auto searchResults0 = sequences.GetAllMatchingSequences0(sequence); sw0.Stop();

                auto sw1 = Stopwatch.StartNew();
                auto searchResults1 = sequences.GetAllMatchingSequences1(sequence); sw1.Stop();

                auto sw2 = Stopwatch.StartNew();
                auto searchResults2 = sequences.Each1(sequence); sw2.Stop();

                auto sw3 = Stopwatch.StartNew();
                auto searchResults3 = sequences.Each(sequence.ShiftRight()); sw3.Stop();

                auto intersection0 = createResults.Intersect(searchResults0)->ToList();
                Assert::IsTrue(intersection0.Count() == searchResults0.Count());
                Assert::IsTrue(intersection0.Count() == createResults.Length);

                auto intersection1 = createResults.Intersect(searchResults1)->ToList();
                Assert::IsTrue(intersection1.Count() == searchResults1.Count());
                Assert::IsTrue(intersection1.Count() == createResults.Length);

                auto intersection2 = createResults.Intersect(searchResults2)->ToList();
                Assert::IsTrue(intersection2.Count() == searchResults2.Count());
                Assert::IsTrue(intersection2.Count() == createResults.Length);

                auto intersection3 = createResults.Intersect(searchResults3)->ToList();
                Assert::IsTrue(intersection3.Count() == searchResults3.Count());
                Assert::IsTrue(intersection3.Count() == createResults.Length);

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: TEST_METHOD(BalancedVariantSearchTest)
        {
            inline static const std::int64_t sequenceLength = 200;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(links);

                auto sw1 = Stopwatch.StartNew();
                auto balancedVariant = balancedVariantConverter.Convert(sequence); sw1.Stop();

                auto sw2 = Stopwatch.StartNew();
                auto searchResults2 = sequences.GetAllMatchingSequences0(sequence); sw2.Stop();

                auto sw3 = Stopwatch.StartNew();
                auto searchResults3 = sequences.GetAllMatchingSequences1(sequence); sw3.Stop();

                Assert::IsTrue(searchResults2.Count() == 1 && balancedVariant == searchResults2[0]);

                Assert::IsTrue(searchResults3.Count() == 1 && balancedVariant == searchResults3.First());

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: TEST_METHOD(AllPartialVariantsSearchTest)
        {
            inline static const std::int64_t sequenceLength = 8;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto createResults = sequences.CreateAllVariants2(sequence);

                auto partialSequence = std::uint64_t[sequenceLength - 2];

                Array.Copy(sequence, 1, partialSequence, 0, (std::int32_t)sequenceLength - 2);

                auto sw1 = Stopwatch.StartNew();
                auto searchResults1 = sequences.GetAllPartiallyMatchingSequences0(partialSequence); sw1.Stop();

                auto sw2 = Stopwatch.StartNew();
                auto searchResults2 = sequences.GetAllPartiallyMatchingSequences1(partialSequence); sw2.Stop();

                auto sw4 = Stopwatch.StartNew();
                auto searchResults4 = sequences.GetAllPartiallyMatchingSequences3(partialSequence); sw4.Stop();

                auto intersection1 = createResults.Intersect(searchResults1)->ToList();
                Assert::IsTrue(intersection1.Count() == createResults.Length);

                auto intersection2 = createResults.Intersect(searchResults2)->ToList();
                Assert::IsTrue(intersection2.Count() == createResults.Length);

                auto intersection4 = createResults.Intersect(searchResults4)->ToList();
                Assert::IsTrue(intersection4.Count() == createResults.Length);

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: TEST_METHOD(BalancedPartialVariantsSearchTest)
        {
            inline static const std::int64_t sequenceLength = 200;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(links);

                auto balancedVariant = balancedVariantConverter.Convert(sequence);

                auto partialSequence = std::uint64_t[sequenceLength - 2];

                Array.Copy(sequence, 1, partialSequence, 0, (std::int32_t)sequenceLength - 2);

                auto sw1 = Stopwatch.StartNew();
                auto searchResults1 = sequences.GetAllPartiallyMatchingSequences0(partialSequence); sw1.Stop();

                auto sw2 = Stopwatch.StartNew();
                auto searchResults2 = sequences.GetAllPartiallyMatchingSequences1(partialSequence); sw2.Stop();

                Assert::IsTrue(searchResults1.Count() == 1 && balancedVariant == searchResults1[0]);

                Assert::IsTrue(searchResults2.Count() == 1 && balancedVariant == searchResults2.First());

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: static void PatternMatchTest()
        {
            auto zeroOrMany = Sequences.ZeroOrMany;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto e1 = links.Create();
                auto e2 = links.Create();

                auto sequence = new[]
                {
                    e1, e2, e1, e2
                };

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(links);

                auto balancedVariant = balancedVariantConverter.Convert(sequence);

                auto doublet = links.GetSource(balancedVariant);

                auto matchedSequences1 = sequences.MatchPattern(e2, e1, zeroOrMany);

                Assert::IsTrue(matchedSequences1.Count() == 0);

                auto matchedSequences2 = sequences.MatchPattern(zeroOrMany, e2, e1);

                Assert::IsTrue(matchedSequences2.Count() == 0);

                auto matchedSequences3 = sequences.MatchPattern(e1, zeroOrMany, e1);

                Assert::IsTrue(matchedSequences3.Count() == 0);

                auto matchedSequences4 = sequences.MatchPattern(e1, zeroOrMany, e2);

                Assert.Contains(doublet, matchedSequences4);
                Assert.Contains(balancedVariant, matchedSequences4);

                for (auto i = 0; i < sequence.Length; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: TEST_METHOD(IndexTest)
        {
            using (auto scope = TempLinksTestScope(SequencesOptions<std::uint64_t> { UseIndex = true }, useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;
                auto index = sequences.Options.Index;

                auto e1 = links.Create();
                auto e2 = links.Create();

                auto sequence = new[]
                {
                    e1, e2, e1, e2
                };

                Assert::IsFalse(index.MightContain(sequence));

                index.Add(sequence);

                Assert::IsTrue(index.MightContain(sequence));
            }
        }

        private: static readonly std::string _exampleText =
            @"([english version](https://github.com/Konard/LinksPlatform/wiki/About-the-beginning))

Обозначение пустоты, какое оно? Темнота ли это? Там где отсутствие света, отсутствие фотонов (носителей света)? Или это то, что полностью отражает свет? Пустой белый лист бумаги? Там где есть место для нового начала? Разве пустота это не характеристика пространства? Пространство это то, что можно чем-то наполнить?

[![чёрное пространство, белое пространство](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/1.png ""чёрное пространство, белое пространство"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/1.png)

Что может быть минимальным рисунком, образом, графикой? Может быть это точка? Это ли простейшая форма? Но есть ли у точки размер? Цвет? Масса? Координаты? Время существования?

[![чёрное пространство, чёрная точка](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/2.png ""чёрное пространство, чёрная точка"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/2.png)

А что если повторить? Сделать копию? Создать дубликат? Из одного сделать два? Может это быть так? Инверсия? Отражение? Сумма?

[![белая точка, чёрная точка](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/3.png ""белая точка, чёрная точка"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/3.png)

А что если мы вообразим движение? Нужно ли время? Каким самым коротким будет путь? Что будет если этот путь зафиксировать? Запомнить след? Как две точки становятся линией? Чертой? Гранью? Разделителем? Единицей?

[![две белые точки, чёрная вертикальная линия](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/4.png ""две белые точки, чёрная вертикальная линия"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/4.png)

Можно ли замкнуть движение? Может ли это быть кругом? Можно ли замкнуть время? Или остаётся только спираль? Но что если замкнуть предел? Создать ограничение, разделение? Получится замкнутая область? Полностью отделённая от всего остального? Но что это всё остальное? Что можно делить? В каком направлении? Ничего или всё? Пустота или полнота? Начало или конец? Или может быть это единица и ноль? Дуальность? Противоположность? А что будет с кругом если у него нет размера? Будет ли круг точкой? Точка состоящая из точек?

[![белая вертикальная линия, чёрный круг](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/5.png ""белая вертикальная линия, чёрный круг"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/5.png)

Как ещё можно использовать грань, черту, линию? А что если она может что-то соединять, может тогда её нужно повернуть? Почему то, что перпендикулярно вертикальному горизонтально? Горизонт? Инвертирует ли это смысл? Что такое смысл? Из чего состоит смысл? Существует ли элементарная единица смысла?

[![белый круг, чёрная горизонтальная линия](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/6.png ""белый круг, чёрная горизонтальная линия"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/6.png)

Соединять, допустим, а какой смысл в этом есть ещё? Что если помимо смысла ""соединить, связать"", есть ещё и смысл направления ""от начала к концу""? От предка к потомку? От родителя к ребёнку? От общего к частному?

[![белая горизонтальная линия, чёрная горизонтальная стрелка](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/7.png ""белая горизонтальная линия, чёрная горизонтальная стрелка"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/7.png)

Шаг назад. Возьмём опять отделённую область, которая лишь та же замкнутая линия, что ещё она может представлять собой? Объект? Но в чём его суть? Разве не в том, что у него есть граница, разделяющая внутреннее и внешнее? Допустим связь, стрелка, линия соединяет два объекта, как бы это выглядело?

[![белая связь, чёрная направленная связь](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/8.png ""белая связь, чёрная направленная связь"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/8.png)

Допустим у нас есть смысл ""связать"" и смысл ""направления"", много ли это нам даёт? Много ли вариантов интерпретации? А что если уточнить, каким именно образом выполнена связь? Что если можно задать ей чёткий, конкретный смысл? Что это будет? Тип? Глагол? Связка? Действие? Трансформация? Переход из состояния в состояние? Или всё это и есть объект, суть которого в его конечном состоянии, если конечно конец определён направлением?

[![белая обычная и направленная связи, чёрная типизированная связь](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/9.png ""белая обычная и направленная связи, чёрная типизированная связь"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/9.png)

А что если всё это время, мы смотрели на суть как бы снаружи? Можно ли взглянуть на это изнутри? Что будет внутри объектов? Объекты ли это? Или это связи? Может ли эта структура описать сама себя? Но что тогда получится, разве это не рекурсия? Может это фрактал?

[![белая обычная и направленная связи с рекурсивной внутренней структурой, чёрная типизированная связь с рекурсивной внутренней структурой](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/10.png ""белая обычная и направленная связи с рекурсивной внутренней структурой, чёрная типизированная связь с рекурсивной внутренней структурой"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/10.png)

На один уровень внутрь (вниз)? Или на один уровень во вне (вверх)? Или это можно назвать шагом рекурсии или фрактала?

[![белая обычная и направленная связи с двойной рекурсивной внутренней структурой, чёрная типизированная связь с двойной рекурсивной внутренней структурой](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/11.png ""белая обычная и направленная связи с двойной рекурсивной внутренней структурой, чёрная типизированная связь с двойной рекурсивной внутренней структурой"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/11.png)

Последовательность? Массив? Список? Множество? Объект? Таблица? Элементы? Цвета? Символы? Буквы? Слово? Цифры? Число? Алфавит? Дерево? Сеть? Граф? Гиперграф?

[![белая обычная и направленная связи со структурой из 8 цветных элементов последовательности, чёрная типизированная связь со структурой из 8 цветных элементов последовательности](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/12.png ""белая обычная и направленная связи со структурой из 8 цветных элементов последовательности, чёрная типизированная связь со структурой из 8 цветных элементов последовательности"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/12.png)

...

[![анимация](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/intro-animation-500.gif ""анимация"")](https://raw.githubusercontent.com/Konard/LinksPlatform/master/doc/Intro/intro-animation-500.gif)";

        private: static readonly std::string _exampleLoremIpsumText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

        public: TEST_METHOD(CompressionTest)
        {
            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto e1 = links.Create();
                auto e2 = links.Create();

                auto sequence = new[]
                {
                    e1, e2, e1, e2
                };

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(links.Unsync);
                auto totalSequenceSymbolFrequencyCounter = TotalSequenceSymbolFrequencyCounter<std::uint64_t>(links.Unsync);
                auto doubletFrequenciesCache = LinkFrequenciesCache<std::uint64_t>(links.Unsync, totalSequenceSymbolFrequencyCounter);
                auto compressingConverter = CompressingConverter<std::uint64_t>(links.Unsync, balancedVariantConverter, doubletFrequenciesCache);

                auto compressedVariant = compressingConverter.Convert(sequence);

                Assert::IsTrue(links.GetSource(links.GetSource(compressedVariant)) == sequence[0]);
                Assert::IsTrue(links.GetTarget(links.GetSource(compressedVariant)) == sequence[1]);
                Assert::IsTrue(links.GetSource(links.GetTarget(compressedVariant)) == sequence[2]);
                Assert::IsTrue(links.GetTarget(links.GetTarget(compressedVariant)) == sequence[3]);

                auto source = _constants.SourcePart;
                auto target = _constants.TargetPart;

                Assert::IsTrue(links.GetByKeys(compressedVariant, source, source) == sequence[0]);
                Assert::IsTrue(links.GetByKeys(compressedVariant, source, target) == sequence[1]);
                Assert::IsTrue(links.GetByKeys(compressedVariant, target, source) == sequence[2]);
                Assert::IsTrue(links.GetByKeys(compressedVariant, target, target) == sequence[3]);

                Assert::IsTrue(links.GetSquareMatrixSequenceElementByIndex(compressedVariant, 4, 0) == sequence[0]);
                Assert::IsTrue(links.GetSquareMatrixSequenceElementByIndex(compressedVariant, 4, 1) == sequence[1]);
                Assert::IsTrue(links.GetSquareMatrixSequenceElementByIndex(compressedVariant, 4, 2) == sequence[2]);
                Assert::IsTrue(links.GetSquareMatrixSequenceElementByIndex(compressedVariant, 4, 3) == sequence[3]);
            }
        }

        public: TEST_METHOD(CompressionEfficiencyTest)
        {
            auto strings = _exampleLoremIpsumText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            auto arrays = strings.Select(UnicodeMap.FromStringToLinkArray).ToArray();
            auto totalCharacters = arrays.Select(x => x.Length).Sum();

            using (auto scope1 = TempLinksTestScope(useSequences: true))
            using (auto scope2 = TempLinksTestScope(useSequences: true))
            using (auto scope3 = TempLinksTestScope(useSequences: true))
            {
                scope1.Links.Unsync.UseUnicode();
                scope2.Links.Unsync.UseUnicode();
                scope3.Links.Unsync.UseUnicode();

                auto balancedVariantConverter1 = BalancedVariantConverter<std::uint64_t>(scope1.Links.Unsync);
                auto totalSequenceSymbolFrequencyCounter = TotalSequenceSymbolFrequencyCounter<std::uint64_t>(scope1.Links.Unsync);
                auto linkFrequenciesCache1 = LinkFrequenciesCache<std::uint64_t>(scope1.Links.Unsync, totalSequenceSymbolFrequencyCounter);
                auto compressor1 = CompressingConverter<std::uint64_t>(scope1.Links.Unsync, balancedVariantConverter1, linkFrequenciesCache1, doInitialFrequenciesIncrement: false);

                auto compressor3 = scope3.Sequences;

                auto constants = Default<LinksConstants<std::uint64_t>>.Instance;

                auto sequences = compressor3;

                auto linkFrequenciesCache3 = LinkFrequenciesCache<std::uint64_t>(scope3.Links.Unsync, totalSequenceSymbolFrequencyCounter);

                auto linkToItsFrequencyNumberConverter = FrequenciesCacheBasedLinkToItsFrequencyNumberConverter<std::uint64_t>(linkFrequenciesCache3);

                auto sequenceToItsLocalElementLevelsConverter = SequenceToItsLocalElementLevelsConverter<std::uint64_t>(scope3.Links.Unsync, linkToItsFrequencyNumberConverter);
                auto optimalVariantConverter = OptimalVariantConverter<std::uint64_t>(scope3.Links.Unsync, sequenceToItsLocalElementLevelsConverter);

                auto compressed1 = std::uint64_t[arrays.Length];
                auto compressed2 = std::uint64_t[arrays.Length];
                auto compressed3 = std::uint64_t[arrays.Length];

                auto START = 0;
                auto END = arrays.Length;

                auto initialCount1 = scope2.Links.Unsync.Count()();

                auto sw1 = Stopwatch.StartNew();

                for (std::int32_t i = START; i < END; i++)
                {
                    linkFrequenciesCache1.IncrementFrequencies(arrays[i]);
                    compressed1[i] = compressor1.Convert(arrays[i]);
                }

                auto elapsed1 = sw1.Elapsed;

                auto balancedVariantConverter2 = BalancedVariantConverter<std::uint64_t>(scope2.Links.Unsync);

                auto initialCount2 = scope2.Links.Unsync.Count()();

                auto sw2 = Stopwatch.StartNew();

                for (std::int32_t i = START; i < END; i++)
                {
                    compressed2[i] = balancedVariantConverter2.Convert(arrays[i]);
                }

                auto elapsed2 = sw2.Elapsed;

                for (std::int32_t i = START; i < END; i++)
                {
                    linkFrequenciesCache3.IncrementFrequencies(arrays[i]);
                }

                auto initialCount3 = scope3.Links.Unsync.Count()();

                auto sw3 = Stopwatch.StartNew();

                for (std::int32_t i = START; i < END; i++)
                {
                    compressed3[i] = optimalVariantConverter.Convert(arrays[i]);
                }

                auto elapsed3 = sw3.Elapsed;

                Console.WriteLine(std::string("Compressor: ").append(Platform::Converters::To<std::string>(elapsed1)).append(", Balanced variant: ").append(Platform::Converters::To<std::string>(elapsed2)).append(", Optimal variant: ").append(Platform::Converters::To<std::string>(elapsed3)).append(""));

                for (std::int32_t i = START; i < END; i++)
                {
                    auto sequence1 = compressed1[i];
                    auto sequence2 = compressed2[i];
                    auto sequence3 = compressed3[i];

                    auto decompress1 = UnicodeMap.FromSequenceLinkToString(sequence1, scope1.Links.Unsync);

                    auto decompress2 = UnicodeMap.FromSequenceLinkToString(sequence2, scope2.Links.Unsync);

                    auto decompress3 = UnicodeMap.FromSequenceLinkToString(sequence3, scope3.Links.Unsync);

                    auto structure1 = scope1.Links.Unsync.FormatStructure(sequence1, link => link.IsPartialPoint());
                    auto structure2 = scope2.Links.Unsync.FormatStructure(sequence2, link => link.IsPartialPoint());
                    auto structure3 = scope3.Links.Unsync.FormatStructure(sequence3, link => link.IsPartialPoint());

                    Assert::IsTrue(strings[i] == decompress1 && decompress1 == decompress2);
                    Assert::IsTrue(strings[i] == decompress3 && decompress3 == decompress2);
                }

                Assert::IsTrue((std::int32_t)(scope1.Links.Unsync.Count()() - initialCount1) < totalCharacters);
                Assert::IsTrue((std::int32_t)(scope2.Links.Unsync.Count()() - initialCount2) < totalCharacters);
                Assert::IsTrue((std::int32_t)(scope3.Links.Unsync.Count()() - initialCount3) < totalCharacters);

                printf("{(double)(scope1.Links.Unsync.Count()() - initialCount1) / totalCharacters} | {(double)(scope2.Links.Unsync.Count()() - initialCount2) / totalCharacters} | {(double)(scope3.Links.Unsync.Count()() - initialCount3) / totalCharacters}\n");

                Assert::IsTrue(scope1.Links.Unsync.Count()() - initialCount1 < scope2.Links.Unsync.Count()() - initialCount2);
                Assert::IsTrue(scope3.Links.Unsync.Count()() - initialCount3 < scope2.Links.Unsync.Count()() - initialCount2);

                auto duplicateProvider1 = DuplicateSegmentsProvider<std::uint64_t>(scope1.Links.Unsync, scope1.Sequences);
                auto duplicateProvider2 = DuplicateSegmentsProvider<std::uint64_t>(scope2.Links.Unsync, scope2.Sequences);
                auto duplicateProvider3 = DuplicateSegmentsProvider<std::uint64_t>(scope3.Links.Unsync, scope3.Sequences);

                auto duplicateCounter1 = DuplicateSegmentsCounter<std::uint64_t>(duplicateProvider1);
                auto duplicateCounter2 = DuplicateSegmentsCounter<std::uint64_t>(duplicateProvider2);
                auto duplicateCounter3 = DuplicateSegmentsCounter<std::uint64_t>(duplicateProvider3);

                auto duplicates1 = duplicateCounter1.Count()();

                ConsoleHelpers.Debug("------");

                auto duplicates2 = duplicateCounter2.Count()();

                ConsoleHelpers.Debug("------");

                auto duplicates3 = duplicateCounter3.Count()();

                Console.WriteLine(std::string("").append(Platform::Converters::To<std::string>(duplicates1)).append(" | ").append(Platform::Converters::To<std::string>(duplicates2)).append(" | ").append(Platform::Converters::To<std::string>(duplicates3)).append(""));

                linkFrequenciesCache1.ValidateFrequencies();
                linkFrequenciesCache3.ValidateFrequencies();
            }
        }

        public: TEST_METHOD(CompressionStabilityTest)
        {
            inline static const std::uint64_t minNumbers = 10000;
            inline static const std::uint64_t maxNumbers = 12500;

            auto strings = List<std::string>();

            for (std::uint64_t i = minNumbers; i < maxNumbers; i++)
            {
                strings.Add(i.ToString());
            }

            auto arrays = strings.Select(UnicodeMap.FromStringToLinkArray).ToArray();
            auto totalCharacters = arrays.Select(x => x.Length).Sum();

            using (auto scope1 = TempLinksTestScope(useSequences: true, sequencesOptions: SequencesOptions<std::uint64_t> { UseCompression = true, EnforceSingleSequenceVersionOnWriteBasedOnExisting = true }))
            using (auto scope2 = TempLinksTestScope(useSequences: true))
            {
                scope1.Links.UseUnicode();
                scope2.Links.UseUnicode();

                auto compressor1 = scope1.Sequences;
                auto compressor2 = scope2.Sequences;

                auto compressed1 = std::uint64_t[arrays.Length];
                auto compressed2 = std::uint64_t[arrays.Length];

                auto sw1 = Stopwatch.StartNew();

                auto START = 0;
                auto END = arrays.Length;

                for (std::int32_t i = START; i < END; i++)
                {
                    auto first = compressor1.Create(arrays[i].ShiftRight());
                    auto second = compressor1.Create(arrays[i].ShiftRight());

                    if (first == second)
                    {
                        compressed1[i] = first;
                    }
                    else
                    {
                    }
                }

                auto elapsed1 = sw1.Elapsed;

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(scope2.Links);

                auto sw2 = Stopwatch.StartNew();

                for (std::int32_t i = START; i < END; i++)
                {
                    auto first = balancedVariantConverter.Convert(arrays[i]);
                    auto second = balancedVariantConverter.Convert(arrays[i]);

                    if (first == second)
                    {
                        compressed2[i] = first;
                    }
                }

                auto elapsed2 = sw2.Elapsed;

                Debug.WriteLine(std::string("Compressor: ").append(Platform::Converters::To<std::string>(elapsed1)).append(", Balanced sequence creator: ").append(Platform::Converters::To<std::string>(elapsed2)).append(""));

                Assert::IsTrue(elapsed1 > elapsed2);

                for (std::int32_t i = START; i < END; i++)
                {
                    auto sequence1 = compressed1[i];
                    auto sequence2 = compressed2[i];

                    if (sequence1 != _constants.Null && sequence2 != _constants.Null)
                    {
                        auto decompress1 = UnicodeMap.FromSequenceLinkToString(sequence1, scope1.Links);

                        auto decompress2 = UnicodeMap.FromSequenceLinkToString(sequence2, scope2.Links);

                        Assert::IsTrue(strings[i] == decompress1 && decompress1 == decompress2);
                    }
                }

                Assert::IsTrue((std::int32_t)(scope1.Links.Count()() - UnicodeMap.MapSize) < totalCharacters);
                Assert::IsTrue((std::int32_t)(scope2.Links.Count()() - UnicodeMap.MapSize) < totalCharacters);

                Debug.WriteLine("{(double)(scope1.Links.Count()() - UnicodeMap.MapSize) / totalCharacters} | {(double)(scope2.Links.Count()() - UnicodeMap.MapSize) / totalCharacters}");

                Assert::IsTrue(scope1.Links.Count()() <= scope2.Links.Count()());
            }
        }

        public: TEST_METHOD(RundomNumbersCompressionQualityTest)
        {
            inline static const std::uint64_t N = 500;

            auto strings = List<std::string>();

            for (std::uint64_t i = 0; i < N; i++)
            {
                strings.Add(RandomHelpers.Default.NextUInt64().ToString());
            }

            strings = strings.Distinct().ToList();

            auto arrays = strings.Select(UnicodeMap.FromStringToLinkArray).ToArray();
            auto totalCharacters = arrays.Select(x => x.Length).Sum();

            using (auto scope1 = TempLinksTestScope(useSequences: true, sequencesOptions: SequencesOptions<std::uint64_t> { UseCompression = true, EnforceSingleSequenceVersionOnWriteBasedOnExisting = true }))
            using (auto scope2 = TempLinksTestScope(useSequences: true))
            {
                scope1.Links.UseUnicode();
                scope2.Links.UseUnicode();

                auto compressor1 = scope1.Sequences;
                auto compressor2 = scope2.Sequences;

                auto compressed1 = std::uint64_t[arrays.Length];
                auto compressed2 = std::uint64_t[arrays.Length];

                auto sw1 = Stopwatch.StartNew();

                auto START = 0;
                auto END = arrays.Length;

                for (std::int32_t i = START; i < END; i++)
                {
                    compressed1[i] = compressor1.Create(arrays[i].ShiftRight());
                }

                auto elapsed1 = sw1.Elapsed;

                auto balancedVariantConverter = BalancedVariantConverter<std::uint64_t>(scope2.Links);

                auto sw2 = Stopwatch.StartNew();

                for (std::int32_t i = START; i < END; i++)
                {
                    compressed2[i] = balancedVariantConverter.Convert(arrays[i]);
                }

                auto elapsed2 = sw2.Elapsed;

                Debug.WriteLine(std::string("Compressor: ").append(Platform::Converters::To<std::string>(elapsed1)).append(", Balanced sequence creator: ").append(Platform::Converters::To<std::string>(elapsed2)).append(""));

                Assert::IsTrue(elapsed1 > elapsed2);

                for (std::int32_t i = START; i < END; i++)
                {
                    auto sequence1 = compressed1[i];
                    auto sequence2 = compressed2[i];

                    if (sequence1 != _constants.Null && sequence2 != _constants.Null)
                    {
                        auto decompress1 = UnicodeMap.FromSequenceLinkToString(sequence1, scope1.Links);

                        auto decompress2 = UnicodeMap.FromSequenceLinkToString(sequence2, scope2.Links);

                        Assert::IsTrue(strings[i] == decompress1 && decompress1 == decompress2);
                    }
                }

                Assert::IsTrue((std::int32_t)(scope1.Links.Count()() - UnicodeMap.MapSize) < totalCharacters);
                Assert::IsTrue((std::int32_t)(scope2.Links.Count()() - UnicodeMap.MapSize) < totalCharacters);

                Debug.WriteLine("{(double)(scope1.Links.Count()() - UnicodeMap.MapSize) / totalCharacters} | {(double)(scope2.Links.Count()() - UnicodeMap.MapSize) / totalCharacters}");
            }
        }

        public: TEST_METHOD(AllTreeBreakDownAtSequencesCreationBugTest)
        {
            inline static const std::int64_t sequenceLength = 4;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto createResults = sequences.CreateAllVariants2(sequence);

                Global.Trash = createResults;

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: TEST_METHOD(AllPossibleConnectionsTest)
        {
            inline static const std::int64_t sequenceLength = 5;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto createResults = sequences.CreateAllVariants2(sequence);
                auto reverseResults = sequences.CreateAllVariants2(sequence.Reverse().ToArray());

                for (auto i = 0; i < 1; i++)
                {
                    auto sw1 = Stopwatch.StartNew();
                    auto searchResults1 = sequences.GetAllConnections(sequence); sw1.Stop();

                    auto sw2 = Stopwatch.StartNew();
                    auto searchResults2 = sequences.GetAllConnections1(sequence); sw2.Stop();

                    auto sw3 = Stopwatch.StartNew();
                    auto searchResults3 = sequences.GetAllConnections2(sequence); sw3.Stop();

                    auto sw4 = Stopwatch.StartNew();
                    auto searchResults4 = sequences.GetAllConnections3(sequence); sw4.Stop();

                    Global.Trash = searchResults3;
                    Global.Trash = searchResults4;

                    auto intersection1 = createResults.Intersect(searchResults1)->ToList();
                    Assert::IsTrue(intersection1.Count() == createResults.Length);

                    auto intersection2 = reverseResults.Intersect(searchResults1)->ToList();
                    Assert::IsTrue(intersection2.Count() == reverseResults.Length);

                    auto intersection0 = searchResults1.Intersect(searchResults2)->ToList();
                    Assert::IsTrue(intersection0.Count() == searchResults2.Count());

                    auto intersection3 = searchResults2.Intersect(searchResults3)->ToList();
                    Assert::IsTrue(intersection3.Count() == searchResults3.Count());

                    auto intersection4 = searchResults3.Intersect(searchResults4)->ToList();
                    Assert::IsTrue(intersection4.Count() == searchResults4.Count());
                }

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }

        public: static void CalculateAllUsagesTest()
        {
            inline static const std::int64_t sequenceLength = 3;

            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto sequence = std::uint64_t[sequenceLength];
                for (auto i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = links.Create();
                }

                auto createResults = sequences.CreateAllVariants2(sequence);

                for (auto i = 0; i < 1; i++)
                {
                    auto linksTotalUsages1 = std::uint64_t[links.Count()() + 1];

                    sequences.CalculateAllUsages(linksTotalUsages1);

                    auto linksTotalUsages2 = std::uint64_t[links.Count()() + 1];

                    sequences.CalculateAllUsages2(linksTotalUsages2);

                    auto intersection1 = linksTotalUsages1.Intersect(linksTotalUsages2)->ToList();
                    Assert::IsTrue(intersection1.Count() == linksTotalUsages2.Length);
                }

                for (auto i = 0; i < sequenceLength; i++)
                {
                    links.Delete(sequence[i]);
                }
            }
        }
    };
}
