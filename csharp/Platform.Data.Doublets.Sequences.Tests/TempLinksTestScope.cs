using System.IO;
using Platform.Disposables;
using Platform.Data.Doublets.Sequences;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Memory.United.Specific;
using Platform.Data.Doublets.Memory.Split.Specific;
using Platform.Memory;

namespace Platform.Data.Doublets.Sequences.Tests
{
    /// <summary>
    /// <para>
    /// Represents the temp links test scope.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="DisposableBase"/>
    public class TempLinksTestScope : DisposableBase
    {
        /// <summary>
        /// <para>
        /// Gets the memory adapter value.
        /// </para>
        /// <para></para>
        /// </summary>
        public ILinks<ulong> MemoryAdapter { get; }
        /// <summary>
        /// <para>
        /// Gets the links value.
        /// </para>
        /// <para></para>
        /// </summary>
        public SynchronizedLinks<ulong> Links { get; }
        /// <summary>
        /// <para>
        /// Gets the sequences value.
        /// </para>
        /// <para></para>
        /// </summary>
        public Sequences Sequences { get; }
        /// <summary>
        /// <para>
        /// Gets the temp filename value.
        /// </para>
        /// <para></para>
        /// </summary>
        public string TempFilename { get; }
        /// <summary>
        /// <para>
        /// Gets the temp transaction log filename value.
        /// </para>
        /// <para></para>
        /// </summary>
        public string TempTransactionLogFilename { get; }
        /// <summary>
        /// <para>
        /// The delete files.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly bool _deleteFiles;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="TempLinksTestScope"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="deleteFiles">
        /// <para>A delete files.</para>
        /// <para></para>
        /// </param>
        /// <param name="useSequences">
        /// <para>A use sequences.</para>
        /// <para></para>
        /// </param>
        /// <param name="useLog">
        /// <para>A use log.</para>
        /// <para></para>
        /// </param>
        public TempLinksTestScope(bool deleteFiles = true, bool useSequences = false, bool useLog = false) : this(new SequencesOptions<ulong>(), deleteFiles, useSequences, useLog) { }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="TempLinksTestScope"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequencesOptions">
        /// <para>A sequences options.</para>
        /// <para></para>
        /// </param>
        /// <param name="deleteFiles">
        /// <para>A delete files.</para>
        /// <para></para>
        /// </param>
        /// <param name="useSequences">
        /// <para>A use sequences.</para>
        /// <para></para>
        /// </param>
        /// <param name="useLog">
        /// <para>A use log.</para>
        /// <para></para>
        /// </param>
        public TempLinksTestScope(SequencesOptions<ulong> sequencesOptions, bool deleteFiles = true, bool useSequences = false, bool useLog = false)
        {
            _deleteFiles = deleteFiles;
            TempFilename = Path.GetTempFileName();
            TempTransactionLogFilename = Path.GetTempFileName();
            //var coreMemoryAdapter = new UInt64UnitedMemoryLinks(TempFilename);
            var coreMemoryAdapter = new UInt64SplitMemoryLinks(new FileMappedResizableDirectMemory(TempFilename), new FileMappedResizableDirectMemory(Path.ChangeExtension(TempFilename, "indexes")), UInt64SplitMemoryLinks.DefaultLinksSizeStep, new LinksConstants<ulong>(), Memory.IndexTreeType.Default, useLinkedList: true);
            MemoryAdapter = useLog ? (ILinks<ulong>)new UInt64LinksTransactionsLayer(coreMemoryAdapter, TempTransactionLogFilename) : coreMemoryAdapter;
            Links = new SynchronizedLinks<ulong>(new UInt64Links(MemoryAdapter));
            if (useSequences)
            {
                Sequences = new Sequences(Links, sequencesOptions);
            }
        }

        /// <summary>
        /// <para>
        /// Disposes the manual.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="manual">
        /// <para>The manual.</para>
        /// <para></para>
        /// </param>
        /// <param name="wasDisposed">
        /// <para>The was disposed.</para>
        /// <para></para>
        /// </param>
        protected override void Dispose(bool manual, bool wasDisposed)
        {
            if (!wasDisposed)
            {
                Links.Unsync.DisposeIfPossible();
                if (_deleteFiles)
                {
                    DeleteFiles();
                }
            }
        }

        /// <summary>
        /// <para>
        /// Deletes the files.
        /// </para>
        /// <para></para>
        /// </summary>
        public void DeleteFiles()
        {
            File.Delete(TempFilename);
            File.Delete(TempTransactionLogFilename);
        }
    }
}
