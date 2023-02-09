using System.Collections.Generic;
using System.Numerics;
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
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Sequences.Tests
{
    public class DefaultSequenceAppenderTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultSequenceAppenderTests(ITestOutputHelper output)
        {
            _output = output;
        }
        public static ILinks<TLinkAddress> CreateLinks() => CreateLinks(new IO.TemporaryFile());

        public static ILinks<TLinkAddress> CreateLinks(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }
        
        public class ValueCriterionMatcher<TLinkAddress> : ICriterionMatcher<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
        {
            public readonly ILinks<TLinkAddress> Links;
            public readonly TLinkAddress Marker;
            public ValueCriterionMatcher(ILinks<TLinkAddress> links, TLinkAddress marker)
            {
                Links = links;
                Marker = marker;
            }

            public bool IsMatched(TLinkAddress link) => (Links.GetSource(link) == Marker);
        }

        [Fact]
        public void AppendArrayBug()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var markerIndex = TLinkAddress.CreateTruncating(1);
            var meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            var sequence = links.Create();
            sequence = links.Update(sequence, meaningRoot, sequence);
            var appendant = links.Create();
            appendant = links.Update(appendant, meaningRoot, appendant);
            ValueCriterionMatcher<TLinkAddress> valueCriterionMatcher = new(links, meaningRoot);
            DefaultSequenceRightHeightProvider<ulong> defaultSequenceRightHeightProvider = new(links, valueCriterionMatcher);
            DefaultSequenceAppender<TLinkAddress> defaultSequenceAppender = new(links, new DefaultStack<ulong>(), defaultSequenceRightHeightProvider);
            var newArray = defaultSequenceAppender.Append(sequence, appendant);
            var output = links.FormatStructure(newArray, link => link.IsFullPoint(), true);
            Assert.Equal("(4:(2:1 2) (3:1 3))", output);
        }
    }
}
