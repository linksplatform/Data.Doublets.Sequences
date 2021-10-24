namespace Platform::Data::Doublets::Unicode
{
    class UnicodeMap
    {
        public: inline static const std::uint64_t FirstCharLink = 1;
        public: inline static const std::uint64_t LastCharLink = FirstCharLink + char.MaxValue;
        public: inline static const std::uint64_t MapSize = 1 + char.MaxValue;

        private: ILinks<std::uint64_t> *_links;
        private: bool _initialized = 0;

        public: UnicodeMap(ILinks<std::uint64_t> &links) { _links = links; }

        public: static UnicodeMap InitNew(ILinks<std::uint64_t> &links)
        {
            auto map = UnicodeMap(links);
            map.Init();
            return map;
        }

        public: void Init()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            auto firstLink = _links.CreatePoint();
            if (firstLink != FirstCharLink)
            {
                _links.Delete(firstLink);
            }
            else
            {
                for (auto i = FirstCharLink + 1; i <= LastCharLink; i++)
                {
                    auto createdLink = _links.CreatePoint();
                    _links.Update(createdLink, firstLink, createdLink);
                    if (createdLink != i)
                    {
                        throw std::runtime_error("Unable to initialize UTF 16 table.");
                    }
                }
            }
        }

        public: static std::uint64_t FromCharToLink(char character) { return (std::uint64_t)character + 1; }

        public: static char FromLinkToChar(std::uint64_t link) { return (char)(link - 1); }

        public: static bool IsCharLink(std::uint64_t link) { return link <= MapSize; }

        public: static std::string FromLinksToString(IList<std::uint64_t> &linksList)
        {
            std::string sb;
            for (std::int32_t i = 0; i < linksList.Count(); i++)
            {
                sb.append(Platform::Converters::To<std::string>(FromLinkToChar(linksList[i])));
            }
            return sb;
        }

        public: static std::string FromSequenceLinkToString(std::uint64_t link, ILinks<std::uint64_t> &links)
        {
            std::string sb;
            if (links.Exists(link))
            {
                StopableSequenceWalker.WalkRight(link, links.GetSource, links.GetTarget,
                    x => x <= MapSize || links.GetSource(x) == x || links.GetTarget(x) == x, element =>
                    {
                        sb.append(Platform::Converters::To<std::string>(FromLinkToChar(element)));
                        return true;
                    });
            }
            return sb;
        }

        public: static std::uint64_t FromCharsToLinkArray[](char chars[]) { return FromCharsToLinkArray(chars, chars.Length); }

        public: static std::uint64_t FromCharsToLinkArray[](char chars[], std::int32_t count)
        {
            auto linksSequence = std::uint64_t[count];
            for (auto i = 0; i < count; i++)
            {
                linksSequence[i] = FromCharToLink(chars[i]);
            }
            return linksSequence;
        }

        public: static std::uint64_t FromStringToLinkArray[](std::string sequence)
        {
            auto linksSequence = std::uint64_t[sequence.Length];
            for (auto i = 0; i < sequence.Length; i++)
            {
                linksSequence[i] = FromCharToLink(sequence[i]);
            }
            return linksSequence;
        }

        public: static List<std::uint64_t[]> FromStringToLinkArrayGroups(std::string sequence)
        {
            auto result = List<std::uint64_t[]>();
            auto offset = 0;
            while (offset < sequence.Length)
            {
                auto currentCategory = CharUnicodeInfo.GetUnicodeCategory(sequence[offset]);
                auto relativeLength = 1;
                auto absoluteLength = offset + relativeLength;
                while (absoluteLength < sequence.Length &&
                       currentCategory == CharUnicodeInfo.GetUnicodeCategory(sequence[absoluteLength]))
                {
                    relativeLength++;
                    absoluteLength++;
                }
                auto innerSequence = std::uint64_t[relativeLength];
                auto maxLength = offset + relativeLength;
                for (auto i = offset; i < maxLength; i++)
                {
                    innerSequence[i - offset] = FromCharToLink(sequence[i]);
                }
                result.Add(innerSequence);
                offset += relativeLength;
            }
            return result;
        }

        public: static List<std::uint64_t[]> FromLinkArrayToLinkArrayGroups(std::uint64_t array[])
        {
            auto result = List<std::uint64_t[]>();
            auto offset = 0;
            while (offset < array.Length)
            {
                auto relativeLength = 1;
                if (array[offset] <= LastCharLink)
                {
                    auto currentCategory = CharUnicodeInfo.GetUnicodeCategory(FromLinkToChar(array[offset]));
                    auto absoluteLength = offset + relativeLength;
                    while (absoluteLength < array.Length &&
                           array[absoluteLength] <= LastCharLink &&
                           currentCategory == CharUnicodeInfo.GetUnicodeCategory(FromLinkToChar(array[absoluteLength])))
                    {
                        relativeLength++;
                        absoluteLength++;
                    }
                }
                else
                {
                    auto absoluteLength = offset + relativeLength;
                    while (absoluteLength < array.Length && array[absoluteLength] > LastCharLink)
                    {
                        relativeLength++;
                        absoluteLength++;
                    }
                }
                auto innerSequence = std::uint64_t[relativeLength];
                auto maxLength = offset + relativeLength;
                for (auto i = offset; i < maxLength; i++)
                {
                    innerSequence[i - offset] = array[i];
                }
                result.Add(innerSequence);
                offset += relativeLength;
            }
            return result;
        }
    };
}
