

using LinkIndex = std::uint64_t;

namespace Platform::Data::Doublets::Sequences
{
    public partial class Sequences : public ILinks<LinkIndex>
    {
        public: inline static const LinkIndex ZeroOrMany = LinkIndex.MaxValue;

        public: const SequencesOptions<LinkIndex> Options;
        public: const SynchronizedLinks<LinkIndex> Links;
        private: ISynchronization _sync = 0;

        public: const LinksConstants<LinkIndex> Constants;

        public: Sequences(SynchronizedLinks<LinkIndex> links, SequencesOptions<LinkIndex> options)
        {
            Links = links;
            _sync = links.SyncRoot;
            Options = options;
            Options.ValidateOptions();
            Options.InitOptions(Links);
            Constants = links.Constants;
        }

        public: Sequences(SynchronizedLinks<LinkIndex> links) : this(links, SequencesOptions<LinkIndex>()) { }

        public: bool IsSequence(LinkIndex sequence)
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (Options.UseSequenceMarker)
                {
                    return Options.MarkedSequenceMatcher.IsMatched(sequence);
                }
                return !Links.Unsync.IsPartialPoint(sequence);
            });
        }

        private: LinkIndex GetSequenceByElements(LinkIndex sequence)
        {
            if (Options.UseSequenceMarker)
            {
                return Links.SearchOrDefault(Options.SequenceMarkerLink, sequence);
            }
            return sequence;
        }

        private: LinkIndex GetSequenceElements(LinkIndex sequence)
        {
            if (Options.UseSequenceMarker)
            {
                auto linkContents = Link<std::uint64_t>(Links.GetLink(sequence));
                if (linkContents.Source == Options.SequenceMarkerLink)
                {
                    return linkContents.Target;
                }
                if (linkContents.Target == Options.SequenceMarkerLink)
                {
                    return linkContents.Source;
                }
            }
            return sequence;
        }

        public: LinkIndex Count(IList<LinkIndex> &restrictions)
        {
            if (restrictions.IsNullOrEmpty())
            {
                return Links.Count()(Constants.Any, Options.SequenceMarkerLink, Constants.Any);
            }
            if (restrictions.Count() == 1)
            {
                auto sequenceIndex = restrictions[0];
                if (sequenceIndex == Constants.Null)
                {
                    return 0;
                }
                if (sequenceIndex == Constants.Any)
                {
                    return this->Count({});
                }
                if (Options.UseSequenceMarker)
                {
                    return Links.Count()(Constants.Any, Options.SequenceMarkerLink, sequenceIndex);
                }
                return Links.Exists(sequenceIndex) ? 1UL : 0;
            }
            throw std::logic_error("Not implemented exception.");
        }

        private: LinkIndex CountUsages(params LinkIndex restrictions[])
        {
            if (restrictions.Length == 0)
            {
                return 0;
            }
            if (restrictions.Length == 1)
            {
                if (restrictions[0] == Constants.Null)
                {
                    return 0;
                }
                auto any = Constants.Any;
                if (Options.UseSequenceMarker)
                {
                    auto elementsLink = this->GetSequenceElements(restrictions[0]);
                    auto sequenceLink = this->GetSequenceByElements(elementsLink);
                    if (sequenceLink != Constants.Null)
                    {
                        return Links.Count()(any, sequenceLink) + Links.Count()(any, elementsLink) - 1;
                    }
                    return Links.Count()(any, elementsLink);
                }
                return Links.Count()(any, restrictions[0]);
            }
            throw std::logic_error("Not implemented exception.");
        }

        public: LinkIndex Create(IList<LinkIndex> &restrictions)
        {
            return _sync.ExecuteWriteOperation(() =>
            {
                if (restrictions.IsNullOrEmpty())
                {
                    return Constants.Null;
                }
                Links.EnsureInnerReferenceExists(restrictions, "restrictions");
                return this->CreateCore(restrictions);
            });
        }

        private: LinkIndex CreateCore(IList<LinkIndex> &restrictions)
        {
            LinkIndex sequence[] = restrictions.SkipFirst();
            if (Options.UseIndex)
            {
                Options.Index.Add(sequence);
            }
            auto sequenceRoot = this->0(LinkIndex);
            if (Options.EnforceSingleSequenceVersionOnWriteBasedOnExisting)
            {
                auto matches = this->Each(restrictions);
                if (matches.Count() > 0)
                {
                    sequenceRoot = matches[0];
                }
            }
            else if (Options.EnforceSingleSequenceVersionOnWriteBasedOnNew)
            {
                return this->CompactCore(sequence);
            }
            if (sequenceRoot == 0)
            {
                sequenceRoot = Options.LinksToSequenceConverter.Convert(sequence);
            }
            if (Options.UseSequenceMarker)
            {
                return Links.Unsync.GetOrCreate(Options.SequenceMarkerLink, sequenceRoot);
            }
            return sequenceRoot;
        }

        public: List<LinkIndex> Each(IList<LinkIndex> &sequence)
        {
            auto results = List<LinkIndex>();
            auto filler = ListFiller<LinkIndex, LinkIndex>(results, Constants.Continue);
            Each(filler.AddFirstAndReturnConstant, sequence);
            return results;
        }

        public: LinkIndex Each(Func<IList<LinkIndex>, LinkIndex> handler, IList<LinkIndex> &restrictions)
        {
            return _sync.ExecuteReadOperation(() =>
            {
                if (restrictions.IsNullOrEmpty())
                {
                    return Constants.Continue;
                }
                Links.EnsureInnerReferenceExists(restrictions, "restrictions");
                if (restrictions.Count() == 1)
                {
                    auto link = restrictions[0];
                    auto any = Constants.Any;
                    if (link == any)
                    {
                        if (Options.UseSequenceMarker)
                        {
                            return Links.Unsync.Each(handler, Link<LinkIndex>(any, Options.SequenceMarkerLink, any));
                        }
                        else
                        {
                            return Links.Unsync.Each(handler, Link<LinkIndex>(any, any, any));
                        }
                    }
                    if (Options.UseSequenceMarker)
                    {
                        auto sequenceLinkValues = Links.Unsync.GetLink(link);
                        if (sequenceLinkValues[Constants.SourcePart] == Options.SequenceMarkerLink)
                        {
                            link = sequenceLinkValues[Constants.TargetPart];
                        }
                    }
                    auto sequence = Options.Walker.Walk(link)->ToArray().ShiftRight();
                    sequence[0] = link;
                    return this->handler(sequence);
                }
                else if (restrictions.Count() == 2)
                {
                    throw std::logic_error("Not implemented exception.");
                }
                else if (restrictions.Count() == 3)
                {
                    return Links.Unsync.Each(handler, restrictions);
                }
                else
                {
                    auto sequence = restrictions.SkipFirst();
                    if (Options.UseIndex && !Options.Index.MightContain(sequence))
                    {
                        return Constants.Break;
                    }
                    return this->EachCore(handler, sequence);
                }
            });
        }

        private: LinkIndex EachCore(Func<IList<LinkIndex>, LinkIndex> handler, IList<LinkIndex> &values)
        {
            auto matcher = this->Matcher(this, values, HashSet<LinkIndex>(), handler);
            Func<IList<LinkIndex>, LinkIndex> innerHandler = Options.UseSequenceMarker ? (Func<IList<LinkIndex>, LinkIndex>)matcher.HandleFullMatchedSequence : matcher.HandleFullMatched;
            if (this->StepRight(innerHandler, values[0], values[1]) != Constants.Continue)
            {
                return Constants.Break;
            }
            auto last = values.Count() - 2;
            for (auto i = 1; i < last; i++)
            {
                if (this->PartialStepRight(innerHandler, values[i], values[i + 1]) != Constants.Continue)
                {
                    return Constants.Break;
                }
            }
            if (values.Count() >= 3)
            {
                if (this->StepLeft(innerHandler, values[values.Count() - 2], values[values.Count() - 1]) != Constants.Continue)
                {
                    return Constants.Break;
                }
            }
            return Constants.Continue;
        }

        private: LinkIndex PartialStepRight(Func<IList<LinkIndex>, LinkIndex> handler, LinkIndex left, LinkIndex right)
        {
            return Links.Unsync.Each(doublet =>
            {
                auto doubletIndex = doublet[Constants.IndexPart];
                if (this->StepRight(handler, doubletIndex, right) != Constants.Continue)
                {
                    return Constants.Break;
                }
                if (left != doubletIndex)
                {
                    return this->PartialStepRight(handler, doubletIndex, right);
                }
                return Constants.Continue;
            }, Link<LinkIndex>(Constants.Any, Constants.Any, left));
        }

        private: LinkIndex StepRight(Func<IList<LinkIndex>, LinkIndex> handler, LinkIndex left, LinkIndex right) { return Links.Unsync.Each(rightStep => this->TryStepRightUp(handler, right, rightStep[Constants.IndexPart]), Link<LinkIndex>(Constants.Any, left, Constants.Any)); }

        private: LinkIndex TryStepRightUp(Func<IList<LinkIndex>, LinkIndex> handler, LinkIndex right, LinkIndex stepFrom)
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
                return this->handler(LinkAddress<LinkIndex>(stepFrom));
            }
            return Constants.Continue;
        }

        private: LinkIndex StepLeft(Func<IList<LinkIndex>, LinkIndex> handler, LinkIndex left, LinkIndex right) { return Links.Unsync.Each(leftStep => this->TryStepLeftUp(handler, left, leftStep[Constants.IndexPart]), Link<LinkIndex>(Constants.Any, Constants.Any, right)); }

        private: LinkIndex TryStepLeftUp(Func<IList<LinkIndex>, LinkIndex> handler, LinkIndex left, LinkIndex stepFrom)
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
                return this->handler(LinkAddress<LinkIndex>(stepFrom));
            }
            return Constants.Continue;
        }

        public: LinkIndex Update(IList<LinkIndex> &restrictions, IList<LinkIndex> &substitution)
        {
            auto sequence = restrictions.SkipFirst();
            auto newSequence = substitution.SkipFirst();
            if (sequence.IsNullOrEmpty() && newSequence.IsNullOrEmpty())
            {
                return Constants.Null;
            }
            if (sequence.IsNullOrEmpty())
            {
                return this->Create(substitution);
            }
            if (newSequence.IsNullOrEmpty())
            {
                this->Delete(restrictions);
                return Constants.Null;
            }
            return _sync.ExecuteWriteOperation((Func<std::uint64_t>)(() =>
            {
                ILinksExtensions.EnsureLinkIsAnyOrExists<std::uint64_t>(Links, (IList<std::uint64_t>)sequence);
                Links.EnsureLinkExists(newSequence);
                return this->UpdateCore(sequence, newSequence);
            }));
        }

        private: LinkIndex UpdateCore(IList<LinkIndex> &sequence, IList<LinkIndex> &newSequence)
        {
            LinkIndex bestVariant = 0;
            if (Options.EnforceSingleSequenceVersionOnWriteBasedOnNew && !sequence.EqualTo(newSequence))
            {
                bestVariant = this->CompactCore(newSequence);
            }
            else
            {
                bestVariant = this->CreateCore(newSequence);
            }
            foreach (auto variant in this->Each(sequence))
            {
                if (variant != bestVariant)
                {
                    this->UpdateOneCore(variant, bestVariant);
                }
            }
            return bestVariant;
        }

        private: void UpdateOneCore(LinkIndex sequence, LinkIndex newSequence)
        {
            if (Options.UseGarbageCollection)
            {
                auto sequenceElements = this->GetSequenceElements(sequence);
                auto sequenceElementsContents = Link<std::uint64_t>(Links.GetLink(sequenceElements));
                auto sequenceLink = this->GetSequenceByElements(sequenceElements);
                auto newSequenceElements = this->GetSequenceElements(newSequence);
                auto newSequenceLink = this->GetSequenceByElements(newSequenceElements);
                if (Options.UseCascadeUpdate || this->CountUsages(sequence) == 0)
                {
                    if (sequenceLink != Constants.Null)
                    {
                        Links.Unsync.MergeAndDelete(sequenceLink, newSequenceLink);
                    }
                    Links.Unsync.MergeAndDelete(sequenceElements, newSequenceElements);
                }
                this->ClearGarbage(sequenceElementsContents.Source);
                this->ClearGarbage(sequenceElementsContents.Target);
            }
            else
            {
                if (Options.UseSequenceMarker)
                {
                    auto sequenceElements = this->GetSequenceElements(sequence);
                    auto sequenceLink = this->GetSequenceByElements(sequenceElements);
                    auto newSequenceElements = this->GetSequenceElements(newSequence);
                    auto newSequenceLink = this->GetSequenceByElements(newSequenceElements);
                    if (Options.UseCascadeUpdate || this->CountUsages(sequence) == 0)
                    {
                        if (sequenceLink != Constants.Null)
                        {
                            Links.Unsync.MergeAndDelete(sequenceLink, newSequenceLink);
                        }
                        Links.Unsync.MergeAndDelete(sequenceElements, newSequenceElements);
                    }
                }
                else
                {
                    if (Options.UseCascadeUpdate || this->CountUsages(sequence) == 0)
                    {
                        Links.Unsync.MergeAndDelete(sequence, newSequence);
                    }
                }
            }
        }

        public: void Delete(IList<LinkIndex> &restrictions)
        {
            _sync.ExecuteWriteOperation(() =>
            {
                auto sequence = restrictions.SkipFirst();
                foreach (auto linkToDelete in this->Each(sequence))
                {
                    this->DeleteOneCore(linkToDelete);
                }
            });
        }

        private: void DeleteOneCore(LinkIndex link)
        {
            if (Options.UseGarbageCollection)
            {
                auto sequenceElements = this->GetSequenceElements(link);
                auto sequenceElementsContents = Link<std::uint64_t>(Links.GetLink(sequenceElements));
                auto sequenceLink = this->GetSequenceByElements(sequenceElements);
                if (Options.UseCascadeDelete || this->CountUsages(link) == 0)
                {
                    if (sequenceLink != Constants.Null)
                    {
                        Links.Unsync.Delete(sequenceLink);
                    }
                    Links.Unsync.Delete(link);
                }
                this->ClearGarbage(sequenceElementsContents.Source);
                this->ClearGarbage(sequenceElementsContents.Target);
            }
            else
            {
                if (Options.UseSequenceMarker)
                {
                    auto sequenceElements = this->GetSequenceElements(link);
                    auto sequenceLink = this->GetSequenceByElements(sequenceElements);
                    if (Options.UseCascadeDelete || this->CountUsages(link) == 0)
                    {
                        if (sequenceLink != Constants.Null)
                        {
                            Links.Unsync.Delete(sequenceLink);
                        }
                        Links.Unsync.Delete(link);
                    }
                }
                else
                {
                    if (Options.UseCascadeDelete || this->CountUsages(link) == 0)
                    {
                        Links.Unsync.Delete(link);
                    }
                }
            }
        }

        public: void CompactAll()
        {
            _sync.ExecuteWriteOperation(() =>
            {
                auto sequences = this->Each((LinkAddress<LinkIndex>)Constants.Any);
                for (std::int32_t i = 0; i < sequences.Count(); i++)
                {
                    auto sequence = this.ToList(sequences[i]);
                    this->Compact(sequence.ShiftRight());
                }
            });
        }

        public: LinkIndex Compact(IList<LinkIndex> &sequence)
        {
            return _sync.ExecuteWriteOperation(() =>
            {
                if (sequence.IsNullOrEmpty())
                {
                    return Constants.Null;
                }
                Links.EnsureInnerReferenceExists(sequence, "sequence");
                return this->CompactCore(sequence);
            });
        }

        private: LinkIndex CompactCore(IList<LinkIndex> &sequence) { return this->UpdateCore(sequence, sequence); }

        private: bool IsGarbage(LinkIndex link) { return link != Options.SequenceMarkerLink && !Links.Unsync.IsPartialPoint(link) && Links.Count()(Constants.Any, link) == 0; }

        private: void ClearGarbage(LinkIndex link)
        {
            if (this->IsGarbage(link))
            {
                auto contents = Link<std::uint64_t>(Links.GetLink(link));
                Links.Unsync.Delete(link);
                this->ClearGarbage(contents.Source);
                this->ClearGarbage(contents.Target);
            }
        }

        public: bool EachPart(Func<LinkIndex, bool> handler, LinkIndex sequence)
        {
            return _sync.ExecuteReadOperation(() =>
            {
                auto links = Links.Unsync;
                foreach (auto part in Options.Walker.Walk(sequence))
                {
                    if (!this->handler(part))
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        class Matcher : public RightSequenceWalker<LinkIndex>
        {
            private: Sequences _sequences = 0;
            private: IList<LinkIndex> *_patternSequence;
            private: HashSet<LinkIndex> _linksInSequence;
            private: HashSet<LinkIndex> _results;
            private: readonly Func<IList<LinkIndex>, LinkIndex> _stopableHandler;
            private: HashSet<LinkIndex> _readAsElements;
            private: std::int32_t _filterPosition = 0;

            public: Matcher(Sequences sequences, IList<LinkIndex> &patternSequence, HashSet<LinkIndex> results, Func<IList<LinkIndex>, LinkIndex> stopableHandler, HashSet<LinkIndex> readAsElements = {})
                : base(sequences.Links.Unsync, DefaultStack<LinkIndex>())
            {
                _sequences = sequences;
                _patternSequence = patternSequence;
                _linksInSequence = HashSet<LinkIndex>(patternSequence.Where(x => x != _links.Constants.Any && x != ZeroOrMany));
                _results = results;
                _stopableHandler = stopableHandler;
                _readAsElements = readAsElements;
            }

            protected: bool IsElement(LinkIndex link) override { return base.IsElement(link) || (_readAsElements != nullptr && _readAsElements.Contains(link)) || _linksInSequence.Contains(link); }

            public: bool FullMatch(LinkIndex sequenceToMatch)
            {
                _filterPosition = 0;
                foreach (auto part in this->Walk(sequenceToMatch))
                {
                    if (!this->FullMatchCore(part))
                    {
                        break;
                    }
                }
                return _filterPosition == _patternSequence.Count();
            }

            private: bool FullMatchCore(LinkIndex element)
            {
                if (_filterPosition == _patternSequence.Count())
                {
                    _filterPosition = -2;
                    return false;
                }
                if (_patternSequence[_filterPosition] != _links.Constants.Any
                 && element != _patternSequence[_filterPosition])
                {
                    _filterPosition = -1;
                    return false;
                }
                _filterPosition++;
                return true;
            }

            public: void AddFullMatchedToResults(IList<LinkIndex> &restrictions)
            {
                auto sequenceToMatch = restrictions[_links.Constants.IndexPart];
                if (this->FullMatch(sequenceToMatch))
                {
                    _results.Add(sequenceToMatch);
                }
            }

            public: LinkIndex HandleFullMatched(IList<LinkIndex> &restrictions)
            {
                auto sequenceToMatch = restrictions[_links.Constants.IndexPart];
                if (this->FullMatch(sequenceToMatch) && _results.Add(sequenceToMatch))
                {
                    return _stopableHandler(LinkAddress<LinkIndex>(sequenceToMatch));
                }
                return _links.Constants.Continue;
            }

            public: LinkIndex HandleFullMatchedSequence(IList<LinkIndex> &restrictions)
            {
                auto sequenceToMatch = restrictions[_links.Constants.IndexPart];
                auto sequence = _sequences.GetSequenceByElements(sequenceToMatch);
                if (sequence != _links.Constants.Null && this->FullMatch(sequenceToMatch) && _results.Add(sequenceToMatch))
                {
                    return _stopableHandler(LinkAddress<LinkIndex>(sequence));
                }
                return _links.Constants.Continue;
            }

            public: bool PartialMatch(LinkIndex sequenceToMatch)
            {
                _filterPosition = -1;
                foreach (auto part in this->Walk(sequenceToMatch))
                {
                    if (!this->PartialMatchCore(part))
                    {
                        break;
                    }
                }
                return _filterPosition == _patternSequence.Count() - 1;
            }

            private: bool PartialMatchCore(LinkIndex element)
            {
                if (_filterPosition == (_patternSequence.Count() - 1))
                {
                    return false;
                }
                if (_filterPosition >= 0)
                {
                    if (element == _patternSequence[_filterPosition + 1])
                    {
                        _filterPosition++;
                    }
                    else
                    {
                        _filterPosition = -1;
                    }
                }
                if (_filterPosition < 0)
                {
                    if (element == _patternSequence[0])
                    {
                        _filterPosition = 0;
                    }
                }
                return true;
            }

            public: void AddPartialMatchedToResults(LinkIndex sequenceToMatch)
            {
                if (this->PartialMatch(sequenceToMatch))
                {
                    _results.Add(sequenceToMatch);
                }
            }

            public: LinkIndex HandlePartialMatched(IList<LinkIndex> &restrictions)
            {
                auto sequenceToMatch = restrictions[_links.Constants.IndexPart];
                if (this->PartialMatch(sequenceToMatch))
                {
                    return _stopableHandler(LinkAddress<LinkIndex>(sequenceToMatch));
                }
                return _links.Constants.Continue;
            }

            public: void AddAllPartialMatchedToResults(IEnumerable<LinkIndex> &sequencesToMatch)
            {
                foreach (auto sequenceToMatch in sequencesToMatch)
                {
                    if (this->PartialMatch(sequenceToMatch))
                    {
                        _results.Add(sequenceToMatch);
                    }
                }
            }

            public: void AddAllPartialMatchedToResultsAndReadAsElements(IEnumerable<LinkIndex> &sequencesToMatch)
            {
                foreach (auto sequenceToMatch in sequencesToMatch)
                {
                    if (this->PartialMatch(sequenceToMatch))
                    {
                        _readAsElements.Add(sequenceToMatch);
                        _results.Add(sequenceToMatch);
                    }
                }
            }
        }
    };
}