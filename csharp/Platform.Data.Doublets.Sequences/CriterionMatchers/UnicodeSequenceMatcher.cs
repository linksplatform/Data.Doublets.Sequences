using System.Collections.Generic;
using System.Numerics;
using Platform.Interfaces;

namespace Platform.Data.Doublets.Sequences.CriterionMatchers;

public class UnicodeSequenceMatcher<TLinkAddress> : ICriterionMatcher<TLinkAddress>  where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
{
    public readonly ILinks<TLinkAddress> Storage;
    public readonly TLinkAddress UnicodeSequenceMarker;
    public UnicodeSequenceMatcher(ILinks<TLinkAddress> storage, TLinkAddress unicodeSequenceMarker)
    {
        Storage = storage;
        UnicodeSequenceMarker = unicodeSequenceMarker;
    }
    public bool IsMatched(TLinkAddress argument)
    {
        var target = Storage.GetTarget(argument);
        return (UnicodeSequenceMarker == argument) || (UnicodeSequenceMarker == target);
    }
}
