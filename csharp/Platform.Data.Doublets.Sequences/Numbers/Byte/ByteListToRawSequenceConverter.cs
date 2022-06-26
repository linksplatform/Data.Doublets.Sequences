using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
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
    public readonly UncheckedConverter<byte, TLinkAddress> ByteToTLinkAddressConverter = UncheckedConverter<byte, TLinkAddress>.Default;
    public TLinkAddress EmptyArrayType;


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
        EmptyArrayType = _links.GetOrCreate(Type, stringToUnicodeSequenceConverter.Convert(nameof(EmptyArrayType)));
    }
    
    public TLinkAddress Convert(IList<byte> source)
    {
        if (source.Count == 0)
        {
            return EmptyArrayType;
        }
        List<TLinkAddress> rawNumberList = new(source.Count / BytesInRawNumberCount + source.Count);
        List<TLinkAddress> notConvertedRawNumberList = new(source.Count / BytesInRawNumberCount + source.Count);
        List<TLinkAddress> rawRawNumberList = new(source.Count / BytesInRawNumberCount + source.Count);
        var byteArray = source.ToArray();
        var i = 0;
        TLinkAddress rawNumberWithNonSavedBitsAtStart = default;
        while (byteArray.Length != 0)
        {
            if (i % 8 == 0)
            {
                var rawNumber = byteArray.ToStructure<TLinkAddress>();
                rawRawNumberList.Add(rawNumber);
                rawNumberWithNonSavedBitsAtStart = Bit.ShiftRight(rawNumber, BitsSize - 1);
                rawNumber = Bit.And(rawNumber, BitMask);
                notConvertedRawNumberList.Add(rawNumber);
                rawNumber = AddressToNumberConverter.Convert(rawNumber);
                rawNumberList.Add(rawNumber);
                byteArray = byteArray.Skip(BytesInRawNumberCount).ToArray();
            }
            else
            {
                var lastNotSavedBitsCount = i % 8;
                if (lastNotSavedBitsCount == 0)
                {
                    lastNotSavedBitsCount = 1;
                }
                var newNotSavedBitsCount = lastNotSavedBitsCount + 1;
                var rawNumber = byteArray.ToStructure<TLinkAddress>();
                rawRawNumberList.Add(rawNumber);
                var newNotSavedBits = Bit.ShiftRight(rawNumber, BitsSize - newNotSavedBitsCount);
                // Shift left for non saved bits from previoys raw number
                rawNumber = Bit.ShiftLeft(rawNumber, lastNotSavedBitsCount);
                // Put non saved bits at the start
                rawNumber = Bit.Or(rawNumber, rawNumberWithNonSavedBitsAtStart);
                // Mask last bit
                rawNumber = Bit.And(rawNumber, BitMask);
                rawNumberWithNonSavedBitsAtStart = newNotSavedBits;
                notConvertedRawNumberList.Add(rawNumber);
                rawNumber = AddressToNumberConverter.Convert(rawNumber);
                rawNumberList.Add(rawNumber);
                var bytesInRawNumberCount = newNotSavedBitsCount % 7 != 0 ? BytesInRawNumberCount : BytesInRawNumberCount - 1;
                byteArray = byteArray.Skip(bytesInRawNumberCount).ToArray();
            }
            i++;
        }
        var length = IntToTLinkAddressConverter.Convert(source.Count);
        var byteArrayLengthAddress = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        var byteArraySequenceAddress = ListToSequenceConverter.Convert(rawNumberList);
        Console.WriteLine("Raw numbers in byte list to raw sequence converter:");
        foreach (var linkAddress in notConvertedRawNumberList)
        {
            Console.Write(System.Convert.ToString((uint)(object)linkAddress, 2));
        }
        Console.WriteLine("Original Raw numbers");
        foreach (var linkAddress in rawRawNumberList)
        {
            Console.Write(System.Convert.ToString((uint)(object)linkAddress, 2));
        }
        return _links.GetOrCreate(byteArrayLengthAddress, byteArraySequenceAddress);
    }
}
