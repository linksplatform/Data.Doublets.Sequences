using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Interfaces;
using Platform.Converters;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.HeightProviders
{
    /// <summary>
    /// <para>
    /// Represents the cached sequence height provider.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="ISequenceHeightProvider{TLinkAddress}"/>
    public class CachedSequenceHeightProvider<TLinkAddress> : ISequenceHeightProvider<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
    {
        private readonly TLinkAddress _heightPropertyMarker;
        private readonly ISequenceHeightProvider<TLinkAddress> _baseHeightProvider;
        private readonly IConverter<TLinkAddress> _addressToUnaryNumberConverter;
        private readonly IConverter<TLinkAddress> _unaryNumberToAddressConverter;
        private readonly IProperties<TLinkAddress, TLinkAddress, TLinkAddress> _propertyOperator;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="CachedSequenceHeightProvider"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="baseHeightProvider">
        /// <para>A base height provider.</para>
        /// <para></para>
        /// </param>
        /// <param name="addressToUnaryNumberConverter">
        /// <para>A address to unary number converter.</para>
        /// <para></para>
        /// </param>
        /// <param name="unaryNumberToAddressConverter">
        /// <para>A unary number to address converter.</para>
        /// <para></para>
        /// </param>
        /// <param name="heightPropertyMarker">
        /// <para>A height property marker.</para>
        /// <para></para>
        /// </param>
        /// <param name="propertyOperator">
        /// <para>A property operator.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CachedSequenceHeightProvider(
            ISequenceHeightProvider<TLinkAddress> baseHeightProvider,
            IConverter<TLinkAddress> addressToUnaryNumberConverter,
            IConverter<TLinkAddress> unaryNumberToAddressConverter,
            TLinkAddress heightPropertyMarker,
            IProperties<TLinkAddress, TLinkAddress, TLinkAddress> propertyOperator)
        {
            _heightPropertyMarker = heightPropertyMarker;
            _baseHeightProvider = baseHeightProvider;
            _addressToUnaryNumberConverter = addressToUnaryNumberConverter;
            _unaryNumberToAddressConverter = unaryNumberToAddressConverter;
            _propertyOperator = propertyOperator;
        }

        /// <summary>
        /// <para>
        /// Gets the sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The height.</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLinkAddress Get(TLinkAddress sequence)
        {
            TLinkAddress height;
            var heightValue = _propertyOperator.GetValue(sequence, _heightPropertyMarker);
            if (heightValue == TLinkAddress.Zero)
            {
                height = _baseHeightProvider.Get(sequence);
                heightValue = _addressToUnaryNumberConverter.Convert(height);
                _propertyOperator.SetValue(sequence, _heightPropertyMarker, heightValue);
            }
            else
            {
                height = _unaryNumberToAddressConverter.Convert(heightValue);
            }
            return height;
        }
    }
}
