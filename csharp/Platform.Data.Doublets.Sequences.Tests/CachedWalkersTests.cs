using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;
using Platform.Collections;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Memory;
using TLinkAddress = System.UInt16;

namespace Platform.Data.Doublets.Sequences.Tests
{
    public class CachedWalkersTests
    {
        [Fact]
        public void CachedSequenceWalkerBaseTest()
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            var storageFileName = new IO.TemporaryFile().Filename;
            var storageMemory = new FileMappedResizableDirectMemory(storageFileName);
            var links = new UnitedMemoryLinks<TLinkAddress>(storageMemory, UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
            var stack = new DefaultStack<TLinkAddress>();
            var isElementCallCount = 0;
            
            bool IsElement(TLinkAddress link)
            {
                isElementCallCount++;
                return links.IsPartialPoint(link);
            }

            var cachedWalker = new CachedLeftSequenceWalker<TLinkAddress>(links, stack, IsElement);

            // Create some test elements
            var element1 = links.Create();

            // Reset call count before testing
            isElementCallCount = 0;

            // Test caching by calling IsElement multiple times on same element through reflection
            var isElementMethod = cachedWalker.GetType().GetMethod("IsElement", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var result1 = isElementMethod?.Invoke(cachedWalker, new object[] { element1 });
            var result2 = isElementMethod?.Invoke(cachedWalker, new object[] { element1 });

            // Should have been called only once due to caching
            Assert.Equal(1, isElementCallCount);
            Assert.Equal(result1, result2);

            // Cache should contain one element
            Assert.Equal(1, cachedWalker.GetCacheSize());

            // Clear cache
            cachedWalker.ClearCache();
            Assert.Equal(0, cachedWalker.GetCacheSize());

            links.Dispose();
        }

        [Fact]
        public void CachedWalkerConstructorTest()
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            var storageFileName = new IO.TemporaryFile().Filename;
            var storageMemory = new FileMappedResizableDirectMemory(storageFileName);
            var links = new UnitedMemoryLinks<TLinkAddress>(storageMemory, UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
            var stack = new DefaultStack<TLinkAddress>();
            
            // Test that constructors work without throwing exceptions
            var cachedLeftWalker = new CachedLeftSequenceWalker<TLinkAddress>(links, stack);
            var cachedRightWalker = new CachedRightSequenceWalker<TLinkAddress>(links, stack);
            var cachedLeveledWalker = new CachedLeveledSequenceWalker<TLinkAddress>(links);

            Assert.NotNull(cachedLeftWalker);
            Assert.NotNull(cachedRightWalker);
            Assert.NotNull(cachedLeveledWalker);

            // All should start with empty cache
            Assert.Equal(0, cachedLeftWalker.GetCacheSize());
            Assert.Equal(0, cachedRightWalker.GetCacheSize());
            Assert.Equal(0, cachedLeveledWalker.GetCacheSize());

            links.Dispose();
        }
    }
}