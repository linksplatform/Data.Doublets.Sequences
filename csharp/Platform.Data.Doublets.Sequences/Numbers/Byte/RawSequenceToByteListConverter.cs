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
    
    public readonly IConverter<TLinkAddress> NumberToAddressConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;

    public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverteer;
    
    public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverteer;

    public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
    


    public RawSequenceToByteListConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>,TLinkAddress> listToSequenceConverter, StringToUnicodeSequenceConverter<TLinkAddress> stringToUnicodeSequenceConverter) : base(links)
    {
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(links);
        StringToUnicodeSequenceConverteer = stringToUnicodeSequenceConverter;
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

    private byte GetByteFromBitArray(BitArray bitArray ) 
    {
        byte[] currentByte = new byte[1];
        bitArray.CopyTo(currentByte, 0);
        return currentByte[0];
    }

    public IList<byte> Convert(TLinkAddress source)
    {
        var byteArrayLengthAddress = _links.GetSource(source);
        var byteArrayLength = GetByteArrayLength(byteArrayLengthAddress);
        List<byte> byteList = new(byteArrayLength);
        List<bool> currentByteBitList = new(8);
        RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_links, new DefaultStack<TLinkAddress>());
        var sequenceAddress = _links.GetTarget(source);
        var sequence = rightSequenceWalker.Walk(sequenceAddress);
        CheckedConverter<TLinkAddress, byte> checkedConverter = CheckedConverter<TLinkAddress, byte>.Default;
        foreach (var currentByteAddress in sequence)
        {
            var a = NumberToAddressConverter.Convert(currentByteAddress);
            var bitArray = new BitArray(a.ToBytes());
            for (var i = 0; i < bitArray.Count - 1 && i <= byteArrayLength * 8; i++)
            {
                if (currentByteBitList.Count == 8)
                {
                    var currentByteBitArray = new BitArray(currentByteBitList.ToArray());
                    var currentByte = GetByteFromBitArray(currentByteBitArray);
                    byteList.Add(currentByte);
                    currentByteBitList.Clear();
                }
                var currentBit = bitArray[i];
                currentByteBitList.Add(currentBit);
            }
        }
        return byteList;
    }
}
