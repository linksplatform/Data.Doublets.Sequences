using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Numbers;
using Platform.Reflection;
using Platform.Unsafe;

namespace Platform.Data.Doublets.Sequences.Numbers.Byte;

public class RawSequenceToByteListConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<TLinkAddress, IList<byte>> where TLinkAddress : struct
{
    public static readonly TLinkAddress MaximumValue = NumericType<TLinkAddress>.MaxValue;

    public static readonly TLinkAddress BitMask = Bit.ShiftRight(MaximumValue, 1);
    
    public readonly IConverter<TLinkAddress> NumberToAddressConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;


    public RawSequenceToByteListConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>,TLinkAddress> listToSequenceConverter) : base(links)
    {
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
    }

    public IList<byte> Convert(TLinkAddress source)
    {
        List<byte> byteList = new();
        RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_links, new DefaultStack<TLinkAddress>());
        var sequence = rightSequenceWalker.Walk(source);
        foreach (var currentByteAddress in sequence)
        {
            var currentByte = (byte)(object)NumberToAddressConverter.Convert(currentByteAddress);
            
        }
        var currentByteArray = new ArraySegment<byte>(byteArray);
        var offset = 0;
        while (currentByteArray.Array != null)
        {
            if (offset != 0)
            {
                currentByteArray = new ArraySegment<byte>(byteArray, offset, byteArray.Length - offset);
            }
            var byteArrayWithBitMask = Bit.And(currentByteArray.Array.ToStructure<TLinkAddress>(), BitMask);
            var rawNumber = NumberToAddressConverter.Convert(byteArrayWithBitMask);
            rawByteAddressList.Add(rawNumber);
            offset += NumericType<TLinkAddress>.BitsSize - 1;
        }
        return ListToSequenceConverter.Convert(rawByteAddressList);
    }
}
