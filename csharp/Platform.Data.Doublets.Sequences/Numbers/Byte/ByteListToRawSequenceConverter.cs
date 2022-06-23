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
    private UncheckedConverter<byte, TLinkAddress> ByteToTLinkAddressConverter = UncheckedConverter<byte, TLinkAddress>.Default;


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
        List<TLinkAddress> rawNumberWithBitMaskList = new(source.Count / BytesInRawNumberCount + source.Count);
        var byteArray = source.ToArray();
        var i = 0;
        byte lastByte = default;
        while (byteArray.Length != 0)
        {
            if (i == 0)
            {
                var rawNumberWithoutBitMask = byteArray.ToStructure<TLinkAddress>();
                lastByte = TLinkAddressToByteConverter.Convert(Bit.ShiftRight(rawNumberWithoutBitMask, BitsSize - 8));
                var rawNumberWithBitMask = Bit.And(rawNumberWithoutBitMask, BitMask);
                rawNumberWithBitMask = AddressToNumberConverter.Convert(rawNumberWithBitMask);
                rawNumberWithBitMaskList.Add(rawNumberWithBitMask);
                byteArray = byteArray.Skip(BytesInRawNumberCount).ToArray();
            }
            else
            {
                var rawNumberWithoutBitMask = byteArray.ToStructure<TLinkAddress>();
                var cutBitsFromPrevRawNumberCount = i;
                var cutBitsFromPrevRawNumber = Bit.ShiftRight(lastByte, 8 - cutBitsFromPrevRawNumberCount);
                lastByte = TLinkAddressToByteConverter.Convert(Bit.ShiftRight(rawNumberWithoutBitMask,BitsSize - 8));
                // Shift left to put cut bits from previous raw number to the start of this raw number
                rawNumberWithoutBitMask = Bit.ShiftLeft(rawNumberWithoutBitMask, i);
                var rawNumberWithBitMask = Bit.And(rawNumberWithoutBitMask, BitMask);
                // Put cut bits to the start
                rawNumberWithBitMask = Bit.Or(rawNumberWithBitMask, ByteToTLinkAddressConverter.Convert(cutBitsFromPrevRawNumber));
                var rawNumber = AddressToNumberConverter.Convert(rawNumberWithBitMask);
                rawNumberWithBitMaskList.Add(rawNumber);
                byteArray = byteArray.Skip(BytesInRawNumberCount).ToArray();
            }
            i++;
        }
        var length = IntToTLinkAddressConverter.Convert(source.Count);
        var byteArrayLengthAddress = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        var byteArraySequenceAddress = ListToSequenceConverter.Convert(rawNumberWithBitMaskList);
        return _links.GetOrCreate(byteArrayLengthAddress, byteArraySequenceAddress);
    }
}
