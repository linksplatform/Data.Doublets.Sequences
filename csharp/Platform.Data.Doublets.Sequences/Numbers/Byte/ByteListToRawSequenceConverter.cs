using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Platform.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Unicode;
using Platform.Numbers;
using Platform.Reflection;
using Platform.Unsafe;

namespace Platform.Data.Doublets.Sequences.Numbers.Byte;

public class ByteListToRawSequenceConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<IList<byte>, TLinkAddress> where TLinkAddress : struct
{
    public static readonly TLinkAddress MaximumValue = NumericType<TLinkAddress>.MaxValue;

    public static readonly TLinkAddress BitMask = Bit.ShiftRight(MaximumValue, 1);

    public static readonly int BitsSize = NumericType<TLinkAddress>.BitsSize;
    
    public readonly IConverter<TLinkAddress> AddressToNumberConverter;
    public readonly IConverter<TLinkAddress> NumberToAddressConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;
    
    public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
    
    public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverter;
    
    public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverter;

    public readonly TLinkAddress Type = default;

    public readonly TLinkAddress ByteArrayLengthType;

    public readonly TLinkAddress ByteArraySequenceType;

    public static readonly UncheckedConverter<Int32, TLinkAddress> IntToTLinkAddressConverter = UncheckedConverter<int, TLinkAddress>.Default;


    public ByteListToRawSequenceConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> addressToNumberConverter, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>,TLinkAddress> listToSequenceConverter, StringToUnicodeSequenceConverter<TLinkAddress> stringToUnicodeSequenceConverter) : base(links)
    {
        AddressToNumberConverter = addressToNumberConverter;
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(_links);
        TLinkAddress Zero = default;
        Type = Arithmetic.Increment(Zero);
        ByteArrayLengthType = _links.GetOrCreate(Type, stringToUnicodeSequenceConverter.Convert(nameof(ByteArrayLengthType)));
        ByteArraySequenceType = _links.GetOrCreate(Type, stringToUnicodeSequenceConverter.Convert(nameof(ByteArraySequenceType)));
    }
    
    public TLinkAddress Convert(IList<byte> source)
    {
        var bigIntWithBitMask = Bit.And(source.ToArray().ToStructure<TLinkAddress>(), BitMask);
        var rawNumber = AddressToNumberConverter.Convert(bigIntWithBitMask);
        var length = IntToTLinkAddressConverter.Convert(source.Count);
        var byteArrayLength = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        var byteArraySequence = ListToSequenceConverter.Convert(new List<TLinkAddress>{rawNumber});
        return _links.GetOrCreate(byteArrayLength, byteArraySequence);
    }
}
