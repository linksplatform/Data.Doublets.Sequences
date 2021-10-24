

using LinkIndex = std::uint64_t;
using Stack = System::Collections::Generic::Stack<std::uint64_t>;

namespace Platform::Data::Doublets::Sequences
{
    partial class Sequences
    {
        public: std::uint64_t CreateAllVariants2[](std::uint64_t sequence[])
        {
            return _sync.ExecuteWriteOperation(() =>
            {
                if (sequence.IsNullOrEmpty())
                {
                    return Array.Empty<std::uint64_t>();
                }
                Links.EnsureLinkExists(sequence);
                if (sequence.Length == 1)
                {
                    return sequence;
                }
                return CreateAllVariants2Core(sequence, 0, (std::uint64_t)sequence.Length - 1);
            });
        }

        private: std::uint64_t CreateAllVariants2Core[](std::uint64_t sequence[], std::uint64_t startAt, std::uint64_t stopAt)
        {
            if ((stopAt - startAt) == 0)
            {
                return new[] { sequence[startAt] };
            }
            if ((stopAt - startAt) == 1)
            {
                return new[] { Links.Unsync.GetOrCreate(sequence[startAt], sequence[stopAt]) };
            }
            auto variants = std::uint64_t[Platform.Numbers.Math.Catalan(stopAt - startAt)];
            auto last = 0;
            for (auto splitter = startAt; splitter < stopAt; splitter++)
            {
                auto left = CreateAllVariants2Core(sequence, startAt, splitter);
                auto right = CreateAllVariants2Core(sequence, splitter + 1, stopAt);
                for (auto i = 0; i < left.Length; i++)
                {
                    for (auto j = 0; j < right.Length; j++)
                    {
                        auto variant = Links.Unsync.GetOrCreate(left[i], right[j]);
                        if (variant == Constants.Null)
                        {
                            throw NotImplementedException("Creation cancellation is not implemented.");
                        }
                        variants[last++] = variant;
                    }
                }
            }
            return variants;
        }

        public: List<std::uint64_t> CreateAllVariants1(params std::uint64_t sequence[])
        {
            return _sync.ExecuteWriteOperation(() =>
            {
                if (sequence.IsNullOrEmpty())
                {
                    return List<std::uint64_t>();
                }
                Links.Unsync.EnsureLinkExists(sequence);
                if (sequence.Length == 1)
                {
                    return List<std::uint64_t> { sequence[0] };
                }
                auto results = List<std::uint64_t>((std::int32_t)Platform.Numbers.Math.Catalan((std::uint64_t)sequence.Length));
                return CreateAllVariants1Core(sequence, results);
            });
        }

        private: List<std::uint64_t> CreateAllVariants1Core(std::uint64_t sequence[], List<std::uint64_t> results)
        {
            if (sequence.Length == 2)
            {
                auto link = Links.Unsync.GetOrCreate(sequence[0], sequence[1]);
                if (link == Constants.Null)
                {
                    throw NotImplementedException("Creation cancellation is not implemented.");
                }
                results.Add(link);
                return results;
            }
            auto innerSequenceLength = sequence.Length - 1;
            auto innerSequence = std::uint64_t[innerSequenceLength];
            for (auto li = 0; li < innerSequenceLength; li++)
            {
                auto link = Links.Unsync.GetOrCreate(sequence[li], sequence[li + 1]);
                if (link == Constants.Null)
                {
                    throw NotImplementedException("Creation cancellation is not implemented.");
                }
                for (auto isi = 0; isi < li; isi++)
                {
                    innerSequence[isi] = sequence[isi];
                }
                innerSequence[li] = link;
                for (auto isi = li + 1; isi < innerSequenceLength; isi++)
                {
                    innerSequence[isi] = sequence[isi + 1];
                }
                CreateAllVariants1Core(innerSequence, results);
            }
            return results;
        }

        public: HashSet<std::uint64_t> Each1(params std::uint64_t sequence[])
        {
            auto visitedLinks = HashSet<std::uint64_t>();
            Each1(link =>
            {
                if (!visitedLinks.Contains(link))
                {
                    visitedLinks.Add(link);
                }
                return true;
            }, sequence);
            return visitedLinks;
        }

        private: void Each1(Func<std::uint64_t, bool> handler, params std::uint64_t sequence[])
        {
            if (sequence.Length == 2)
            {
                Links.Unsync.Each(sequence[0], sequence[1], handler);
            }
            else
            {
                auto innerSequenceLength = sequence.Length - 1;
                for (auto li = 0; li < innerSequenceLength; li++)
                {
                    auto left = sequence[li];
                    auto right = sequence[li + 1];
                    if (left == 0 && right == 0)
                    {
                        continue;
                    }
                    auto linkIndex = li;
                    std::uint64_t innerSequence[] = {};
                    Links.Unsync.Each(doublet =>
                    {
                        if (innerSequence == nullptr)
                        {
                            innerSequence = std::uint64_t[innerSequenceLength];
                            for (auto isi = 0; isi < linkIndex; isi++)
                            {
                                innerSequence[isi] = sequence[isi];
                            }
                            for (auto isi = linkIndex + 1; isi < innerSequenceLength; isi++)
                            {
                                innerSequence[isi] = sequence[isi + 1];
                            }
                        }
                        innerSequence[linkIndex] = doublet[Constants.IndexPart];
                        this->Each1(handler, innerSequence);
                        return Constants.Continue;
                    }, Constants.Any, left, right);
                }
            }
        }

        public: HashSet<std::uint64_t> EachPart(params std::uint64_t sequence[])
        {
            auto visitedLinks = HashSet<std::uint64_t>();
            EachPartCore(link =>
            {
                auto linkIndex = link[Constants.IndexPart];
                if (!visitedLinks.Contains(linkIndex))
                {
                    visitedLinks.Add(linkIndex);
                }
                return Constants.Continue;
            }, sequence);
            return visitedLinks;
        }

        public: void EachPart(Func<IList<LinkIndex>, LinkIndex> handler, params std::uint64_t sequence[])
        {
            auto visitedLinks = HashSet<std::uint64_t>();
            this->EachPartCore(link =>
            {
                auto linkIndex = link[Constants.IndexPart];
                if (!visitedLinks.Contains(linkIndex))
                {
                    visitedLinks.Add(linkIndex);
                    return this->handler(LinkAddress<LinkIndex>(linkIndex));
                }
                return Constants.Continue;
            }, sequence);
        }

