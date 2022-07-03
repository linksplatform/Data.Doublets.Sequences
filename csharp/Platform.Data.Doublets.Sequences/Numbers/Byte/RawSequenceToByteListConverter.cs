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
        TLinkAddress zero = default;
        var type = zero.Increment();
        var emptyArrayTypeUnicodeSequence = StringToUnicodeSequenceConverteer.Convert("EmptyArrayType");
        var emptyArrayType = _links.SearchOrDefault(type, emptyArrayTypeUnicodeSequence);
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
            var output = TestExtensions.PrettifyBinary<uint>(System.Convert.ToString((uint)(object)currentRawNumber, 2));
            Console.WriteLine(output);
            var nonSavedBitsCount = i;
            if (i != 0)
            {
                int newNotSavedBitsCount = nonSavedBitsCount;
                // Get last byte bits and add its last bits to it
                var byteWithNonSavedBitsAtEnd = GetByteWithNotSavedBitsAtEnd(currentRawNumber, newNotSavedBitsCount);
                var lastByte = Bit.Or(byteList.Last(), byteWithNonSavedBitsAtEnd);
                Console.WriteLine($"lastByte {lastByte}");
                byteList[byteList.Count - 1] = lastByte;
                // Shift bits from last byte
                currentRawNumber = Bit.ShiftRight(currentRawNumber, newNotSavedBitsCount);
            }
            if (nonSavedBitsCount % 8 == 0)
            {
                nonSavedBitsCount = 1;
            }
            // TODO: Temporary hack fix
            if (nonSavedBitsCount % 7 == 0)
            {
                byteList.RemoveAt(byteList.Count - 1);
            }
            // Count how many bytes in raw number 
            int bytesInRawNumberCount = nonSavedBitsCount % 7 != 0 ? BytesInRawNumberCount : BytesInRawNumberCount + 1;
            Console.WriteLine(nonSavedBitsCount);
            Console.WriteLine(nonSavedBitsCount % 7 != 0);
            for (int j = 0; j < bytesInRawNumberCount && byteList.Count != byteArrayLength; j++)
            {
                var currentByte = TLinkAddressToByteConverter.Convert(currentRawNumber);
                output = TestExtensions.PrettifyBinary<byte>(System.Convert.ToString(currentByte, 2));
                Console.WriteLine(j);
                Console.WriteLine(output);
                byteList.Add(currentByte);
                // Shift current byte from raw number to get other bytes
                currentRawNumber = Bit.ShiftRight(currentRawNumber, 8);
            }
            i++;
        }
        return byteList;
    }

    private static byte GetByteWithNotSavedBitsAtEnd(TLinkAddress currentRawNumber, int nonSavedBits)
    {
        var @byte = TLinkAddressToByteConverter.Convert(currentRawNumber);
        @byte = Bit.ShiftLeft(@byte, 8 - nonSavedBits);
        return @byte;
    }
}
