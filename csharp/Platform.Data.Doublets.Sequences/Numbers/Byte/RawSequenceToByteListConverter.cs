using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

public class RawSequenceToByteListConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<TLinkAddress, IList<byte>> where TLinkAddress : struct
{
    public static readonly EqualityComparer<TLinkAddress> EqualityComparer = EqualityComparer<TLinkAddress>.Default;

    public static readonly TLinkAddress MaximumValue = NumericType<TLinkAddress>.MaxValue;

    public static readonly TLinkAddress BitMask = Bit.ShiftRight(MaximumValue, 1);

    public static readonly int BitsSize = NumericType<TLinkAddress>.BitsSize;


    public readonly IConverter<TLinkAddress> NumberToAddressConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;

    public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverteer;

    public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverteer;

    public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
    public static readonly UncheckedConverter<TLinkAddress, byte> TLinkAddressToByteConverter = UncheckedConverter<TLinkAddress, byte>.Default;
    public static readonly int BytesInRawNumberCount = BitsSize / 8;


    public RawSequenceToByteListConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>, TLinkAddress> listToSequenceConverter, StringToUnicodeSequenceConverter<TLinkAddress> stringToUnicodeSequenceConverter) : base(links)
    {
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(links);
        StringToUnicodeSequenceConverteer = stringToUnicodeSequenceConverter;
    }

    private bool IsEmptyArray(TLinkAddress array)
    {
        var emptyArrayType = StringToUnicodeSequenceConverteer.Convert("EmptyArrayType");
        return EqualityComparer.Equals(emptyArrayType, array);
    }
    
    private void EnsureIsByteArrayLength(TLinkAddress byteArrayLengthAddress)
    {
        var source = _links.GetSource(byteArrayLengthAddress);
        TLinkAddress zero = default;
        var type = Arithmetic.Increment(zero);
        var byteArrayLengthType = _links.SearchOrDefault(type, StringToUnicodeSequenceConverteer.Convert("ByteArrayLengthType"));
        if (EqualityComparer.Equals(byteArrayLengthType, default))
        {
            throw new Exception("Could not find ByteArrayLengthType");
        }
        if (!EqualityComparer.Equals(source, byteArrayLengthType))
        {
            throw new Exception("Source must be ByteArrayLengthType");
        }
    }

    private int GetByteArrayLength(TLinkAddress byteArrayLengthAddress)
    {
        EnsureIsByteArrayLength(byteArrayLengthAddress);
        var target = _links.GetTarget(byteArrayLengthAddress);
        CheckedConverter<TLinkAddress, int> checkedConverter = CheckedConverter<TLinkAddress, int>.Default;
        return checkedConverter.Convert(NumberToAddressConverter.Convert(target));
    }

    private byte GetByteFromBitArray(BitArray bitArray)
    {
        byte[] currentByte = new byte[1];
        bitArray.CopyTo(currentByte, 0);
        return currentByte[0];
    }

    public IList<byte> Convert(TLinkAddress source)
    {
        if (IsEmptyArray(source))
        {
            return new List<byte>();
        }
        var byteArrayLengthAddress = _links.GetSource(source);
        var byteArrayLength = GetByteArrayLength(byteArrayLengthAddress);
        List<byte> byteList = new(byteArrayLength);
        RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_links, new DefaultStack<TLinkAddress>());
        var rawNumberSequenceAddress = _links.GetTarget(source);
        var rawNumberSequence = rightSequenceWalker.Walk(rawNumberSequenceAddress);
        using var rawNumberSequenceEnumerator = rawNumberSequence.GetEnumerator();
        var i = 0;
        while (rawNumberSequenceEnumerator.MoveNext())
        {
            var currentRawNumber = NumberToAddressConverter.Convert(rawNumberSequenceEnumerator.Current);
            if (i != 0)
            {
                // Get last byte bits and add its last bits to it
                var byteWithLastByteLastBits = GetByteWithLastByteLastBits(currentRawNumber, i);
                var lastByte = Bit.Or(byteList.Last(), byteWithLastByteLastBits);
                byteList[byteList.Count - 1] = lastByte;
                // Shift bits from last byte
                currentRawNumber = Bit.ShiftRight(currentRawNumber, i);
            }
            // Count how many bytes in raw number 
            int bytesInRawNumberCount = i == 0 ? BytesInRawNumberCount : i % 7 == 0 ? 3 : 4;
            for (int j = 0; j < bytesInRawNumberCount && byteList.Count != byteArrayLength; j++)
            {
                var currentByte = TLinkAddressToByteConverter.Convert(currentRawNumber);
                byteList.Add(currentByte);
                // Shift current byte from raw number to get other bytes
                currentRawNumber = Bit.ShiftRight(currentRawNumber, 8);
            }
            i++;
        }
        return byteList;
        // var byteArrayLengthAddress = _links.GetSource(source);
        // var byteArrayLength = GetByteArrayLength(byteArrayLengthAddress);
        // List<byte> byteList = new(byteArrayLength);
        // List<bool> currentByteBitList = new(8);
        // RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_links, new DefaultStack<TLinkAddress>());
        // var sequenceAddress = _links.GetTarget(source);
        // var sequence = rightSequenceWalker.Walk(sequenceAddress);
        // CheckedConverter<TLinkAddress, byte> checkedConverter = CheckedConverter<TLinkAddress, byte>.Default;
        // var j = 0;
        // foreach (var currentByteAddress in sequence)
        // {
        //     var a = NumberToAddressConverter.Convert(currentByteAddress);
        //     var bitArray = new BitArray(a.ToBytes());
        //     for (var j = 0; j < bitArray.Count - 1 && j <= byteArrayLength * 8; j++)
        //     {
        //         j++;
        //         if (currentByteBitList.Count == 8)
        //         {
        //             var currentByteBitArray = new BitArray(currentByteBitList.ToArray());
        //             var currentByte = GetByteFromBitArray(currentByteBitArray);
        //             byteList.Add(currentByte);
        //             currentByteBitList.Clear();
        //         }
        //         var currentBit = bitArray[j];
        //         currentByteBitList.Add(currentBit);
        //     }
        // }
        // return byteList;
    }

    private static byte GetByteWithLastByteLastBits(TLinkAddress currentRawNumber, int i)
    {
        var currentRawNumberWithLastByteBitsAtEndOfByte = Bit.ShiftLeft(currentRawNumber, 8 - i);
        var byteWithLastByteBitsAtEnd = TLinkAddressToByteConverter.Convert(currentRawNumberWithLastByteBitsAtEndOfByte);
        return byteWithLastByteBitsAtEnd;
    }
}
