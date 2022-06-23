using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Platform.Collections.Stacks;
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
    public static readonly UncheckedConverter<TLinkAddress, byte> TLinkAddressToByteConverter = UncheckedConverter<TLinkAddress, byte>.Default;
    public ArraySegment<byte> CurrentByteArray;
    public static readonly int BytesInRawNumberCount = BitsSize / 8;


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
        List<TLinkAddress> rawNumberList = new(BitsSize / source.Count);
        CurrentByteArray = new ArraySegment<byte>(source.ToArray());
        while (CurrentByteArray.Count > BytesInRawNumberCount)
        {
            rawNumberList.Add(GetRawNumber());
            SetNewCurrentByteArrayOffset();
        }
        var byteArrayLengthAddress = GetByteArrayLengthAddress(source);
        var byteArraySequenceAddress = ListToSequenceConverter.Convert(rawNumberList);
        return _links.GetOrCreate(byteArrayLengthAddress, byteArraySequenceAddress);
    }

    private TLinkAddress GetByteArrayLengthAddress(IList<byte> source)
    {
        var length = IntToTLinkAddressConverter.Convert(source.Count);
        var byteArrayLength = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        return byteArrayLength;
    }

    private void SetNewCurrentByteArrayOffset()
    {
        var newOffset = CurrentByteArray.Offset + BytesInRawNumberCount;
        if (newOffset > CurrentByteArray.Count)
        {
            CurrentByteArray = new ArraySegment<byte>(CurrentByteArray.Array, newOffset, 0);
        }
        CurrentByteArray = new ArraySegment<byte>(CurrentByteArray.Array, newOffset, CurrentByteArray.Count - newOffset);
    }

    private TLinkAddress GetRawNumber()
    {
        var rawNumber = CurrentByteArray.ToArray().ToStructure<TLinkAddress>();
        var rawNumberWithBitMask = Bit.And(rawNumber, BitMask);
        return AddressToNumberConverter.Convert(rawNumberWithBitMask);
    }
}
