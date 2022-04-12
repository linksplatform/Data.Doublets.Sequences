using System.Collections.Generic;
using Platform.Interfaces;

namespace Platform.Data.Doublets.Sequences.CriterionMatchers;

public class UnicodeSequenceMatcher<TLinkAddress> : ICriterionMatcher<TLinkAddress>
{
    public readonly ILinks<TLinkAddress> Storage;
    public readonly TLinkAddress UnicodeSequenceMarker;
    public readonly EqualityComparer<TLinkAddress> EqualityComparer = EqualityComparer<TLinkAddress>.Default;
    public UnicodeSequenceMatcher(ILinks<TLinkAddress> storage, TLinkAddress unicodeSequenceMarker)
    {
        Storage = storage;
        UnicodeSequenceMarker = unicodeSequenceMarker;
    }
    public bool IsMatched(TLinkAddress argument)
    {
        var target = Storage.GetTarget(argument);
        return EqualityComparer.Equals(UnicodeSequenceMarker, argument) || EqualityComparer.Equals(UnicodeSequenceMarker, target);
    }
}
