using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Converters;
using Platform.Numbers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Numbers.Unary
{
    /// <summary>
    /// <para>
    /// Represents the unary number to address add operation converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
    /// <seealso cref="IConverter{TLinkAddress}"/>
    public class UnaryNumberToAddressAddOperationConverter<TLinkAddress> : LinksOperatorBase<TLinkAddress>, IConverter<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly Dictionary<TLinkAddress, TLinkAddress> _unaryToUInt64;
        private readonly TLinkAddress _unaryOne;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="UnaryNumberToAddressAddOperationConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="unaryOne">
        /// <para>A unary one.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnaryNumberToAddressAddOperationConverter(ILinks<TLinkAddress> links, TLinkAddress unaryOne)
            : base(links)
        {
            _unaryOne = unaryOne;
            _unaryToUInt64 = CreateUnaryToUInt64Dictionary(links, unaryOne);
        }

        /// <summary>
        /// <para>
        /// Converts the unary number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="unaryNumber">
        /// <para>The unary number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Convert(TLinkAddress unaryNumber)
        {
            if ((unaryNumber == TLinkAddress.Zero))
            {
                return TLinkAddress.Zero;
            }
            if ((unaryNumber == _unaryOne))
            {
                return TLinkAddress.One;
            }
            var links = _links;
            var source = links.GetSource(unaryNumber);
            var target = links.GetTarget(unaryNumber);
            if ((source == target))
            {
                return _unaryToUInt64[unaryNumber];
            }
            else
            {
                var result = _unaryToUInt64[source];
                TLinkAddress lastValue;
                while (!_unaryToUInt64.TryGetValue(target, out lastValue))
                {
                    source = links.GetSource(target);
                    result = result + _unaryToUInt64[source];
                    target = links.GetTarget(target);
                }
                result = result + lastValue;
                return result;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<TLinkAddress, TLinkAddress> CreateUnaryToUInt64Dictionary(ILinks<TLinkAddress> links, TLinkAddress unaryOne)
        {
            var unaryToUInt64 = new Dictionary<TLinkAddress, TLinkAddress>
            {
                { unaryOne, TLinkAddress.One }
            };
            var unary = unaryOne;
            var number = TLinkAddress.One;
            for (var i = 1; i < 64; i++)
            {
                unary = links.GetOrCreate(unary, unary);
                number = Double(number);
                unaryToUInt64.Add(unary, number);
            }
            return unaryToUInt64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TLinkAddress Double(TLinkAddress number) => ((number) * TLinkAddress.CreateTruncating(2));
    }
}
