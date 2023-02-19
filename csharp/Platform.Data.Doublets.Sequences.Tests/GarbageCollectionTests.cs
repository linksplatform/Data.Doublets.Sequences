using System.Collections.Generic;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Memory;
using TLinkAddress = System.UInt64;
using Xunit;

namespace Platform.Data.Doublets.Sequences.Tests;

public class GarbageCollectionTests
{
    public IResizableDirectMemory LinksMemory;
    public ILinks<TLinkAddress> LinksStorage;
    public BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;

    public GarbageCollectionTests()
    {
        LinksMemory = new HeapResizableDirectMemory();
        LinksStorage = new UnitedMemoryLinks<TLinkAddress>(LinksMemory);
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(LinksStorage);
    }
    
    [Fact]
    public void FullPointsSequence()
    {
        TLinkAddress link1 = LinksStorage.CreatePoint();
        TLinkAddress link2 = LinksStorage.CreatePoint();
        TLinkAddress link3 = LinksStorage.CreatePoint();
        TLinkAddress sequence =  BalancedVariantConverter.Convert(new List<TLinkAddress>{link1, link2, link3});
        LinksStorage.ClearGarbage(sequence);
        Assert.True(LinksStorage.Exists(link1));
        Assert.True(LinksStorage.Exists(link2));
        Assert.True(LinksStorage.Exists(link3));
        Assert.False(LinksStorage.Exists(sequence));
    }
    
    [Fact]
    public void Test()
    {
        TLinkAddress link1 = LinksStorage.CreatePoint();
        TLinkAddress link2 = LinksStorage.CreatePoint();
        TLinkAddress link3 = LinksStorage.GetOrCreate(link1, link2);
        TLinkAddress link4 = LinksStorage.CreatePoint();
        TLinkAddress link5 = LinksStorage.CreatePoint();
        TLinkAddress link6 = LinksStorage.GetOrCreate(link4, link5);
        TLinkAddress sequence =  BalancedVariantConverter.Convert(new List<TLinkAddress>{link3, link6});
        LinksStorage.ClearGarbage(sequence);
        Assert.True(LinksStorage.Exists(link1));
        Assert.True(LinksStorage.Exists(link2));
        Assert.False(LinksStorage.Exists(link3));
        Assert.True(LinksStorage.Exists(link4));
        Assert.True(LinksStorage.Exists(link5));
        Assert.False(LinksStorage.Exists(link6));
        Assert.False(LinksStorage.Exists(sequence));
    }
}
