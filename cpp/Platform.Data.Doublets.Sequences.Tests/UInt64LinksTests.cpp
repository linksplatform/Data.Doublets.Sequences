namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(UInt64LinksTests)
    {
        private: static readonly LinksConstants<std::uint64_t> _constants = Default<LinksConstants<std::uint64_t>>.Instance;

        private: inline static const std::int64_t Iterations = 10 * 1024;

        public: TEST_METHOD(MultipleCreateAndDeleteTest)
        {
            using (auto scope = Scope<Types<HeapResizableDirectMemory, UInt64UnitedMemoryLinks>>())
            {
                UInt64Links(scope.Use<ILinks<std::uint64_t>>()).TestMultipleRandomCreationsAndDeletions(100);
            }
        }

        public: TEST_METHOD(CascadeUpdateTest)
        {
            auto itself = _constants.Itself;
            using (auto scope = TempLinksTestScope(useLog: true))
            {
                auto links = scope.Links;

                auto l1 = links.Create();
                auto l2 = links.Create();

                l2 = links.Update(l2, l2, l1, l2);

                links.CreateAndUpdate(l2, itself);
                links.CreateAndUpdate(l2, itself);

                l2 = links.Update(l2, l1);

                links.Delete(l2);

                Global.Trash = links.Count()();

                links.Unsync.DisposeIfPossible();

                Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(scope.TempTransactionLogFilename);
            }
        }

        public: TEST_METHOD(BasicTransactionLogTest)
        {
            using (auto scope = TempLinksTestScope(useLog: true))
            {
                auto links = scope.Links;
                auto l1 = links.Create();
                auto l2 = links.Create();

                Global.Trash = links.Update(l2, l2, l1, l2);

                links.Delete(l1);

                links.Unsync.DisposeIfPossible();

                Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(scope.TempTransactionLogFilename);
            }
        }

        public: TEST_METHOD(TransactionAutoRevertedTest)
        {
            using (auto scope = TempLinksTestScope(useLog: true))
            {
                auto links = scope.Links;
                auto transactionsLayer = (UInt64LinksTransactionsLayer)scope.MemoryAdapter;
                using (auto transaction = transactionsLayer.BeginTransaction())
                {
                    auto l1 = links.Create();
                    auto l2 = links.Create();

                    links.Update(l2, l2, l1, l2);
                }

                Assert::AreEqual(0UL, links.Count()());

                links.Unsync.DisposeIfPossible();

                auto transitions = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(scope.TempTransactionLogFilename);
                Assert.Single(transitions);
            }
        }

        public: TEST_METHOD(TransactionUserCodeErrorNoDataSavedTest)
        {
            auto itself = _constants.Itself;

            TempLinksTestScope lastScope = {};
            try
            {
                using (auto scope = lastScope = TempLinksTestScope(deleteFiles: false, useLog: true))
                {
                    auto links = scope.Links;
                    auto transactionsLayer = (UInt64LinksTransactionsLayer)((LinksDisposableDecoratorBase<std::uint64_t>)links.Unsync).Links;
                    using (auto transaction = transactionsLayer.BeginTransaction())
                    {
                        auto l1 = links.CreateAndUpdate(itself, itself);
                        auto l2 = links.CreateAndUpdate(itself, itself);

                        l2 = links.Update(l2, l2, l1, l2);

                        links.CreateAndUpdate(l2, itself);
                        links.CreateAndUpdate(l2, itself);

                        l2 = links.Update(l2, l1);

                        links.Delete(l2);

                        ExceptionThrower();

                        transaction.Commit();
                    }

                    Global.Trash = links.Count()();
                }
            }
            catch
            {
                Assert::IsFalse(lastScope == nullptr);

                auto transitions = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(lastScope.TempTransactionLogFilename);

                Assert::IsTrue(transitions.Length == 1 && transitions[0].Before.IsNull() && transitions[0].After.IsNull());

                lastScope.DeleteFiles();
            }
        }

        public: TEST_METHOD(TransactionUserCodeErrorSomeDataSavedTest)
        {
            auto itself = _constants.Itself;

            TempLinksTestScope lastScope = {};
            try
            {
                std::uint64_t l1 = 0;
                std::uint64_t l2 = 0;

                using (auto scope = TempLinksTestScope(useLog: true))
                {
                    auto links = scope.Links;
                    l1 = links.CreateAndUpdate(itself, itself);
                    l2 = links.CreateAndUpdate(itself, itself);

                    l2 = links.Update(l2, l2, l1, l2);

                    links.CreateAndUpdate(l2, itself);
                    links.CreateAndUpdate(l2, itself);

                    links.Unsync.DisposeIfPossible();

                    Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(scope.TempTransactionLogFilename);
                }

                using (auto scope = lastScope = TempLinksTestScope(deleteFiles: false, useLog: true))
                {
                    auto links = scope.Links;
                    auto transactionsLayer = (UInt64LinksTransactionsLayer)links.Unsync;
                    using (auto transaction = transactionsLayer.BeginTransaction())
                    {
                        l2 = links.Update(l2, l1);

                        links.Delete(l2);

                        ExceptionThrower();

                        transaction.Commit();
                    }

                    Global.Trash = links.Count()();
                }
            }
            catch
            {
                Assert::IsFalse(lastScope == nullptr);

                Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(lastScope.TempTransactionLogFilename);

                lastScope.DeleteFiles();
            }
        }

        public: TEST_METHOD(TransactionCommit)
        {
            auto itself = _constants.Itself;

            auto tempDatabaseFilename = Path.GetTempFileName();
            auto tempTransactionLogFilename = Path.GetTempFileName();

            using (auto memoryAdapter = UInt64LinksTransactionsLayer(UInt64UnitedMemoryLinks(tempDatabaseFilename), tempTransactionLogFilename))
            using (auto links = UInt64Links(memoryAdapter))
            {
                using (auto transaction = memoryAdapter.BeginTransaction())
                {
                    auto l1 = links.CreateAndUpdate(itself, itself);
                    auto l2 = links.CreateAndUpdate(itself, itself);

                    Global.Trash = links.Update(l2, l2, l1, l2);

                    links.Delete(l1);

                    transaction.Commit();
                }

                Global.Trash = links.Count()();
            }

            Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(tempTransactionLogFilename);
        }

        public: TEST_METHOD(TransactionDamage)
        {
            auto itself = _constants.Itself;

            auto tempDatabaseFilename = Path.GetTempFileName();
            auto tempTransactionLogFilename = Path.GetTempFileName();

            using (auto memoryAdapter = UInt64LinksTransactionsLayer(UInt64UnitedMemoryLinks(tempDatabaseFilename), tempTransactionLogFilename))
            using (auto links = UInt64Links(memoryAdapter))
            {
                using (auto transaction = memoryAdapter.BeginTransaction())
                {
                    auto l1 = links.CreateAndUpdate(itself, itself);
                    auto l2 = links.CreateAndUpdate(itself, itself);

                    Global.Trash = links.Update(l2, l2, l1, l2);

                    links.Delete(l1);

                    transaction.Commit();
                }

                Global.Trash = links.Count()();
            }

            Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(tempTransactionLogFilename);

            FileHelpers.WriteFirst(tempTransactionLogFilename, UInt64LinksTransactionsLayer.Transition(UniqueTimestampFactory(), 555));

            try
            {
                using (auto memoryAdapter = UInt64LinksTransactionsLayer(UInt64UnitedMemoryLinks(tempDatabaseFilename), tempTransactionLogFilename))
                using (auto links = UInt64Links(memoryAdapter))
                {
                    Global.Trash = links.Count()();
                }
            }
            catch (NotSupportedException ex)
            {
                Assert::IsTrue(ex.Message == "Database is damaged, autorecovery is not supported yet.");
            }

            Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(tempTransactionLogFilename);

            File.Delete(tempDatabaseFilename);
            File.Delete(tempTransactionLogFilename);
        }

        public: TEST_METHOD(Bug1Test)
        {
            auto tempDatabaseFilename = Path.GetTempFileName();
            auto tempTransactionLogFilename = Path.GetTempFileName();

            auto itself = _constants.Itself;

            try
            {
                std::uint64_t l1 = 0;
                std::uint64_t l2 = 0;

                using (auto memory = UInt64UnitedMemoryLinks(tempDatabaseFilename))
                using (auto memoryAdapter = UInt64LinksTransactionsLayer(memory, tempTransactionLogFilename))
                using (auto links = UInt64Links(memoryAdapter))
                {
                    l1 = links.CreateAndUpdate(itself, itself);
                    l2 = links.CreateAndUpdate(itself, itself);

                    l2 = links.Update(l2, l2, l1, l2);

                    links.CreateAndUpdate(l2, itself);
                    links.CreateAndUpdate(l2, itself);
                }

                Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(tempTransactionLogFilename);

                using (auto memory = UInt64UnitedMemoryLinks(tempDatabaseFilename))
                using (auto memoryAdapter = UInt64LinksTransactionsLayer(memory, tempTransactionLogFilename))
                using (auto links = UInt64Links(memoryAdapter))
                {
                    using (auto transaction = memoryAdapter.BeginTransaction())
                    {
                        l2 = links.Update(l2, l1);

                        links.Delete(l2);

                        ExceptionThrower();

                        transaction.Commit();
                    }

                    Global.Trash = links.Count()();
                }
            }
            catch
            {
                Global.Trash = FileHelpers.ReadAll<UInt64LinksTransactionsLayer.Transition>(tempTransactionLogFilename);
            }

            File.Delete(tempDatabaseFilename);
            File.Delete(tempTransactionLogFilename);
        }

        private: static void ExceptionThrower() { throw std::runtime_error(); }

        public: TEST_METHOD(PathsTest)
        {
            auto source = _constants.SourcePart;
            auto target = _constants.TargetPart;

            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                auto l1 = links.CreatePoint();
                auto l2 = links.CreatePoint();

                auto r1 = links.GetByKeys(l1, source, target, source);
                auto r2 = links.CheckPathExistance(l2, l2, l2, l2);
            }
        }

        public: TEST_METHOD(RecursiveStringFormattingTest)
        {
            using (auto scope = TempLinksTestScope(useSequences: true))
            {
                auto links = scope.Links;
                auto sequences = scope.Sequences;

                auto a = links.CreatePoint();
                auto b = links.CreatePoint();
                auto c = links.CreatePoint();

                auto ab = links.GetOrCreate(a, b);
                auto cb = links.GetOrCreate(c, b);
                auto ac = links.GetOrCreate(a, c);

                a = links.Update(a, c, b);
                b = links.Update(b, a, c);
                c = links.Update(c, a, b);

                Debug.WriteLine(links.FormatStructure(ab, link => link.IsFullPoint(), true));
                Debug.WriteLine(links.FormatStructure(cb, link => link.IsFullPoint(), true));
                Debug.WriteLine(links.FormatStructure(ac, link => link.IsFullPoint(), true));

                Assert::IsTrue(links.FormatStructure(cb, link => link.IsFullPoint(), true) == "(5:(4:5 (6:5 4)) 6)");
                Assert::IsTrue(links.FormatStructure(ac, link => link.IsFullPoint(), true) == "(6:(5:(4:5 6) 6) 4)");
                Assert::IsTrue(links.FormatStructure(ab, link => link.IsFullPoint(), true) == "(4:(5:4 (6:5 4)) 6)");

                Assert::IsTrue(sequences.SafeFormatSequence(cb, DefaultFormatter, false) == "{{5}{5}{4}{6}}");
                Assert::IsTrue(sequences.SafeFormatSequence(ac, DefaultFormatter, false) == "{{5}{6}{6}{4}}");
                Assert::IsTrue(sequences.SafeFormatSequence(ab, DefaultFormatter, false) == "{{4}{5}{4}{6}}");
            }
        }

        private: static void DefaultFormatter(std::string& sb, std::uint64_t link)
        {
            sb.append(Platform::Converters::To<std::string>(Platform::Converters::To<std::string>(link).data()));
        }

        /*
       public: static void RunAllPerformanceTests()
       {
           try
           {
               links.TestLinksInSteps();
           }
           catch (const std::exception& ex)
           {
               ex.WriteToConsole();
           }

           return;

           try
           {
               for (auto i = 0; i < 10; i++)
               {
                   links.Create64BillionLinks();

                   links.TestRandomSearchFixed();
                   links.TestEachFunction();
               }

               links.TestDeletionOfAllLinks();
           }
           catch (const std::exception& ex)
           {
               ex.WriteToConsole();
           }
       }*/

        /*
       public: static void TestLinksInSteps()
       {
           inline static const std::int64_t gibibyte = 1024 * 1024 * 1024;
           inline static const std::int64_t mebibyte = 1024 * 1024;

           auto totalLinksToCreate = gibibyte / Platform.Links.Data.Core.Doublets.Links.LinkSizeInBytes;
           auto linksStep = 102 * mebibyte / Platform.Links.Data.Core.Doublets.Links.LinkSizeInBytes;

           auto creationMeasurements = List<TimeSpan>();
           auto searchMeasuremets = List<TimeSpan>();
           auto deletionMeasurements = List<TimeSpan>();

           GetBaseRandomLoopOverhead(linksStep);
           GetBaseRandomLoopOverhead(linksStep);

           auto stepLoopOverhead = GetBaseRandomLoopOverhead(linksStep);

           ConsoleHelpers.Debug("Step loop overhead: {0}.", stepLoopOverhead);

           auto loops = totalLinksToCreate / linksStep;

           for (std::int32_t i = 0; i < loops; i++)
           {
               creationMeasurements.Add(Measure([&]()-> auto { return links.RunRandomCreations(linksStep))); }
               searchMeasuremets.Add(Measure([&]()-> auto { return links.RunRandomSearches(linksStep))); }

               Console.Write("\rC + S {0}/{1}", i + 1, loops);
           }

           ConsoleHelpers.Debug();

           for (std::int32_t i = 0; i < loops; i++)
           {
               deletionMeasurements.Add(Measure([&]()-> auto { return links.RunRandomDeletions(linksStep))); }

               Console.Write("\rD {0}/{1}", i + 1, loops);
           }

           ConsoleHelpers.Debug();

           ConsoleHelpers.Debug("C S D");

           for (std::int32_t i = 0; i < loops; i++)
           {
               ConsoleHelpers.Debug("{0} {1} {2}", creationMeasurements[i], searchMeasuremets[i], deletionMeasurements[i]);
           }

           ConsoleHelpers.Debug("C S D (no overhead)");

           for (std::int32_t i = 0; i < loops; i++)
           {
               ConsoleHelpers.Debug("{0} {1} {2}", creationMeasurements[i] - stepLoopOverhead, searchMeasuremets[i] - stepLoopOverhead, deletionMeasurements[i] - stepLoopOverhead);
           }

           ConsoleHelpers.Debug("All tests done. Total links left in database: {0}.", links.Total);
       }

       private: static void CreatePoints(Platform.Links.Data.Core.Doublets.Links links, std::int64_t amountToCreate)
       {
           for (std::int64_t i = 0; i < amountToCreate; i++)
               links.Create(0, 0);
       }
       
        private: static TimeSpan GetBaseRandomLoopOverhead(std::int64_t loops)
        {
            return Measure(() =>
            {
                std::uint64_t maxValue = RandomHelpers.DefaultFactory.NextUInt64();
                std::uint64_t result = 0;
                for (std::int64_t i = 0; i < loops; i++)
                {
                    auto source = RandomHelpers.DefaultFactory.NextUInt64(maxValue);
                    auto target = RandomHelpers.DefaultFactory.NextUInt64(maxValue);

                    result += maxValue + source + target;
                }
                Global.Trash = result;
            });
        }
         */

        public: static void GetSourceTest()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                ConsoleHelpers.Debug("Testing GetSource function with {0} Iterations.", Iterations);

                std::uint64_t counter = 0;

                auto firstLink = links.Create();

                auto sw = Stopwatch.StartNew();

                for (std::uint64_t i = 0; i < Iterations; i++)
                {
                    counter += links.GetSource(firstLink);
                }

                auto elapsedTime = sw.Elapsed;

                auto iterationsPerSecond = Iterations / elapsedTime.TotalSeconds;

                links.Delete(firstLink);

                ConsoleHelpers.Debug(
                    "{0} Iterations of GetSource function done in {1} ({2} Iterations per second), counter result: {3}",
                    Iterations, elapsedTime, (std::int64_t)iterationsPerSecond, counter);
            }
        }

        public: static void GetSourceInParallel()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                ConsoleHelpers.Debug("Testing GetSource function with {0} Iterations in parallel.", Iterations);

                std::int64_t counter = 0;

                auto firstLink = links.Create();

                auto sw = Stopwatch.StartNew();

                Parallel.For(0, Iterations, x =>
                {
                    Interlocked.Add(counter, (std::int64_t)links.GetSource(firstLink));
                });

                auto elapsedTime = sw.Elapsed;

                auto iterationsPerSecond = Iterations / elapsedTime.TotalSeconds;

                links.Delete(firstLink);

                ConsoleHelpers.Debug(
                    "{0} Iterations of GetSource function done in {1} ({2} Iterations per second), counter result: {3}",
                    Iterations, elapsedTime, (std::int64_t)iterationsPerSecond, counter);
            }
        }

        public: static void TestGetTarget()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                ConsoleHelpers.Debug("Testing GetTarget function with {0} Iterations.", Iterations);

                std::uint64_t counter = 0;

                auto firstLink = links.Create();

                auto sw = Stopwatch.StartNew();

                for (std::uint64_t i = 0; i < Iterations; i++)
                {
                    counter += links.GetTarget(firstLink);
                }

                auto elapsedTime = sw.Elapsed;

                auto iterationsPerSecond = Iterations / elapsedTime.TotalSeconds;

                links.Delete(firstLink);

                ConsoleHelpers.Debug(
                    "{0} Iterations of GetTarget function done in {1} ({2} Iterations per second), counter result: {3}",
                    Iterations, elapsedTime, (std::int64_t)iterationsPerSecond, counter);
            }
        }

        public: static void TestGetTargetInParallel()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                ConsoleHelpers.Debug("Testing GetTarget function with {0} Iterations in parallel.", Iterations);

                std::int64_t counter = 0;

                auto firstLink = links.Create();

                auto sw = Stopwatch.StartNew();

                Parallel.For(0, Iterations, x =>
                {
                    Interlocked.Add(counter, (std::int64_t)links.GetTarget(firstLink));
                });

                auto elapsedTime = sw.Elapsed;

                auto iterationsPerSecond = Iterations / elapsedTime.TotalSeconds;

                links.Delete(firstLink);

                ConsoleHelpers.Debug(
                    "{0} Iterations of GetTarget function done in {1} ({2} Iterations per second), counter result: {3}",
                    Iterations, elapsedTime, (std::int64_t)iterationsPerSecond, counter);
            }
        }

        /*
        public: TEST_METHOD(TestRandomSearchFixed)
        {
            auto tempFilename = Path.GetTempFileName();

            using (auto links = Platform.Links.Data.Core.Doublets.Links(tempFilename, DefaultLinksSizeStep))
            {
                std::int64_t iterations = 64 * 1024 * 1024 / Platform.Links.Data.Core.Doublets.Links.LinkSizeInBytes;

                std::uint64_t counter = 0;
                auto maxLink = links.Total;

                ConsoleHelpers.Debug("Testing Random Search with {0} Iterations.", iterations);

                auto sw = Stopwatch.StartNew();

                for (auto i = iterations; i > 0; i--)
                {
                    auto source = RandomHelpers.DefaultFactory.NextUInt64(LinksConstants.MinPossibleIndex, maxLink);
                    auto target = RandomHelpers.DefaultFactory.NextUInt64(LinksConstants.MinPossibleIndex, maxLink);

                    counter += links.Search(source, target);
                }

                auto elapsedTime = sw.Elapsed;

                auto iterationsPerSecond = iterations / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} Iterations of Random Search done in {1} ({2} Iterations per second), c: {3}", iterations, elapsedTime, (std::int64_t)iterationsPerSecond, counter);
            }

            File.Delete(tempFilename);
        }*/

        public: static void TestRandomSearchAll()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                std::uint64_t counter = 0;

                auto maxLink = links.Count()();

                auto iterations = links.Count()();

                ConsoleHelpers.Debug("Testing Random Search with {0} Iterations.", links.Count()());

                auto sw = Stopwatch.StartNew();

                for (auto i = iterations; i > 0; i--)
                {
                    auto linksAddressRange = Range<std::uint64_t>(_constants.InternalReferencesRange.Minimum, maxLink);

                    auto source = RandomHelpers.Default.NextUInt64(linksAddressRange);
                    auto target = RandomHelpers.Default.NextUInt64(linksAddressRange);

                    counter += links.SearchOrDefault(source, target);
                }

                auto elapsedTime = sw.Elapsed;

                auto iterationsPerSecond = iterations / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} Iterations of Random Search done in {1} ({2} Iterations per second), c: {3}",
                     iterations, elapsedTime, (std::int64_t)iterationsPerSecond, counter);
            }
        }

        public: static void TestEach()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;

                auto counter = Counter<IList<std::uint64_t>, std::uint64_t>(links.Constants.Continue);

                ConsoleHelpers.Debug("Testing Each function.");

                auto sw = Stopwatch.StartNew();

                links.Each(counter.IncrementAndReturnTrue);

                auto elapsedTime = sw.Elapsed;

                auto linksPerSecond = counter.Count() / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} Iterations of Each's handler function done in {1} ({2} links per second)",
                    counter, elapsedTime, (std::int64_t)linksPerSecond);
            }
        }

        /*
        public: TEST_METHOD(TestForeach)
        {
            auto tempFilename = Path.GetTempFileName();

            using (auto links = Platform.Links.Data.Core.Doublets.Links(tempFilename, DefaultLinksSizeStep))
            {
                std::uint64_t counter = 0;

                ConsoleHelpers.Debug("Testing foreach through links.");

                auto sw = Stopwatch.StartNew();

                auto elapsedTime = sw.Elapsed;

                auto linksPerSecond = (double)counter / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} Iterations of Foreach's handler block done in {1} ({2} links per second)", counter, elapsedTime, (std::int64_t)linksPerSecond);
            }

            File.Delete(tempFilename);
        }
        */

        /*
        public: TEST_METHOD(TestParallelForeach)
        {
            auto tempFilename = Path.GetTempFileName();

            using (auto links = Platform.Links.Data.Core.Doublets.Links(tempFilename, DefaultLinksSizeStep))
            {
                std::int64_t counter = 0;

                ConsoleHelpers.Debug("Testing parallel foreach through links.");

                auto sw = Stopwatch.StartNew();

                auto elapsedTime = sw.Elapsed;

                auto linksPerSecond = (double)counter / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} Iterations of Parallel Foreach's handler block done in {1} ({2} links per second)", counter, elapsedTime, (std::int64_t)linksPerSecond);
            }

            File.Delete(tempFilename);
        }
        */

        public: static void Create64BillionLinks()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                auto linksBeforeTest = links.Count()();

                std::int64_t linksToCreate = 64 * 1024 * 1024 / UInt64UnitedMemoryLinks.LinkSizeInBytes;

                ConsoleHelpers.Debug("Creating {0} links.", linksToCreate);

                auto elapsedTime = Performance.Measure(() =>
                {
                    for (std::int64_t i = 0; i < linksToCreate; i++)
                    {
                        links.Create();
                    }
                });

                auto linksCreated = links.Count()() - linksBeforeTest;
                auto linksPerSecond = linksCreated / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("Current links count: {0}.", links.Count()());

                ConsoleHelpers.Debug("{0} links created in {1} ({2} links per second)", linksCreated, elapsedTime,
                    (std::int64_t)linksPerSecond);
            }
        }

        public: static void Create64BillionLinksInParallel()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                auto linksBeforeTest = links.Count()();

                auto sw = Stopwatch.StartNew();

                std::int64_t linksToCreate = 64 * 1024 * 1024 / UInt64UnitedMemoryLinks.LinkSizeInBytes;

                ConsoleHelpers.Debug("Creating {0} links in parallel.", linksToCreate);

                Parallel.For(0, linksToCreate, x => links.Create());

                auto elapsedTime = sw.Elapsed;

                auto linksCreated = links.Count()() - linksBeforeTest;
                auto linksPerSecond = linksCreated / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} links created in {1} ({2} links per second)", linksCreated, elapsedTime,
                    (std::int64_t)linksPerSecond);
            }
        }

        public: static void TestDeletionOfAllLinks()
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                auto linksBeforeTest = links.Count()();

                ConsoleHelpers.Debug("Deleting all links");

                auto elapsedTime = Performance.Measure(links.DeleteAll);

                auto linksDeleted = linksBeforeTest - links.Count()();
                auto linksPerSecond = linksDeleted / elapsedTime.TotalSeconds;

                ConsoleHelpers.Debug("{0} links deleted in {1} ({2} links per second)", linksDeleted, elapsedTime,
                    (std::int64_t)linksPerSecond);
            }
        }
    };
}