        private: void EachPartCore(Func<IList<LinkIndex>, LinkIndex> handler, params std::uint64_t sequence[])
        {
            if (sequence.IsNullOrEmpty())
            {
                return;
            }
            Links.EnsureLinkIsAnyOrExists(sequence);
            if (sequence.Length == 1)
            {
                auto link = sequence[0];
                if (link > 0)
                {
                    this->handler(LinkAddress<LinkIndex>(link));
                }
                else
                {
                    Links.Each(Constants.Any, Constants.Any, handler);
                }
            }
            else if (sequence.Length == 2)
            {
                Links.Each(sequence[1], Constants.Any, doublet =>
                {
                    auto match = Links.SearchOrDefault(sequence[0], doublet);
                    if (match != Constants.Null)
                    {
                        this->handler(LinkAddress<LinkIndex>(match));
                    }
                    return true;
                });
                Links.Each(Constants.Any, sequence[0], doublet =>
                {
                    auto match = Links.SearchOrDefault(doublet, sequence[1]);
                    if (match != 0)
                    {
                        this->handler(LinkAddress<LinkIndex>(match));
                    }
                    return true;
                });
                this->PartialStepRight(x => this->handler(x), sequence[0], sequence[1]);
            }
            else
            {
                throw std::logic_error("Not implemented exception.");
            }
        }

        private: void PartialStepRight(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t right)
        {
            Links.Unsync.Each(Constants.Any, left, doublet =>
            {
                this->StepRight(handler, doublet, right);
                if (left != doublet)
                {
                    this->PartialStepRight(handler, doublet, right);
                }
                return true;
            });
        }

        private: void StepRight(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t right)
        {
            Links.Unsync.Each(left, Constants.Any, rightStep =>
            {
                this->TryStepRightUp(handler, right, rightStep);
                return true;
            });
        }

        private: void TryStepRightUp(Action<IList<LinkIndex>> handler, std::uint64_t right, std::uint64_t stepFrom)
        {
            auto upStep = stepFrom;
            auto firstSource = Links.Unsync.GetTarget(upStep);
            while (firstSource != right && firstSource != upStep)
            {
                upStep = firstSource;
                firstSource = Links.Unsync.GetSource(upStep);
            }
            if (firstSource == right)
            {
                this->handler(LinkAddress<LinkIndex>(stepFrom));
            }
        }

        private: void PartialStepLeft(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t right)
        {
            Links.Unsync.Each(right, Constants.Any, doublet =>
            {
                this->StepLeft(handler, left, doublet);
                if (right != doublet)
                {
                    this->PartialStepLeft(handler, left, doublet);
                }
                return true;
            });
        }

        private: void StepLeft(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t right)
        {
            Links.Unsync.Each(Constants.Any, right, leftStep =>
            {
                this->TryStepLeftUp(handler, left, leftStep);
                return true;
            });
        }

        private: void TryStepLeftUp(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t stepFrom)
        {
            auto upStep = stepFrom;
            auto firstTarget = Links.Unsync.GetSource(upStep);
            while (firstTarget != left && firstTarget != upStep)
            {
                upStep = firstTarget;
                firstTarget = Links.Unsync.GetTarget(upStep);
            }
            if (firstTarget == left)
            {
                this->handler(LinkAddress<LinkIndex>(stepFrom));
            }
        }

        private: bool StartsWith(std::uint64_t sequence, std::uint64_t link)
        {
            auto upStep = sequence;
            auto firstSource = Links.Unsync.GetSource(upStep);
            while (firstSource != link && firstSource != upStep)
            {
                upStep = firstSource;
                firstSource = Links.Unsync.GetSource(upStep);
            }
            return firstSource == link;
        }

        private: bool EndsWith(std::uint64_t sequence, std::uint64_t link)
        {
            auto upStep = sequence;
            auto lastTarget = Links.Unsync.GetTarget(upStep);
            while (lastTarget != link && lastTarget != upStep)
            {
                upStep = lastTarget;
                lastTarget = Links.Unsync.GetTarget(upStep);
            }
            return lastTarget == link;
        }

