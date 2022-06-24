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
        foreach (var b in source)
        {
            TestExtensions.PrettifyBinary<byte>(System.Convert.ToString(b));
        }        
        if (source.Count == 0)
        {
            return EmptyArrayType;
        }
        List<TLinkAddress> rawNumberList = new(source.Count / BytesInRawNumberCount + source.Count);
        var byteArray = source.ToArray();
        var i = 0;
        TLinkAddress notSavedBits = default;
        List<TLinkAddress> rawNumberWithoutBitMaskList = new();
        while (byteArray.Length != 0)
        {
            var nonSavedBitsCount = i % 8;
            if (nonSavedBitsCount == 0)
            {
                nonSavedBitsCount = 1;
            }
            if (i == 0)
            {
                var rawNumber = byteArray.ToStructure<TLinkAddress>();
                notSavedBits = Bit.ShiftRight(rawNumber, BitsSize - 1);
                // Console.WriteLine($"Not saved bits: {System.Convert.ToString(TLinkAddressToByteConverter.Convert(notSavedBits), 2)}");
                // Console.WriteLine($"Raw number (not converted): {TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((uint)(object)rawNumber, 2))}");
                rawNumber = Bit.And(rawNumber, BitMask);
                rawNumber = AddressToNumberConverter.Convert(rawNumber);
                rawNumberList.Add(rawNumber);
                byteArray = byteArray.Skip(BytesInRawNumberCount).ToArray();
            }
            else
            {
                var rawNumber = byteArray.ToStructure<TLinkAddress>();
                var newNotSavedBits = Bit.ShiftRight(rawNumber, BitsSize - nonSavedBitsCount);
                // Console.WriteLine($"Not saved bits: {System.Convert.ToString(TLinkAddressToByteConverter.Convert(notSavedBits), 2)}");
                // Shift left for non saved bits from previoys raw number
                rawNumber = Bit.ShiftLeft(rawNumber, nonSavedBitsCount);
                // Console.WriteLine($"Raw number (not converted): {TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((uint)(object)rawNumber, 2))}");
                // Put non saved bits at the start
                rawNumber = Bit.Or(rawNumber, notSavedBits);
                notSavedBits = newNotSavedBits;
                // Console.WriteLine($"Raw number with non saved bits at the start (not converted): {TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((uint)(object)rawNumber, 2))}");
                // Mask last bit
                rawNumber = Bit.And(rawNumber, BitMask);
                rawNumber = AddressToNumberConverter.Convert(rawNumber);
                rawNumberList.Add(rawNumber);
                var bytesInRawNumberCount = BytesInRawNumberCount;
                if (i % 7 == 0)
                {
                    bytesInRawNumberCount--;
                }
                byteArray = byteArray.Skip(bytesInRawNumberCount).ToArray();
            }
            i++;
        }
        var length = IntToTLinkAddressConverter.Convert(source.Count);
        var byteArrayLengthAddress = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        var byteArraySequenceAddress = ListToSequenceConverter.Convert(rawNumberList);
        Console.WriteLine("Raw numbers in byte list to raw sequence converter:");
        foreach (var linkAddress in rawNumberList)
        {
            Console.WriteLine(TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((uint)(object)linkAddress, 2)));
        }
        return _links.GetOrCreate(byteArrayLengthAddress, byteArraySequenceAddress);
    }
}