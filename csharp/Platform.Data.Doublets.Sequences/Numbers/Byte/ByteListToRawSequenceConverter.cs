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

    public readonly TLinkAddress ByteArrayType;

    public static readonly UncheckedConverter<Int32, TLinkAddress> IntToTLinkAddressConverter = UncheckedConverter<int, TLinkAddress>.Default;
    public static readonly UncheckedConverter<TLinkAddress, byte> TLinkAddressToByteConverter = UncheckedConverter<TLinkAddress, byte>.Default;
    public ArraySegment<byte> CurrentByteArray;
    public static readonly int BytesInRawNumberCount = BitsSize / 8;
    public readonly UncheckedConverter<byte, TLinkAddress> ByteToTLinkAddressConverter = UncheckedConverter<byte, TLinkAddress>.Default;
    public TLinkAddress EmptyByteArraySequenceType;


    public ByteListToRawSequenceConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> addressToNumberConverter, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>,TLinkAddress> listToSequenceConverter, StringToUnicodeSequenceConverter<TLinkAddress> stringToUnicodeSequenceConverter) : base(links)
    {
        AddressToNumberConverter = addressToNumberConverter;
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(_links);
        TLinkAddress Zero = default;
        Type = Arithmetic.Increment(Zero);
        ByteArrayLengthType = _links.GetOrCreate(Type, stringToUnicodeSequenceConverter.Convert(nameof(ByteArrayLengthType)));
        ByteArrayType = _links.GetOrCreate(Type, stringToUnicodeSequenceConverter.Convert(nameof(ByteArrayType)));
        EmptyByteArraySequenceType = _links.GetOrCreate(Type, stringToUnicodeSequenceConverter.Convert(nameof(EmptyByteArraySequenceType)));
    }
    
    private int GetNonSavedBitsCount(int i)
    {
        if (i == 0)
        {
            return 0;
        }
        var nonSavedBitsCount = i % 8;
        // if (nonSavedBitsCount == 0)
        // {
        //     return 1;
        // }
        return nonSavedBitsCount;
    }
    
    public TLinkAddress Convert(IList<byte> source)
    {
        Console.WriteLine("Byte list to raw sequence conveters");
        var rawNumbers = new List<TLinkAddress>();
        var rawrawNumbers = new List<TLinkAddress>();
        if (source.Count == 0)
        {
            return Links.GetOrCreate(ByteArrayType, EmptyByteArraySequenceType);
        }
        var byteArrayToSave = source.ToArray();
        // while (byteArrayToSave.Length != 0)
        // {
        //     var rawNumber = byteArrayToSave.ToStructure<TLinkAddress>();
        //     rawNumbers.Add(rawNumber);
        //     var bytesInRawNumberCount = rawNumbers.Count % 7 == 0 ? BytesInRawNumberCount - 1 : BytesInRawNumberCount;
        //     byteArrayToSave = byteArrayToSave.Skip(bytesInRawNumberCount).ToArray();
        // }
        // var processedRawNumbers = new List<TLinkAddress>(rawNumbers.Count);
        // for (var j = 0; j < rawNumbers.Count; j++)
        // {
        //     var processedRawNumber = rawNumbers[j];
        //     var nonSavedBitsCount = GetNonSavedBitsCount(j);
        //     if (nonSavedBitsCount != 0)
        //     {
        //         processedRawNumber = Bit.ShiftLeft(processedRawNumber, nonSavedBitsCount);
        //         var nonSavedBits = Bit.ShiftRight(rawNumbers[j - 1], BitsSize - nonSavedBitsCount);
        //         processedRawNumber = Bit.Or(processedRawNumber, nonSavedBits);
        //     }
        //     processedRawNumber = AddressToNumberConverter.Convert(processedRawNumber);
        //     processedRawNumbers.Add(processedRawNumber);
        // }
        TLinkAddress lastRawNumber = default;
        var i = 0;
        while (byteArrayToSave.Length != 0)
        {
            var rawNumber = byteArrayToSave.ToStructure<TLinkAddress>();
            rawrawNumbers.Add(rawNumber);
            Console.WriteLine("Raw number:");
            Console.WriteLine(TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((ushort)(object)rawNumber, 2)));
            var nonSavedBitsCount = GetNonSavedBitsCount(i);
            var prevRawNumberWithNonSavedBitsAtStart = Bit.ShiftRight(lastRawNumber, BitsSize - nonSavedBitsCount);
            var processedRawNumber = Bit.ShiftLeft(rawNumber, nonSavedBitsCount);
            processedRawNumber = Bit.Or(processedRawNumber, prevRawNumberWithNonSavedBitsAtStart);
            processedRawNumber = Bit.And(processedRawNumber, BitMask);
            Console.WriteLine("Processed raw number:");
            Console.WriteLine(TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((ushort)(object)processedRawNumber, 2)));
            processedRawNumber = AddressToNumberConverter.Convert(processedRawNumber);
            var bytesInRawNumberCount = nonSavedBitsCount == 7 ? BytesInRawNumberCount - 1 : BytesInRawNumberCount;
            byteArrayToSave = byteArrayToSave.Skip(bytesInRawNumberCount).ToArray();
            lastRawNumber = rawNumber;
            i++;
            rawNumbers.Add(processedRawNumber);
        }
        var notSavedBitsCount = GetNonSavedBitsCount(i);
        if ((source.Count != BitsSize) && (source.Count % BytesInRawNumberCount == 0))
        {
            lastRawNumber = Bit.ShiftRight(lastRawNumber, BitsSize - notSavedBitsCount);
            lastRawNumber = AddressToNumberConverter.Convert(lastRawNumber);
            rawNumbers.Add(lastRawNumber);
        }
        Console.WriteLine("Raw numbers");
        rawrawNumbers.Reverse();
        StringBuilder rawrawNumbersSb = new StringBuilder();
        foreach (var rawrawNumber in rawrawNumbers)
        {
            rawrawNumbersSb.Append(System.Convert.ToString((ushort)(object)rawrawNumber, 2));
        }
        Console.WriteLine(rawrawNumbersSb.ToString());

        rawNumbers.Reverse();
        Console.WriteLine("Processed raw numbers");
        StringBuilder rawNumbersSb = new StringBuilder();
        foreach (var rawNumber in rawNumbers)
        {
            rawNumbersSb.Append(System.Convert.ToString((ushort)(object)NumberToAddressConverter.Convert(rawNumber), 2));
        }
        for (int c = 31; c < rawNumbersSb.Length; c += 32)
        {
            rawNumbersSb.Remove(c, 1);
        }
        Console.WriteLine(rawNumbersSb.ToString());
        var length = IntToTLinkAddressConverter.Convert(source.Count);
        var byteArrayLengthAddress = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        var byteArraySequenceAddress = ListToSequenceConverter.Convert(rawNumbers);
        return _links.GetOrCreate(ByteArrayType, _links.GetOrCreate(byteArrayLengthAddress, byteArraySequenceAddress));
        
        // var processedRawNumbers = new List<TLinkAddress>();
        // if (source.Count == 0)
        // {
        //     return Links.GetOrCreate(ByteArrayType, EmptyByteArraySequenceType);
        // }
        // var byteArrayToSave = source.ToArray();
        // TLinkAddress lastRawNumber = default;
        // var i = 0;
        // while (byteArrayToSave.Length != 0)
        // {
        //     var rawNumber = byteArrayToSave.ToStructure<TLinkAddress>();
        //     var nonSavedBitsCount = GetNonSavedBitsCount(i);
        //     var prevRawNumberWithNonSavedBitsAtStart = Bit.ShiftRight(lastRawNumber, BitsSize - nonSavedBitsCount);
        //     var processedRawNumber = Bit.ShiftLeft(rawNumber, nonSavedBitsCount);
        //     processedRawNumber = Bit.Or(processedRawNumber, prevRawNumberWithNonSavedBitsAtStart);
        //     processedRawNumber = Bit.And(processedRawNumber, BitMask);
        //     processedRawNumber = AddressToNumberConverter.Convert(processedRawNumber);
        //     var bytesInRawNumberCount = nonSavedBitsCount == 7 ? BytesInRawNumberCount - 1 : BytesInRawNumberCount;
        //     byteArrayToSave = byteArrayToSave.Skip(bytesInRawNumberCount).ToArray();
        //     lastRawNumber = rawNumber;
        //     i++;
        //     processedRawNumbers.Add(processedRawNumber);
        // }
        // var notSavedBitsCount = GetNonSavedBitsCount(i);
        // if (source.Count % BytesInRawNumberCount == 0)
        // {
        //     lastRawNumber = Bit.ShiftRight(lastRawNumber, BitsSize - notSavedBitsCount);
        //     lastRawNumber = AddressToNumberConverter.Convert(lastRawNumber);
        //     processedRawNumbers.Add(lastRawNumber);
        // }
        // var length = IntToTLinkAddressConverter.Convert(source.Count);
        // var byteArrayLengthAddress = _links.GetOrCreate(ByteArrayLengthType, AddressToNumberConverter.Convert(length));
        // var byteArraySequenceAddress = ListToSequenceConverter.Convert(processedRawNumbers);
        // return _links.GetOrCreate(ByteArrayType, _links.GetOrCreate(byteArrayLengthAddress, byteArraySequenceAddress));
        
        
        // List<TLinkAddress> rawNumberList = new(source.Count / BytesInRawNumberCount + source.Count);
        // var byteArray = source.ToArray();
        // var i = 0;
        // TLinkAddress rawNumberWithNonSavedBitsAtStart = default;
        // int lastNotSavedBitsCount = 0;
        // bool hasNotSavedBits = false;
        // while (byteArray.Length != 0)
        // {
        //     // if (i % 8 == 0)
        //     if (i == 0)
        //     {
        //         var rawNumber = byteArray.ToStructure<TLinkAddress>();
        //         hasNotSavedBits = byteArray.Length >= BytesInRawNumberCount;
        //         // var output = TestExtensions.PrettifyBinary<uint>(System.Convert.ToString((ushort)(object)rawNumber, 2));
        //         // Console.WriteLine(output);
        //         rawNumberWithNonSavedBitsAtStart = Bit.ShiftRight(rawNumber, BitsSize - 1);
        //         lastNotSavedBitsCount = 1;
        //         rawNumber = Bit.And(rawNumber, BitMask);
        //         var output = TestExtensions.PrettifyBinary<uint>(System.Convert.ToString((ushort)(object)rawNumber, 2));
        //         Console.WriteLine(output);
        //         rawNumber = AddressToNumberConverter.Convert(rawNumber);
        //         // output = TestExtensions.PrettifyBinary<uint>(System.Convert.ToString((ushort)(object)rawNumber, 2));
        //         // Console.WriteLine(output);
        //         rawNumberList.Add(rawNumber);
        //         byteArray = byteArray.Skip(BytesInRawNumberCount).ToArray();
        //     }
        //     else
        //     {
        //         // var lastNotSavedBitsCount = i % 8;
        //         // if (lastNotSavedBitsCount == 0)
        //         // {
        //         //     lastNotSavedBitsCount = 1;
        //         // }
        //         // // if (lastNotSavedBitsCount % BitsSize == 0)
        //         // // {
        //         // //     lastNotSavedBitsCount = 0;
        //         // // }
        //         //
        //         
        //         var newNotSavedBitsCount = lastNotSavedBitsCount + 1;
        //         var rawNumber = byteArray.ToStructure<TLinkAddress>();
        //         hasNotSavedBits = byteArray.Length >= BytesInRawNumberCount;
        //         // TODO: Check for lastNotSavedBitsCount == 0
        //         var newNotSavedBits = Bit.ShiftRight(rawNumber, BitsSize - newNotSavedBitsCount);
        //         // Shift left for non saved bits from previous raw number
        //         rawNumber = Bit.ShiftLeft(rawNumber, lastNotSavedBitsCount);
        //         // Put non saved bits at the start
        //         rawNumber = Bit.Or(rawNumber, rawNumberWithNonSavedBitsAtStart);
        //         // Mask last bit
        //         rawNumber = Bit.And(rawNumber, BitMask);
        //         rawNumberWithNonSavedBitsAtStart = newNotSavedBits;
        //         var output = TestExtensions.PrettifyBinary<uint>(System.Convert.ToString((ushort)(object)rawNumber, 2));
        //         Console.WriteLine(output);
        //         rawNumber = AddressToNumberConverter.Convert(rawNumber);
        //         rawNumberList.Add(rawNumber);
        //         var bytesInRawNumberCount = newNotSavedBitsCount % 7 != 0 ? BytesInRawNumberCount : BytesInRawNumberCount - 1;
        //         Console.WriteLine(bytesInRawNumberCount);
        //         Console.WriteLine(newNotSavedBitsCount);
        //         Console.WriteLine(newNotSavedBitsCount % 7 != 0);
        //         Console.WriteLine(newNotSavedBits);
        //         byteArray = byteArray.Skip(bytesInRawNumberCount).ToArray();
        //         
        //         if (lastNotSavedBitsCount % 8 == 0)
        //         {
        //             lastNotSavedBitsCount = 0;
        //         }
        //         lastNotSavedBitsCount++;
        //     }
        //     i++;
        // }
        // // if(rawNumberWithNonSavedBitsAtStart)
        // // Console.WriteLine();
        // Console.WriteLine(lastNotSavedBitsCount);
        // if (hasNotSavedBits && lastNotSavedBitsCount % 7 != 0)
        // {
        //     var output = TestExtensions.PrettifyBinary<uint>(System.Convert.ToString((ushort)(object)rawNumberWithNonSavedBitsAtStart, 2));
        //     Console.WriteLine(output);
        //     var lastRawNumber = AddressToNumberConverter.Convert(rawNumberWithNonSavedBitsAtStart);
        //     rawNumberList.Add(lastRawNumber);
        // }

    }
}
