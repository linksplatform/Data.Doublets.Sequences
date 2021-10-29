namespace Platform::Data::Doublets::Sequences::Tests
{
    class TempLinksTestScope : public DisposableBase
    {
        public: const ILinks<std::uint64_t> *MemoryAdapter;
        public: const SynchronizedLinks<std::uint64_t> Links;
        public: const Sequences Sequences;
        public: const std::string TempFilename;
        public: const std::string TempTransactionLogFilename;
        private: bool _deleteFiles = 0;

        public: TempLinksTestScope(bool deleteFiles = true, bool useSequences = false, bool useLog = false) : this(SequencesOptions<std::uint64_t>(), deleteFiles, useSequences, useLog) { }

        public: TempLinksTestScope(SequencesOptions<std::uint64_t> sequencesOptions, bool deleteFiles = true, bool useSequences = false, bool useLog = false)
        {
            _deleteFiles = deleteFiles;
            TempFilename = Path.GetTempFileName();
            TempTransactionLogFilename = Path.GetTempFileName();
            auto coreMemoryAdapter = this->UInt64SplitMemoryLinks(this->FileMappedResizableDirectMemory(TempFilename), this->FileMappedResizableDirectMemory(Path.ChangeExtension(TempFilename, "indexes")), UInt64SplitMemoryLinks.DefaultLinksSizeStep, LinksConstants<std::uint64_t>(), Memory.IndexTreeType.Default, useLinkedList: true);
            MemoryAdapter = useLog ? (ILinks<std::uint64_t>)this->UInt64LinksTransactionsLayer(coreMemoryAdapter, TempTransactionLogFilename) : coreMemoryAdapter;
            Links = SynchronizedLinks<std::uint64_t>(this->UInt64Links(MemoryAdapter));
            if (useSequences)
            {
                Sequences = this->Sequences(Links, sequencesOptions);
            }
        }

        protected: void Dispose(bool manual, bool wasDisposed) override
        {
            if (!wasDisposed)
            {
                Links.Unsync.DisposeIfPossible();
                if (_deleteFiles)
                {
                    this->DeleteFiles();
                }
            }
        }

        public: void DeleteFiles()
        {
            File.Delete(TempFilename);
            File.Delete(TempTransactionLogFilename);
        }
    };
}