        public: List<std::uint64_t> GetAllMatchingSequences0(params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto results = List<std::uint64_t>();
                if (sequence.Length > 0)
                {
                    Links.EnsureLinkExists(sequence);
                    auto firstElement = sequence[0];
                    if (sequence.Length == 1)
                    {
                        results.Add(firstElement);
                        return results;
                    }
                    if (sequence.Length == 2)
                    {
                        auto doublet = Links.SearchOrDefault(firstElement, sequence[1]);
                        if (doublet != Constants.Null)
                        {
                            results.Add(doublet);
                        }
                        return results;
                    }
                    auto linksInSequence = HashSet<std::uint64_t>(sequence);
                    void handler(IList<LinkIndex> &result)
                    {
                        auto resultIndex = result[Links.Constants.IndexPart];
                        auto filterPosition = 0;
                        StopableSequenceWalker.WalkRight(resultIndex, Links.Unsync.GetSource, Links.Unsync.GetTarget,
                            x => linksInSequence.Contains(x) || Links.Unsync.GetTarget(x) == x, x =>
                            {
                                if (filterPosition == sequence.Length)
                                {
                                    filterPosition = -2;
                                    return false;
                                }
                                if (x != sequence[filterPosition])
                                {
                                    filterPosition = -1;
                                    return false;
                                }
                                filterPosition++;

                                return true;
                            });
                        if (filterPosition == sequence.Length)
                        {
                            results.Add(resultIndex);
                        }
                    }
                    if (sequence.Length >= 2)
                    {
                        StepRight(handler, sequence[0], sequence[1]);
                    }
                    auto last = sequence.Length - 2;
                    for (auto i = 1; i < last; i++)
                    {
                        PartialStepRight(handler, sequence[i], sequence[i + 1]);
                    }
                    if (sequence.Length >= 3)
                    {
                        StepLeft(handler, sequence[sequence.Length - 2], sequence[sequence.Length - 1]);
                    }
                }
                return results;
            });
        }

        public: HashSet<std::uint64_t> GetAllMatchingSequences1(params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto results = HashSet<std::uint64_t>();
                if (sequence.Length > 0)
                {
                    Links.EnsureLinkExists(sequence);
                    auto firstElement = sequence[0];
                    if (sequence.Length == 1)
                    {
                        results.Add(firstElement);
                        return results;
                    }
                    if (sequence.Length == 2)
                    {
                        auto doublet = Links.SearchOrDefault(firstElement, sequence[1]);
                        if (doublet != Constants.Null)
                        {
                            results.Add(doublet);
                        }
                        return results;
                    }
                    auto matcher = Matcher(this, sequence, results, {});
                    if (sequence.Length >= 2)
                    {
                        StepRight(matcher.AddFullMatchedToResults, sequence[0], sequence[1]);
                    }
                    auto last = sequence.Length - 2;
                    for (auto i = 1; i < last; i++)
                    {
                        PartialStepRight(matcher.AddFullMatchedToResults, sequence[i], sequence[i + 1]);
                    }
                    if (sequence.Length >= 3)
                    {
                        StepLeft(matcher.AddFullMatchedToResults, sequence[sequence.Length - 2], sequence[sequence.Length - 1]);
                    }
                }
                return results;
            });
        }

        public: inline static const std::int32_t MaxSequenceFormatSize = 200;

        public: std::string FormatSequence(LinkIndex sequenceLink, params LinkIndex knownElements[]) { auto FormatSequence = [&]() -> return { return sb.Append(x), true, knownElements); }; }

        public: std::string FormatSequence(LinkIndex sequenceLink, std::function<void(StringBuilder, LinkIndex)> elementToString, bool insertComma, params LinkIndex knownElements[]) { return Links.SyncRoot.ExecuteReadOperation([&]()-> auto { return FormatSequence(Links.Unsync, sequenceLink, elementToString, insertComma, knownElements); }); }

        private: std::string FormatSequence(ILinks<LinkIndex> &links, LinkIndex sequenceLink, std::function<void(StringBuilder, LinkIndex)> elementToString, bool insertComma, params LinkIndex knownElements[])
        {
            auto linksInSequence = HashSet<std::uint64_t>(knownElements);
            std::string sb;
            sb.append(Platform::Converters::To<std::string>('{'));
            if (links.Exists(sequenceLink))
            {
                StopableSequenceWalker.WalkRight(sequenceLink, links.GetSource, links.GetTarget,
                    x => linksInSequence.Contains(x) || links.IsPartialPoint(x), element =>
                    {
                        if (insertComma && sb.Length > 1)
                        {
                            sb.Append(',');
                        }
                        elementToString(sb, element);
                        if (sb.Length < MaxSequenceFormatSize)
                        {
                            return true;
                        }
                        sb.Append(insertComma ? ", ..." : "...");
                        return false;
                    });
            }
            sb.append(Platform::Converters::To<std::string>('}'));
            return sb;
        }

        public: std::string SafeFormatSequence(LinkIndex sequenceLink, params LinkIndex knownElements[]) { auto SafeFormatSequence = [&]() -> return { return sb.append(Platform::Converters::To<std::string>(x)), true, knownElements); }; }

        public: std::string SafeFormatSequence(LinkIndex sequenceLink, std::function<void(StringBuilder, LinkIndex)> elementToString, bool insertComma, params LinkIndex knownElements[]) { return Links.SyncRoot.ExecuteReadOperation([&]()-> auto { return SafeFormatSequence(Links.Unsync, sequenceLink, elementToString, insertComma, knownElements); }); }

        private: std::string SafeFormatSequence(ILinks<LinkIndex> &links, LinkIndex sequenceLink, std::function<void(StringBuilder, LinkIndex)> elementToString, bool insertComma, params LinkIndex knownElements[])
        {
            auto linksInSequence = HashSet<std::uint64_t>(knownElements);
            auto entered = HashSet<std::uint64_t>();
            std::string sb;
            sb.append(Platform::Converters::To<std::string>('{'));
            if (links.Exists(sequenceLink))
            {
                StopableSequenceWalker.WalkRight(sequenceLink, links.GetSource, links.GetTarget,
                    x => linksInSequence.Contains(x) || links.IsFullPoint(x), entered.AddAndReturnVoid, x => { }, entered.DoNotContains, element =>
                    {
                        if (insertComma && sb.Length > 1)
                        {
                            sb.Append(',');
                        }
                        if (entered.Contains(element))
                        {
                            sb.append(Platform::Converters::To<std::string>('{'));
                            elementToString(sb, element);
                            sb.append(Platform::Converters::To<std::string>('}'));
                        }
                        else
                        {
                            elementToString(sb, element);
                        }
                        if (sb.Length < MaxSequenceFormatSize)
                        {
                            return true;
                        }
                        sb.Append(insertComma ? ", ..." : "...");
                        return false;
                    });
            }
            sb.append(Platform::Converters::To<std::string>('}'));
            return sb;
        }

        public: List<std::uint64_t> GetAllPartiallyMatchingSequences0(params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (sequence.Length > 0)
                {
                    Links.EnsureLinkExists(sequence);
                    auto results = HashSet<std::uint64_t>();
                    for (auto i = 0; i < sequence.Length; i++)
                    {
                        AllUsagesCore(sequence[i], results);
                    }
                    auto filteredResults = List<std::uint64_t>();
                    auto linksInSequence = HashSet<std::uint64_t>(sequence);
                    foreach (auto result in results)
                    {
                        auto filterPosition = -1;
                        StopableSequenceWalker.WalkRight(result, Links.Unsync.GetSource, Links.Unsync.GetTarget,
                            x => linksInSequence.Contains(x) || Links.Unsync.GetTarget(x) == x, x =>
                            {
                                if (filterPosition == (sequence.Length - 1))
                                {
                                    return false;
                                }
                                if (filterPosition >= 0)
                                {
                                    if (x == sequence[filterPosition + 1])
                                    {
                                        filterPosition++;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                if (filterPosition < 0)
                                {
                                    if (x == sequence[0])
                                    {
                                        filterPosition = 0;
                                    }
                                }
                                return true;
                            });
                        if (filterPosition == (sequence.Length - 1))
                        {
                            filteredResults.Add(result);
                        }
                    }
                    return filteredResults;
                }
                return List<std::uint64_t>();
            });
        }

        public: HashSet<std::uint64_t> GetAllPartiallyMatchingSequences1(params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (sequence.Length > 0)
                {
                    Links.EnsureLinkExists(sequence);
                    auto results = HashSet<std::uint64_t>();
                    for (auto i = 0; i < sequence.Length; i++)
                    {
                        AllUsagesCore(sequence[i], results);
                    }
                    auto filteredResults = HashSet<std::uint64_t>();
                    auto matcher = Matcher(this, sequence, filteredResults, {});
                    matcher.AddAllPartialMatchedToResults(results);
                    return filteredResults;
                }
                return HashSet<std::uint64_t>();
            });
        }

        public: bool GetAllPartiallyMatchingSequences2(Func<IList<LinkIndex>, LinkIndex> handler, params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (sequence.Length > 0)
                {
                    Links.EnsureLinkExists(sequence);

                    auto results = HashSet<std::uint64_t>();
                    auto filteredResults = HashSet<std::uint64_t>();
                    auto matcher = this->Matcher(this, sequence, filteredResults, handler);
                    for (auto i = 0; i < sequence.Length; i++)
                    {
                        if (!this->AllUsagesCore1(sequence[i], results, matcher.HandlePartialMatched))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return true;
            });
        }

        public: HashSet<std::uint64_t> GetAllPartiallyMatchingSequences3(params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (sequence.Length > 0)
                {
                    ILinksExtensions.EnsureLinkIsAnyOrExists(Links, sequence);
                    auto firstResults = HashSet<std::uint64_t>();
                    auto lastResults = HashSet<std::uint64_t>();
                    auto first = sequence.First(x => x != Constants.Any);
                    auto last = sequence.Last(x => x != Constants.Any);
                    AllUsagesCore(first, firstResults);
                    AllUsagesCore(last, lastResults);
                    firstResults.IntersectWith(lastResults);
                    auto filteredResults = HashSet<std::uint64_t>();
                    auto matcher = Matcher(this, sequence, filteredResults, {});
                    matcher.AddAllPartialMatchedToResults(firstResults);
                    return filteredResults;
                }
                return HashSet<std::uint64_t>();
            });
        }

        public: HashSet<std::uint64_t> GetAllPartiallyMatchingSequences4(HashSet<std::uint64_t> readAsElements, IList<std::uint64_t> &sequence)
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (sequence.Count() > 0)
                {
                    Links.EnsureLinkExists(sequence);
                    std::unordered_set<LinkIndex> results;
                    auto collector1 = AllUsagesCollector1(Links.Unsync, results);
                    collector1.Collect(Links.Unsync.GetLink(sequence[0]));
                    auto next = HashSet<std::uint64_t>();
                    for (auto i = 1; i < sequence.Count(); i++)
                    {
                        auto collector = AllUsagesCollector1(Links.Unsync, next);
                        collector.Collect(Links.Unsync.GetLink(sequence[i]));

                        results.IntersectWith(next);
                        next.Clear();
                    }
                    auto filteredResults = HashSet<std::uint64_t>();
                    auto matcher = Matcher(this, sequence, filteredResults, {}, readAsElements);
                    matcher.AddAllPartialMatchedToResultsAndReadAsElements(results.OrderBy(x => x));
                    return filteredResults;
                }
                return HashSet<std::uint64_t>();
            });
        }

        public: List<std::uint64_t> GetAllPartiallyMatchingSequences(params std::uint64_t sequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (sequence.Length > 0)
                {
                    Links.EnsureLinkExists(sequence);
                    auto firstLinkUsages = HashSet<std::uint64_t>();
                    AllUsagesCore(sequence[0], firstLinkUsages);
                    firstLinkUsages.Add(sequence[0]);
                    auto results = HashSet<std::uint64_t>();
                    foreach (auto match in GetAllPartiallyMatchingSequencesCore(sequence, firstLinkUsages, 1))
                    {
                        AllUsagesCore(match, results);
                    }
                    return results.ToList();
                }
                return List<std::uint64_t>();
            });
        }

        public: HashSet<std::uint64_t> AllUsages(std::uint64_t link)
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto usages = HashSet<std::uint64_t>();
                AllUsagesCore(link, usages);
                return usages;
            });
        }

        private: void AllUsagesCore(std::uint64_t link, HashSet<std::uint64_t> usages)
        {
            auto handler = [&]() -> bool {
                if (usages.Add(doublet))
                {
                    this->AllUsagesCore(doublet, usages);
                };
                return true;
            }
            Links.Unsync.Each(link, Constants.Any, handler);
            Links.Unsync.Each(Constants.Any, link, handler);
        }

        public: HashSet<std::uint64_t> AllBottomUsages(std::uint64_t link)
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto visits = HashSet<std::uint64_t>();
                auto usages = HashSet<std::uint64_t>();
                AllBottomUsagesCore(link, visits, usages);
                return usages;
            });
        }

        private: void AllBottomUsagesCore(std::uint64_t link, HashSet<std::uint64_t> visits, HashSet<std::uint64_t> usages)
        {
            auto handler = [&]() -> bool {
                if (visits.Add(doublet))
                {
                    this->AllBottomUsagesCore(doublet, visits, usages);
                };
                return true;
            }
            if (Links.Unsync.Count()(Constants.Any, link) == 0)
            {
                usages.Add(link);
            }
            else
            {
                Links.Unsync.Each(link, Constants.Any, handler);
                Links.Unsync.Each(Constants.Any, link, handler);
            }
        }

        public: std::uint64_t CalculateTotalSymbolFrequencyCore(std::uint64_t symbol)
        {
            if (Options.UseSequenceMarker)
            {
                auto counter = TotalMarkedSequenceSymbolFrequencyOneOffCounter<std::uint64_t>(Links, Options.MarkedSequenceMatcher, symbol);
                return counter.Count()();
            }
            else
            {
                auto counter = TotalSequenceSymbolFrequencyOneOffCounter<std::uint64_t>(Links, symbol);
                return counter.Count()();
            }
        }

        private: bool AllUsagesCore1(std::uint64_t link, HashSet<std::uint64_t> usages, Func<IList<LinkIndex>, LinkIndex> outerHandler)
        {
            auto handler = [&]() -> bool {
                if (usages.Add(doublet))
                {
                    if (this->outerHandler(LinkAddress<LinkIndex>(doublet)) != Constants.Continue)
                    {
                        return false;
                    };
                    if (!this->AllUsagesCore1(doublet, usages, outerHandler))
                    {
                        return false;
                    }
                }
                return true;
            }
            return Links.Unsync.Each(link, Constants.Any, handler)
                && Links.Unsync.Each(Constants.Any, link, handler);
        }

        public: void CalculateAllUsages(std::uint64_t totals[])
        {
            auto calculator = this->AllUsagesCalculator(Links, totals);
            calculator.Calculate();
        }

        public: void CalculateAllUsages2(std::uint64_t totals[])
        {
            auto calculator = this->AllUsagesCalculator2(Links, totals);
            calculator.Calculate();
        }

        class AllUsagesCalculator
        {
            private: SynchronizedLinks<std::uint64_t> _links;
            private: std::uint64_t _totals[N];

            public: AllUsagesCalculator(SynchronizedLinks<std::uint64_t> links, std::uint64_t totals[])
            {
                _links = links;
                _totals = totals;
            }

            public: void Calculate() { _links.Each(_links.Constants.Any, _links.Constants.Any, CalculateCore); }

            private: bool CalculateCore(std::uint64_t link)
            {
                if (_totals[link] == 0)
                {
                    auto total = 1UL;
                    _totals[link] = total;
                    auto visitedChildren = HashSet<std::uint64_t>();
                    bool this->linkCalculator(std::uint64_t child)
                    {
                        if (link != child && visitedChildren.Add(child))
                        {
                            total += _totals[child] == 0 ? 1 : _totals[child];
                        }
                        return true;
                    }
                    _links.Unsync.Each(link, _links.Constants.Any, linkCalculator);
                    _links.Unsync.Each(_links.Constants.Any, link, linkCalculator);
                    _totals[link] = total;
                }
                return true;
            }
        }

        class AllUsagesCalculator2
        {
            private: SynchronizedLinks<std::uint64_t> _links;
            private: std::uint64_t _totals[N];

            public: AllUsagesCalculator2(SynchronizedLinks<std::uint64_t> links, std::uint64_t totals[])
            {
                _links = links;
                _totals = totals;
            }

            public: void Calculate() { _links.Each(_links.Constants.Any, _links.Constants.Any, CalculateCore); }

            private: bool IsElement(std::uint64_t link)
            {
                return _links.Unsync.GetTarget(link) == link || _links.Unsync.GetSource(link) == link;
            }

            private: bool CalculateCore(std::uint64_t link)
            {
                Func<std::uint64_t, std::uint64_t> getSource = _links.Unsync.GetSource;
                Func<std::uint64_t, std::uint64_t> getTarget = _links.Unsync.GetTarget;
                Func<std::uint64_t, bool> isElement = IsElement;
                auto visitLeaf = [&]() -> void {
                    if (link != parent)
                    {
                        _totals[parent]++;
                    };
                }
                void this->visitNode(std::uint64_t parent)
                {
                    if (link != parent)
                    {
                        _totals[parent]++;
                    }
                }
                auto stack = this->Stack();
                auto element = link;
                if (this->isElement(element))
                {
                    this->visitLeaf(element);
                }
                else
                {
                    while (true)
                    {
                        if (this->isElement(element))
                        {
                            if (stack.Count() == 0)
                            {
                                break;
                            }
                            element = stack.Pop();
                            auto source = this->getSource(element);
                            auto target = this->getTarget(element);
                            if (this->isElement(target))
                            {
                                this->visitLeaf(target);
                            }
                            if (this->isElement(source))
                            {
                                this->visitLeaf(source);
                            }
                            element = source;
                        }
                        else
                        {
                            stack.Push(element);
                            this->visitNode(element);
                            element = this->getTarget(element);
                        }
                    }
                }
                _totals[link]++;
                return true;
            }
        }

        class AllUsagesCollector
        {
            private: ILinks<std::uint64_t> *_links;
            private: HashSet<std::uint64_t> _usages;

            public: AllUsagesCollector(ILinks<std::uint64_t> &links, HashSet<std::uint64_t> usages)
            {
                _links = links;
                _usages = usages;
            }

            public: bool Collect(std::uint64_t link)
            {
                if (_usages.Add(link))
                {
                    _links.Each(link, _links.Constants.Any, Collect);
                    _links.Each(_links.Constants.Any, link, Collect);
                }
                return true;
            }
        }

        class AllUsagesCollector1
        {
            private: ILinks<std::uint64_t> *_links;
            private: HashSet<std::uint64_t> _usages;
            private: std::uint64_t _continue = 0;

            public: AllUsagesCollector1(ILinks<std::uint64_t> &links, HashSet<std::uint64_t> usages)
            {
                _links = links;
                _usages = usages;
                _continue = _links.Constants.Continue;
            }

            public: std::uint64_t Collect(IList<std::uint64_t> &link)
            {
                auto linkIndex = _links.GetIndex(link);
                if (_usages.Add(linkIndex))
                {
                    _links.Each(Collect, _links.Constants.Any, linkIndex);
                }
                return _continue;
            }
        }

        class AllUsagesCollector2
        {
            private: ILinks<std::uint64_t> *_links;
            private: BitString _usages = 0;

            public: AllUsagesCollector2(ILinks<std::uint64_t> &links, BitString usages)
            {
                _links = links;
                _usages = usages;
            }

            public: bool Collect(std::uint64_t link)
            {
                if (_usages.Add((std::int64_t)link))
                {
                    _links.Each(link, _links.Constants.Any, Collect);
                    _links.Each(_links.Constants.Any, link, Collect);
                }
                return true;
            }
        }

        class AllUsagesIntersectingCollector
        {
            private: SynchronizedLinks<std::uint64_t> _links;
            private: HashSet<std::uint64_t> _intersectWith;
            private: HashSet<std::uint64_t> _usages;
            private: HashSet<std::uint64_t> _enter;

            public: AllUsagesIntersectingCollector(SynchronizedLinks<std::uint64_t> links, HashSet<std::uint64_t> intersectWith, HashSet<std::uint64_t> usages)
            {
                _links = links;
                _intersectWith = intersectWith;
                _usages = usages;
                _enter = HashSet<std::uint64_t>();
            }

            public: bool Collect(std::uint64_t link)
            {
                if (_enter.Add(link))
                {
                    if (_intersectWith.Contains(link))
                    {
                        _usages.Add(link);
                    }
                    _links.Unsync.Each(link, _links.Constants.Any, Collect);
                    _links.Unsync.Each(_links.Constants.Any, link, Collect);
                }
                return true;
            }
        }

        private: void CloseInnerConnections(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t right)
        {
            this->TryStepLeftUp(handler, left, right);
            this->TryStepRightUp(handler, right, left);
        }

        private: void AllCloseConnections(Action<IList<LinkIndex>> handler, std::uint64_t left, std::uint64_t right)
        {
            if (left == right)
            {
                this->handler(LinkAddress<LinkIndex>(left));
            }
            auto doublet = Links.Unsync.SearchOrDefault(left, right);
            if (doublet != Constants.Null)
            {
                this->handler(LinkAddress<LinkIndex>(doublet));
            }
            this->CloseInnerConnections(handler, left, right);
            this->StepLeft(handler, left, right);
            this->StepRight(handler, left, right);
            this->PartialStepRight(handler, left, right);
            this->PartialStepLeft(handler, left, right);
        }

        private: HashSet<std::uint64_t> GetAllPartiallyMatchingSequencesCore(std::uint64_t sequence[], HashSet<std::uint64_t> previousMatchings, std::int64_t startAt)
        {
            if (startAt >= sequence.Length)
            {
                return previousMatchings;
            }
            auto secondLinkUsages = HashSet<std::uint64_t>();
            AllUsagesCore(sequence[startAt], secondLinkUsages);
            secondLinkUsages.Add(sequence[startAt]);
            auto matchings = HashSet<std::uint64_t>();
            auto filler = SetFiller<LinkIndex, LinkIndex>(matchings, Constants.Continue);
            foreach (auto secondLinkUsage in secondLinkUsages)
            {
                foreach (auto previousMatching in previousMatchings)
                {
                    StepRight(filler.AddFirstAndReturnConstant, previousMatching, secondLinkUsage);
                    TryStepRightUp(filler.AddFirstAndReturnConstant, secondLinkUsage, previousMatching);
                    PartialStepRight(filler.AddFirstAndReturnConstant, previousMatching, secondLinkUsage);
                }
            }
            if (matchings.Count() == 0)
            {
                return matchings;
            }
            return GetAllPartiallyMatchingSequencesCore(sequence, matchings, startAt + 1);
        }

        private: static void EnsureEachLinkIsAnyOrZeroOrManyOrExists(SynchronizedLinks<std::uint64_t> links, params std::uint64_t sequence[])
        {
            if (sequence == nullptr)
            {
                return;
            }
            for (auto i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] != links.Constants.Any && sequence[i] != ZeroOrMany && !links.Exists(sequence[i]))
                {
                    throw ArgumentLinkDoesNotExistsException<std::uint64_t>(sequence[i], std::string("patternSequence[").append(Platform::Converters::To<std::string>(i)).append(1, ']'));
                }
            }
        }

        public: HashSet<std::uint64_t> MatchPattern(params std::uint64_t patternSequence[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                patternSequence = Simplify(patternSequence);
                if (patternSequence.Length > 0)
                {
                    EnsureEachLinkIsAnyOrZeroOrManyOrExists(Links, patternSequence);
                    auto uniqueSequenceElements = HashSet<std::uint64_t>();
                    for (auto i = 0; i < patternSequence.Length; i++)
                    {
                        if (patternSequence[i] != Constants.Any && patternSequence[i] != ZeroOrMany)
                        {
                            uniqueSequenceElements.Add(patternSequence[i]);
                        }
                    }
                    auto results = HashSet<std::uint64_t>();
                    foreach (auto uniqueSequenceElement in uniqueSequenceElements)
                    {
                        AllUsagesCore(uniqueSequenceElement, results);
                    }
                    auto filteredResults = HashSet<std::uint64_t>();
                    auto matcher = PatternMatcher(this, patternSequence, filteredResults);
                    matcher.AddAllPatternMatchedToResults(results);
                    return filteredResults;
                }
                return HashSet<std::uint64_t>();
            });
        }

        public: HashSet<std::uint64_t> GetAllConnections(params std::uint64_t linksToConnect[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto results = HashSet<std::uint64_t>();
                if (linksToConnect.Length > 0)
                {
                    Links.EnsureLinkExists(linksToConnect);
                    AllUsagesCore(linksToConnect[0], results);
                    for (auto i = 1; i < linksToConnect.Length; i++)
                    {
                        auto next = HashSet<std::uint64_t>();
                        AllUsagesCore(linksToConnect[i], next);
                        results.IntersectWith(next);
                    }
                }
                return results;
            });
        }

        public: HashSet<std::uint64_t> GetAllConnections1(params std::uint64_t linksToConnect[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto results = HashSet<std::uint64_t>();
                if (linksToConnect.Length > 0)
                {
                    Links.EnsureLinkExists(linksToConnect);
                    auto collector1 = AllUsagesCollector(Links.Unsync, results);
                    collector1.Collect(linksToConnect[0]);
                    auto next = HashSet<std::uint64_t>();
                    for (auto i = 1; i < linksToConnect.Length; i++)
                    {
                        auto collector = AllUsagesCollector(Links.Unsync, next);
                        collector.Collect(linksToConnect[i]);
                        results.IntersectWith(next);
                        next.Clear();
                    }
                }
                return results;
            });
        }

        public: HashSet<std::uint64_t> GetAllConnections2(params std::uint64_t linksToConnect[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto results = HashSet<std::uint64_t>();
                if (linksToConnect.Length > 0)
                {
                    Links.EnsureLinkExists(linksToConnect);
                    auto collector1 = AllUsagesCollector(Links, results);
                    collector1.Collect(linksToConnect[0]);
                    for (auto i = 1; i < linksToConnect.Length; i++)
                    {
                        auto next = HashSet<std::uint64_t>();
                        auto collector = AllUsagesIntersectingCollector(Links, results, next);
                        collector.Collect(linksToConnect[i]);
                        results = next;
                    }
                }
                return results;
            });
        }

        public: List<std::uint64_t> GetAllConnections3(params std::uint64_t linksToConnect[])
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto results = BitString((std::int64_t)Links.Unsync.Count()() + 1);
                if (linksToConnect.Length > 0)
                {
                    Links.EnsureLinkExists(linksToConnect);
                    auto collector1 = AllUsagesCollector2(Links.Unsync, results);
                    collector1.Collect(linksToConnect[0]);
                    for (auto i = 1; i < linksToConnect.Length; i++)
                    {
                        auto next = BitString((std::int64_t)Links.Unsync.Count()() + 1);
                        auto collector = AllUsagesCollector2(Links.Unsync, next);
                        collector.Collect(linksToConnect[i]);
                        results = results.And(next);
                    }
                }
                return results.GetSetUInt64Indices();
            });
        }

        private: static std::uint64_t Simplify[](std::uint64_t sequence[])
        {
            std::int64_t newLength = 0;
            auto zeroOrManyStepped = false;
            for (auto i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] == ZeroOrMany)
                {
                    if (zeroOrManyStepped)
                    {
                        continue;
                    }
                    zeroOrManyStepped = true;
                }
                else
                {
                    zeroOrManyStepped = false;
                }
                newLength++;
            }
            zeroOrManyStepped = false;
            auto newSequence = std::uint64_t[newLength];
            std::int64_t j = 0;
            for (auto i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] == ZeroOrMany)
                {
                    if (zeroOrManyStepped)
                    {
                        continue;
                    }
                    zeroOrManyStepped = true;
                }
                else
                {
                    zeroOrManyStepped = false;
                }
                newSequence[j++] = sequence[i];
            }
            return newSequence;
        }

        public: static void TestSimplify()
        {
            auto sequence = std::uint64_t[] { ZeroOrMany, ZeroOrMany, 2, 3, 4, ZeroOrMany, ZeroOrMany, ZeroOrMany, 4, ZeroOrMany, ZeroOrMany, ZeroOrMany };
            auto simplifiedSequence = Simplify(sequence);
        }

        public: List<std::uint64_t> GetSimilarSequences() { return List<std::uint64_t>(); }

        public: void Prediction()
        {
        }

        public: List<std::uint64_t> CollectMatchingSequences(std::uint64_t links[])
        {
            if (links.Length == 1)
            {
                throw std::runtime_error("Подпоследовательности с одним элементом не поддерживаются.");
            }
            auto leftBound = 0;
            auto rightBound = links.Length - 1;
            auto left = links[leftBound++];
            auto right = links[rightBound--];
            auto results = List<std::uint64_t>();
            CollectMatchingSequences(left, leftBound, links, right, rightBound, ref results);
            return results;
        }

        private: void CollectMatchingSequences(std::uint64_t leftLink, std::int32_t leftBound, std::uint64_t middleLinks[], std::uint64_t rightLink, std::int32_t rightBound, ref List<std::uint64_t> results)
        {
            auto leftLinkTotalReferers = Links.Unsync.Count()(leftLink);
            auto rightLinkTotalReferers = Links.Unsync.Count()(rightLink);
            if (leftLinkTotalReferers <= rightLinkTotalReferers)
            {
                auto nextLeftLink = middleLinks[leftBound];
                auto elements = this->GetRightElements(leftLink, nextLeftLink);
                if (leftBound <= rightBound)
                {
                    for (auto i = elements.Length - 1; i >= 0; i--)
                    {
                        auto element = elements[i];
                        if (element != 0)
                        {
                            this->CollectMatchingSequences(element, leftBound + 1, middleLinks, rightLink, rightBound, ref results);
                        }
                    }
                }
                else
                {
                    for (auto i = elements.Length - 1; i >= 0; i--)
                    {
                        auto element = elements[i];
                        if (element != 0)
                        {
                            results.insert(element);
                        }
                    }
                }
            }
            else
            {
                auto nextRightLink = middleLinks[rightBound];
                auto elements = this->GetLeftElements(rightLink, nextRightLink);
                if (leftBound <= rightBound)
                {
                    for (auto i = elements.Length - 1; i >= 0; i--)
                    {
                        auto element = elements[i];
                        if (element != 0)
                        {
                            this->CollectMatchingSequences(leftLink, leftBound, middleLinks, elements[i], rightBound - 1, ref results);
                        }
                    }
                }
                else
                {
                    for (auto i = elements.Length - 1; i >= 0; i--)
                    {
                        auto element = elements[i];
                        if (element != 0)
                        {
                            results.insert(element);
                        }
                    }
                }
            }
        }

        public: std::uint64_t GetRightElements[](std::uint64_t startLink, std::uint64_t rightLink)
        {
            auto result = std::uint64_t[5];
            TryStepRight(startLink, rightLink, result, 0);
            Links.Each(Constants.Any, startLink, couple =>
            {
                if (couple != startLink)
                {
                    if (TryStepRight(couple, rightLink, result, 2))
                    {
                        return false;
                    }
                }
                return true;
            });
            if (Links.GetTarget(Links.GetTarget(startLink)) == rightLink)
            {
                result[4] = startLink;
            }
            return result;
        }

        public: bool TryStepRight(std::uint64_t startLink, std::uint64_t rightLink, std::uint64_t result[], std::int32_t offset)
        {
            auto added = 0;
            Links.Each(startLink, Constants.Any, couple =>
            {
                if (couple != startLink)
                {
                    auto coupleTarget = Links.GetTarget(couple);
                    if (coupleTarget == rightLink)
                    {
                        result[offset] = couple;
                        if (++added == 2)
                        {
                            return false;
                        }
                    }
                    else if (Links.GetSource(coupleTarget) == rightLink)
                    {
                        result[offset + 1] = couple;
                        if (++added == 2)
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
            return added > 0;
        }

        public: std::uint64_t GetLeftElements[](std::uint64_t startLink, std::uint64_t leftLink)
        {
            auto result = std::uint64_t[5];
            TryStepLeft(startLink, leftLink, result, 0);
            Links.Each(startLink, Constants.Any, couple =>
            {
                if (couple != startLink)
                {
                    if (TryStepLeft(couple, leftLink, result, 2))
                    {
                        return false;
                    }
                }
                return true;
            });
            if (Links.GetSource(Links.GetSource(leftLink)) == startLink)
            {
                result[4] = leftLink;
            }
            return result;
        }

        public: bool TryStepLeft(std::uint64_t startLink, std::uint64_t leftLink, std::uint64_t result[], std::int32_t offset)
        {
            auto added = 0;
            Links.Each(Constants.Any, startLink, couple =>
            {
                if (couple != startLink)
                {
                    auto coupleSource = Links.GetSource(couple);
                    if (coupleSource == leftLink)
                    {
                        result[offset] = couple;
                        if (++added == 2)
                        {
                            return false;
                        }
                    }
                    else if (Links.GetTarget(coupleSource) == leftLink)
                    {
                        result[offset + 1] = couple;
                        if (++added == 2)
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
            return added > 0;
        }

        class PatternMatcher : public RightSequenceWalker<std::uint64_t>
        {
            private: Sequences _sequences = 0;
            private: std::uint64_t _patternSequence[N];
            private: HashSet<LinkIndex> _linksInSequence;
            private: HashSet<LinkIndex> _results;

            enum PatternBlockType
            {
                Undefined,
                Gap,
                Elements
            }

            struct PatternBlock
            {
                public: PatternBlockType Type = 0;
                public: std::int64_t Start = 0;
                public: std::int64_t Stop = 0;
            };

            private: List<PatternBlock> _pattern;
            private: std::int32_t _patternPosition = 0;
            private: std::int64_t _sequencePosition = 0;

            public: PatternMatcher(Sequences sequences, LinkIndex patternSequence[], HashSet<LinkIndex> results)
                : base(sequences.Links.Unsync, DefaultStack<std::uint64_t>())
            {
                _sequences = sequences;
                _patternSequence = patternSequence;
                _linksInSequence = HashSet<LinkIndex>(patternSequence.Where(x => x != _sequences.Constants.Any && x != ZeroOrMany));
                _results = results;
                _pattern = CreateDetailedPattern();
            }

            protected: bool IsElement(std::uint64_t link) override { return _linksInSequence.Contains(link) || base.IsElement(link); }

            public: bool PatternMatch(LinkIndex sequenceToMatch)
            {
                _patternPosition = 0;
                _sequencePosition = 0;
                foreach (auto part in this->Walk(sequenceToMatch))
                {
                    if (!this->PatternMatchCore(part))
                    {
                        break;
                    }
                }
                return _patternPosition == _pattern.Count() || (_patternPosition == _pattern.Count() - 1 && _pattern[_patternPosition].Start == 0);
            }

            private: List<PatternBlock> CreateDetailedPattern()
            {
                auto pattern = List<PatternBlock>();
                auto patternBlock = PatternBlock();
                for (auto i = 0; i < _patternSequence.Length; i++)
                {
                    if (patternBlock.Type == PatternBlockType.Undefined)
                    {
                        if (_patternSequence[i] == _sequences.Constants.Any)
                        {
                            patternBlock.Type = PatternBlockType.Gap;
                            patternBlock.Start = 1;
                            patternBlock.Stop = 1;
                        }
                        else if (_patternSequence[i] == ZeroOrMany)
                        {
                            patternBlock.Type = PatternBlockType.Gap;
                            patternBlock.Start = 0;
                            patternBlock.Stop = std::numeric_limits<std::int64_t>::max();
                        }
                        else
                        {
                            patternBlock.Type = PatternBlockType.Elements;
                            patternBlock.Start = i;
                            patternBlock.Stop = i;
                        }
                    }
                    else if (patternBlock.Type == PatternBlockType.Elements)
                    {
                        if (_patternSequence[i] == _sequences.Constants.Any)
                        {
                            pattern.Add(patternBlock);
                            patternBlock = PatternBlock
                            {
                                Type = PatternBlockType.Gap,
                                Start = 1,
                                Stop = 1
                            };
                        }
                        else if (_patternSequence[i] == ZeroOrMany)
                        {
                            pattern.Add(patternBlock);
                            patternBlock = PatternBlock
                            {
                                Type = PatternBlockType.Gap,
                                Start = 0,
                                Stop = std::numeric_limits<std::int64_t>::max()
                            };
                        }
                        else
                        {
                            patternBlock.Stop = i;
                        }
                    }
                    else
                    {
                        if (_patternSequence[i] == _sequences.Constants.Any)
                        {
                            patternBlock.Start++;
                            if (patternBlock.Stop < patternBlock.Start)
                            {
                                patternBlock.Stop = patternBlock.Start;
                            }
                        }
                        else if (_patternSequence[i] == ZeroOrMany)
                        {
                            patternBlock.Stop = std::numeric_limits<std::int64_t>::max();
                        }
                        else
                        {
                            pattern.Add(patternBlock);
                            patternBlock = PatternBlock
                            {
                                Type = PatternBlockType.Elements,
                                Start = i,
                                Stop = i
                            };
                        }
                    }
                }
                if (patternBlock.Type != PatternBlockType.Undefined)
                {
                    pattern.Add(patternBlock);
                }
                return pattern;
            }

            private: bool PatternMatchCore(LinkIndex element)
            {
                if (_patternPosition >= _pattern.Count())
                {
                    _patternPosition = -2;
                    return false;
                }
                auto currentPatternBlock = _pattern[_patternPosition];
                if (currentPatternBlock.Type == PatternBlockType.Gap)
                {
                    if (_sequencePosition < currentPatternBlock.Start)
                    {
                        _sequencePosition++;
                        return true;
                    }
                    if (_pattern.Count() == _patternPosition + 1)
                    {
                        _patternPosition++;
                        _sequencePosition = 0;
                        return false;
                    }
                    else
                    {
                        if (_sequencePosition > currentPatternBlock.Stop)
                        {
                            return false;
                        }
                        auto nextPatternBlock = _pattern[_patternPosition + 1];
                        if (_patternSequence[nextPatternBlock.Start] == element)
                        {
                            if (nextPatternBlock.Start < nextPatternBlock.Stop)
                            {
                                _patternPosition++;
                                _sequencePosition = 1;
                            }
                            else
                            {
                                _patternPosition += 2;
                                _sequencePosition = 0;
                            }
                        }
                    }
                }
                else
                {
                    auto patternElementPosition = currentPatternBlock.Start + _sequencePosition;
                    if (_patternSequence[patternElementPosition] != element)
                    {
                        return false;
                    }
                    if (patternElementPosition == currentPatternBlock.Stop)
                    {
                        _patternPosition++;
                        _sequencePosition = 0;
                    }
                    else
                    {
                        _sequencePosition++;
                    }
                }
                return true;
            }

            public: void AddAllPatternMatchedToResults(IEnumerable<std::uint64_t> &sequencesToMatch)
            {
                foreach (auto sequenceToMatch in sequencesToMatch)
                {
                    if (this->PatternMatch(sequenceToMatch))
                    {
                        _results.insert(sequenceToMatch);
                    }
                }
            }
        }
    };
}
