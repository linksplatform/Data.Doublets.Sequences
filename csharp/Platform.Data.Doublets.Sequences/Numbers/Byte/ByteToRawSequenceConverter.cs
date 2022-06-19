using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Numbers;
using Platform.Reflection;
using Platform.Unsafe;

namespace Platform.Data.Doublets.Sequences.Numbers.Byte;

public class ByteToSequenceConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<IList<byte>, TLinkAddress> where TLinkAddress : struct
{
    public static readonly TLinkAddress MaximumValue = NumericType<TLinkAddress>.MaxValue;

    public static readonly TLinkAddress BitMask = Bit.ShiftRight(MaximumValue, 1);
    
    public readonly IConverter<TLinkAddress> AddressToNumberConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;


    public ByteToSequenceConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> addressToNumberConverter, IConverter<IList<TLinkAddress>,TLinkAddress> listToSequenceConverter) : base(links)
    {
        AddressToNumberConverter = addressToNumberConverter;
        ListToSequenceConverter = listToSequenceConverter;
    }

    public TLinkAddress Convert(IList<byte> source)
    {
        List<TLinkAddress> rawByteAddressList = new();
        var byteArray = source.ToArray();
        var currentByteArray = new ArraySegment<byte>(byteArray);
        var offset = 0;
        while (currentByteArray.Array != null)
        {
            if (offset != 0)
            {
                currentByteArray = new ArraySegment<byte>(byteArray, offset, byteArray.Length - offset);
            }
            var byteArrayWithBitMask = Bit.And(currentByteArray.Array.ToStructure<TLinkAddress>(), BitMask);
            var rawNumber = AddressToNumberConverter.Convert(byteArrayWithBitMask);
            rawByteAddressList.Add(rawNumber);
            offset += NumericType<TLinkAddress>.BitsSize - 1;
        }
        return ListToSequenceConverter.Convert(rawByteAddressList);
    }
}
