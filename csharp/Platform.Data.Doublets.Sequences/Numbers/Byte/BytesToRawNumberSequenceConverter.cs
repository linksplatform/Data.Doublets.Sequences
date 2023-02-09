using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
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

public class BytesToRawNumberSequenceConverter<TLinkAddress> : LinksDecoratorBase<TLinkAddress>, IConverter<IList<byte>, TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>, IShiftOperators<TLinkAddress, int, TLinkAddress>
{
    public static readonly TLinkAddress MaximumValue = NumericType<TLinkAddress>.MaxValue;

    public static readonly TLinkAddress BitMask = (MaximumValue >> 1);

    public static readonly int BitsSize = NumericType<TLinkAddress>.BitsSize;
    
    public readonly IConverter<TLinkAddress> AddressToNumberConverter;
    public readonly IConverter<TLinkAddress> NumberToAddressConverter;

    public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;
    
    public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
    
    public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverter;
    
    public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverter;

    public readonly TLinkAddress Type = TLinkAddress.Zero;

    public readonly TLinkAddress ByteArrayLengthType;

    public readonly TLinkAddress ByteArrayType;

    public TLinkAddress EmptyByteArraySequenceType;


    public BytesToRawNumberSequenceConverter(ILinks<TLinkAddress> links, IConverter<TLinkAddress> addressToNumberConverter, IConverter<TLinkAddress> numberToAddressConverter, IConverter<IList<TLinkAddress>,TLinkAddress> listToSequenceConverter, StringToUnicodeSequenceConverter<TLinkAddress> stringToUnicodeSequenceConverter) : base(links)
    {
        AddressToNumberConverter = addressToNumberConverter;
        NumberToAddressConverter = numberToAddressConverter;
        ListToSequenceConverter = listToSequenceConverter;
        BalancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(_links);
        StringToUnicodeSequenceConverter = stringToUnicodeSequenceConverter;
        Type = TLinkAddress.Zero + TLinkAddress.One;
        ByteArrayLengthType = _links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ByteArrayLengthType)));
        ByteArrayType = _links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ByteArrayType)));
        EmptyByteArraySequenceType = _links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(EmptyByteArraySequenceType)));
    }

    public TLinkAddress Convert(IList<byte> source)
    {
        return ListToSequenceConverter.Convert(source.Select(b => AddressToNumberConverter.Convert(TLinkAddress.CreateTruncating(b))).ToList());
    }
}
