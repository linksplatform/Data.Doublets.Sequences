using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Reflection;
using Platform.Converters;
using Platform.Numbers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Numbers.Unary
{
    /// <summary>
    /// <para>
    /// Represents the unary number to address or operation converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{TLinkAddress}"/>
    public class UnaryNumberToAddressOrOperationConverter<TLinkAddress> : LinksOperatorBase<TLinkAddress>, IConverter<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>, IBitwiseOperators<TLinkAddress, TLinkAddress, TLinkAddress>, IShiftOperators<TLinkAddress, int, TLinkAddress>
    {
        private readonly IDictionary<TLinkAddress, int> _unaryNumberPowerOf2Indicies;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="UnaryNumberToAddressOrOperationConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="powerOf2ToUnaryNumberConverter">
        /// <para>A power of to unary number converter.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnaryNumberToAddressOrOperationConverter(ILinks<TLinkAddress> links, IConverter<int, TLinkAddress> powerOf2ToUnaryNumberConverter) : base(links) => _unaryNumberPowerOf2Indicies = CreateUnaryNumberPowerOf2IndiciesDictionary(powerOf2ToUnaryNumberConverter);

        /// <summary>
        /// <para>
        /// Converts the source number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sourceNumber">
        /// <para>The source number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The target.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Convert(TLinkAddress sourceNumber)
        {
            var links = _links;
            var nullConstant = links.Constants.Null;
            var source = sourceNumber;
            var target = nullConstant;
            if ((source != nullConstant))
            {
                while (true)
                {
                    if (_unaryNumberPowerOf2Indicies.TryGetValue(source, out int powerOf2Index))
                    {
                        SetBit(ref target, powerOf2Index);
                        break;
                    }
                    else
                    {
                        powerOf2Index = _unaryNumberPowerOf2Indicies[links.GetSource(source)];
                        SetBit(ref target, powerOf2Index);
                        source = links.GetTarget(source);
                    }
                }
            }
            return target;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<TLinkAddress, int> CreateUnaryNumberPowerOf2IndiciesDictionary(IConverter<int, TLinkAddress> powerOf2ToUnaryNumberConverter)
        {
            var unaryNumberPowerOf2Indicies = new Dictionary<TLinkAddress, int>();
            for (int i = 0; i < NumericType<TLinkAddress>.BitsSize; i++)
            {
                unaryNumberPowerOf2Indicies.Add(powerOf2ToUnaryNumberConverter.Convert(i), i);
            }
            return unaryNumberPowerOf2Indicies;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetBit(ref TLinkAddress target, int powerOf2Index) => target = (target | TLinkAddress.CreateTruncating(1 << powerOf2Index));
    }
}
