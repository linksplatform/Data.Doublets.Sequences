using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Converters;
using Platform.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Numbers.Unary
{
    /// <summary>
    /// <para>
    /// Represents the address to unary number converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{TLinkAddress}"/>
    public class AddressToUnaryNumberConverter<TLinkAddress> : LinksOperatorBase<TLinkAddress>, IConverter<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>, IBitwiseOperators<TLinkAddress, TLinkAddress, TLinkAddress>, IShiftOperators<TLinkAddress, int, TLinkAddress>
    {
        private readonly IConverter<int, TLinkAddress> _powerOf2ToUnaryNumberConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="AddressToUnaryNumberConverter"/> instance.
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
        public AddressToUnaryNumberConverter(ILinks<TLinkAddress> links, IConverter<int, TLinkAddress> powerOf2ToUnaryNumberConverter) : base(links) => _powerOf2ToUnaryNumberConverter = powerOf2ToUnaryNumberConverter;

        /// <summary>
        /// <para>
        /// Converts the number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The target.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Convert(TLinkAddress number)
        {
            var links = _links;
            var nullConstant = links.Constants.Null;
            var target = nullConstant;
            for (var i = 0; (number !=  TLinkAddress.Zero) && i < NumericType<TLinkAddress>.BitsSize; i++)
            {
                if (((number & TLinkAddress.One) == TLinkAddress.One))
                {
                    target = target == nullConstant
                        ? _powerOf2ToUnaryNumberConverter.Convert(i)
                        : links.GetOrCreate(_powerOf2ToUnaryNumberConverter.Convert(i), target);
                }
                number = (number >> 1);
            }
            return target;
        }
    }
}
