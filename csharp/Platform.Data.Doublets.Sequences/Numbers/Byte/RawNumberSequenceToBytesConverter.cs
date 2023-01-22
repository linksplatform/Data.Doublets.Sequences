using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Data.Doublets.Unicode;
using Platform.Data.Numbers.Raw;
using Platform.Numbers;
using Platform.Reflection;
using Platform.Unsafe;

namespace Platform.Data.Doublets.Sequences.Numbers.Byte;

public class RawNumberSequenceToBytesConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<TLinkAddress, IList<byte>> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
{
    public static readonly TLinkAddress MaximumValue = NumericType<TLinkAddress>.MaxValue;

    public static readonly TLinkAddress BitMask = Bit.ShiftRight(MaximumValue, 1);

    public static readonly int BitsSize = NumericType<TLinkAddress>.BitsSize;


    public readonly IConverter<TLinkAddress> NumberToAddressConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;

    public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverteer;

    public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverteer;

    public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;

    public readonly TLinkAddress Type;

    public RawNumberSequenceToBytesConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>, TLinkAddress> listToSequenceConverter, StringToUnicodeSequenceConverter<TLinkAddress> stringToUnicodeSequenceConverter, UnicodeSequenceToStringConverter<TLinkAddress> unicodeSequenceToStringConverteer) : base(links)
    {
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(links);
        StringToUnicodeSequenceConverteer = stringToUnicodeSequenceConverter;
        UnicodeSequenceToStringConverteer = unicodeSequenceToStringConverteer;
        Type = Arithmetic.Increment(TLinkAddress.Zero);
    }

    private bool IsEmptyArray(TLinkAddress array)
    {
        var type = TLinkAddress.One;
        var emptyArrayTypeUnicodeSequence = StringToUnicodeSequenceConverteer.Convert("EmptyArrayType");
        var emptyArrayType = _links.SearchOrDefault(type, emptyArrayTypeUnicodeSequence);
        return emptyArrayType == array;
    }
    
    private void EnsureIsByteArrayLength(TLinkAddress byteArrayLengthAddress)
    {
        var source = _links.GetSource(byteArrayLengthAddress);
        var type = Arithmetic.Increment(TLinkAddress.Zero);
        var byteArrayLengthType = _links.SearchOrDefault(type, StringToUnicodeSequenceConverteer.Convert("ByteArrayLengthType"));
        if (byteArrayLengthType == TLinkAddress.Zero)
        {
            throw new Exception("Could not find ByteArrayLengthType");
        }
        if (source != byteArrayLengthType)
        {
            throw new Exception("Source must be ByteArrayLengthType");
        }
    }

    private int GetByteArrayLength(TLinkAddress byteArrayLinkAddress)
    {
        EnsureIsByteArray(byteArrayLinkAddress);
        var byteArrayValueLinkAddress = Links.GetTarget(byteArrayLinkAddress);
        var byteArrayLengthLinkAddress = _links.GetSource(byteArrayValueLinkAddress);
        EnsureIsByteArrayLength(byteArrayLengthLinkAddress);
        var lengthValue = _links.GetTarget(byteArrayLengthLinkAddress);
        CheckedConverter<TLinkAddress, int> checkedConverter = CheckedConverter<TLinkAddress, int>.Default;
        return checkedConverter.Convert(NumberToAddressConverter.Convert(lengthValue));
    }

    private void EnsureIsByteArray(TLinkAddress possibleByteArray)
    {
        var byteArrayType = Links.SearchOrDefault(Type, StringToUnicodeSequenceConverteer.Convert("ByteArrayType"));
        if(byteArrayType == TLinkAddress.Zero)
        {
            throw new Exception("ByteArrayType not found in the storage.");
        }
        var possibleByteArrayType = Links.GetSource(possibleByteArray);
        if (possibleByteArrayType != byteArrayType)
        {
            throw new ArgumentException($"{possibleByteArray} is not a byte array.");
        }
    }

    private IEnumerator<TLinkAddress> GetRawNumbersEnumerator(TLinkAddress byteArrayLinkAddress)
    {
        EnsureIsByteArray(byteArrayLinkAddress);
        var byteArrayValueLinkAddress = Links.GetTarget(byteArrayLinkAddress);
        RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_links, new DefaultStack<TLinkAddress>());
        var rawNumberSequenceAddress = _links.GetTarget(byteArrayValueLinkAddress);
        var rawNumberSequence = rightSequenceWalker.Walk(rawNumberSequenceAddress);
        return rawNumberSequence.GetEnumerator();
    }

    public IList<byte> Convert(TLinkAddress source)
    {
        return new RightSequenceWalker<TLinkAddress>(Links, new DefaultStack<TLinkAddress>()).Walk(source).Select(address => NumberToAddressConverter.Convert(address)).Select(address => byte.CreateTruncating(address)).ToList();
        // Console.WriteLine("RawSequenceToByteListConverter.Convert");
        // if (IsEmptyArray(source))
        // {
        //     return new List<byte>();
        // }
        // EnsureIsByteArray(source);
        // var byteArrayLength = GetByteArrayLength(source);
        // List<byte> byteList = new(byteArrayLength);
        // var rawNumbersEnumerator = GetRawNumbersEnumerator(source);
        // var i = 0;
        // while (rawNumbersEnumerator.MoveNext())
        // {
        //     Console.WriteLine("Raw number: ");
        //     var rawNumber = NumberToAddressConverter.Convert(rawNumbersEnumerator.Current);
        //     Console.WriteLine(TestExtensions.PrettifyBinary<TLinkAddress>(System.Convert.ToString((ushort)(object)rawNumber, 2)));
        //     var nonSavedBitsCount = i % 8;
        //     var isLastRawNumber = (byteArrayLength % BytesInRawNumberCount == 0) && (byteList.Count == byteArrayLength);
        //     if(isLastRawNumber)
        //     {
        //         rawNumber = Bit.ShiftLeft(rawNumber, 8 - nonSavedBitsCount);
        //         var @byte = TLinkAddressToByteConverter.Convert(rawNumber);
        //         byteList[byteList.Count - 1] = Bit.Or(byteList.Last(), @byte);
        //         break;
        //     }
        //     if (nonSavedBitsCount != 0)
        //     {
        //         var rawNumberWithOnlyNonSavedBits = rawNumber;
        //         rawNumberWithOnlyNonSavedBits = Bit.ShiftLeft(rawNumberWithOnlyNonSavedBits, BitsSize - nonSavedBitsCount);
        //         rawNumberWithOnlyNonSavedBits = Bit.ShiftRight(rawNumberWithOnlyNonSavedBits, BitsSize - nonSavedBitsCount);
        //         rawNumberWithOnlyNonSavedBits = Bit.ShiftLeft(rawNumberWithOnlyNonSavedBits, 8 - nonSavedBitsCount);
        //         var @byte = TLinkAddressToByteConverter.Convert(rawNumberWithOnlyNonSavedBits);
        //         byteList[byteList.Count - 1] = Bit.Or(byteList.Last(), @byte);
        //         rawNumber = Bit.ShiftRight(rawNumber, nonSavedBitsCount);
        //     }
        //     var bytesInRawNumber = nonSavedBitsCount == 7 ? BytesInRawNumberCount -1 : BytesInRawNumberCount;
        //     for (int j = 0; (j < bytesInRawNumber) && (byteList.Count < byteArrayLength); j++)
        //     {
        //         var @byte = TLinkAddressToByteConverter.Convert(rawNumber);
        //         byteList.Add(@byte);
        //         rawNumber = Bit.ShiftRight(rawNumber, 8);
        //     }
        //     i++;
        // }
        // return byteList;
    }

    private static byte GetByteWithNotSavedBitsAtEnd(TLinkAddress currentRawNumber, int nonSavedBits)
    {
        var @byte = byte.CreateTruncating(currentRawNumber);
        @byte = Bit.ShiftLeft(@byte, 8 - nonSavedBits);
        return @byte;
    }
}
