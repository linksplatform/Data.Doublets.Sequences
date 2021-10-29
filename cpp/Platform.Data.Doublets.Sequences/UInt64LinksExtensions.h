namespace Platform::Data::Doublets
{
    class UInt64LinksExtensions
    {
        public: static void UseUnicode(ILinks<std::uint64_t> &links) { UnicodeMap.InitNew(links); }
    };
}
