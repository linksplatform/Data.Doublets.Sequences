using System.Collections.Generic;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences;
using Platform.Data.Doublets.Sequences.HeightProviders;
using Platform.Data.Numbers.Raw;
using Platform.Interfaces;
using Platform.Memory;
using Platform.Numbers;
using Xunit;
using Xunit.Abstractions;
using TLink = System.UInt64;

namespace Platform.Data.Doublets.Sequences.Tests
{
    /// <summary>
    /// <para>
    /// Represents the default sequence appender tests.
    /// </para>
    /// <para></para>
    /// </summary>
    public class DefaultSequenceAppenderTests
    {
        /// <summary>
        /// <para>
        /// The output.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DefaultSequenceAppenderTests"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="output">
        /// <para>A output.</para>
        /// <para></para>
        /// </param>
        public DefaultSequenceAppenderTests(ITestOutputHelper output)
        {
            _output = output;
        }
        /// <summary>
        /// <para>
        /// Creates the links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new IO.TemporaryFile());

        /// <summary>
        /// <para>
        /// Creates the links using the specified data db filename.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <typeparam name="TLink">
        /// <para>The link.</para>
        /// <para></para>
        /// </typeparam>
        /// <param name="dataDBFilename">
        /// <para>The data db filename.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A links of t link</para>
        /// <para></para>
        /// </returns>
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        
        /// <summary>
        /// <para>
        /// Represents the value criterion matcher.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <seealso cref="ICriterionMatcher{TLink}"/>
        public class ValueCriterionMatcher<TLink> : ICriterionMatcher<TLink>
        {
            /// <summary>
            /// <para>
            /// The links.
            /// </para>
            /// <para></para>
            /// </summary>
            public readonly ILinks<TLink> Links;
            /// <summary>
            /// <para>
            /// The marker.
            /// </para>
            /// <para></para>
            /// </summary>
            public readonly TLink Marker;
            /// <summary>
            /// <para>
            /// Initializes a new <see cref="ValueCriterionMatcher"/> instance.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="links">
            /// <para>A links.</para>
            /// <para></para>
            /// </param>
            /// <param name="marker">
            /// <para>A marker.</para>
            /// <para></para>
            /// </param>
            public ValueCriterionMatcher(ILinks<TLink> links, TLink marker)
            {
                Links = links;
                Marker = marker;
            }

            /// <summary>
            /// <para>
            /// Determines whether this instance is matched.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="link">
            /// <para>The link.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The bool</para>
            /// <para></para>
            /// </returns>
            public bool IsMatched(TLink link) => EqualityComparer<TLink>.Default.Equals(Links.GetSource(link), Marker);
        }

        /// <summary>
        /// <para>
        /// Tests that append array bug.
        /// </para>
        /// <para></para>
        /// </summary>
        [Fact]
        public void AppendArrayBug()
        {
            ILinks<TLink> links = CreateLinks();
            TLink zero = default;
            var markerIndex = Arithmetic.Increment(zero);
            var meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            var sequence = links.Create();
            sequence = links.Update(sequence, meaningRoot, sequence);
            var appendant = links.Create();
            appendant = links.Update(appendant, meaningRoot, appendant);
            ValueCriterionMatcher<TLink> valueCriterionMatcher = new(links, meaningRoot);
            DefaultSequenceRightHeightProvider<ulong> defaultSequenceRightHeightProvider = new(links, valueCriterionMatcher);
            DefaultSequenceAppender<TLink> defaultSequenceAppender = new(links, new DefaultStack<ulong>(), defaultSequenceRightHeightProvider);
            var newArray = defaultSequenceAppender.Append(sequence, appendant);
            var output = links.FormatStructure(newArray, link => link.IsFullPoint(), true);
            Assert.Equal("(4:(2:1 2) (3:1 3))", output);
        }
    }
}
