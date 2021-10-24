namespace Platform::Data::Doublets::Sequences::Walkers
{
    template <typename ...> class LeveledSequenceWalker;
    template <typename TLink> class LeveledSequenceWalker<TLink> : public LinksOperatorBase<TLink>, ISequenceWalker<TLink>
    {
        private: readonly Func<TLink, bool> _isElement;

        public: LeveledSequenceWalker(ILinks<TLink> &links, Func<TLink, bool> isElement) : base(links) { return _isElement = isElement; }

        public: LeveledSequenceWalker(ILinks<TLink> &links) : base(links) { return _isElement = _links.IsPartialPoint; }

        public: IEnumerable<TLink> Walk(TLink sequence) { return ToArray(sequence); }

        public: TLink ToArray[](TLink sequence)
        {
            auto length = 1;
            TLink array[length] = { {0} };
            array[0] = sequence;
            if (_isElement(sequence))
            {
                return array;
            }
            bool hasElements = 0;
            do
            {
                length *= 2;
#if USEARRAYPOOL
                auto nextArray = ArrayPool.Allocate<std::uint64_t>(length);
#else
                TLink nextArray[length] = { {0} };
#endif
                hasElements = false;
                for (auto i = 0; i < array.Length; i++)
                {
                    auto candidate = array[i];
                    if (array[i] == 0)
                    {
                        continue;
                    }
                    auto doubletOffset = i * 2;
                    if (_isElement(candidate))
                    {
                        nextArray[doubletOffset] = candidate;
                    }
                    else
                    {
                        auto links = _links;
                        auto link = links.GetLink(candidate);
                        auto linkSource = links.GetSource(link);
                        auto linkTarget = links.GetTarget(link);
                        nextArray[doubletOffset] = linkSource;
                        nextArray[doubletOffset + 1] = linkTarget;
                        if (!hasElements)
                        {
                            hasElements = !(_isElement(linkSource) && _isElement(linkTarget));
                        }
                    }
                }
#if USEARRAYPOOL
                if (array.Length > 1)
                {
                    ArrayPool.Free(array);
                }
#endif
                array = nextArray;
            }
            while hasElements;
            auto filledElementsCount = CountFilledElements(array);
            if (filledElementsCount == array.Length)
            {
                return array;
            }
            else
            {
                return CopyFilledElements(array, filledElementsCount);
            }
        }

        private: static TLink CopyFilledElements[](TLink array[], std::int32_t filledElementsCount)
        {
            TLink finalArray[filledElementsCount] = { {0} };
            for (std::int32_t i = 0, j = 0; i < array.Length; i++)
            {
                if (!array[i] == 0)
                {
                    finalArray[j] = array[i];
                    j++;
                }
            }
#if USEARRAYPOOL
                ArrayPool.Free(array);
#endif
            return finalArray;
        }

        private: static std::int32_t CountFilledElements(TLink array[])
        {
            auto count = 0;
            for (auto i = 0; i < array.Length; i++)
            {
                if (!array[i] == 0)
                {
                    count++;
                }
            }
            return count;
        }
    };
}
