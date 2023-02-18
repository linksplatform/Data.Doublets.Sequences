using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Numbers.Raw
{
    /// <summary>
    /// <para>
    /// Represents the number to long raw number sequence converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksDecoratorBase{TTarget}"/>
    /// <seealso cref="IConverter{TSource, TTarget}"/>
    public class NumberToLongRawNumberSequenceConverter<TSource, TTarget> : LinksDecoratorBase<TTarget>, IConverter<TSource, TTarget> 
        where TSource : struct, INumber<TSource>, IComparisonOperators<TSource, TSource, bool>, IShiftOperators<TSource, int, TSource>, IBitwiseOperators<TSource, TSource, TSource>
        where TTarget : struct, IUnsignedNumber<TTarget>, IComparisonOperators<TTarget, TTarget, bool>, IShiftOperators<TTarget, int, TTarget>, IBitwiseOperators<TTarget, TTarget, TTarget>
    {
        private static readonly Comparer<TSource> _comparer = Comparer<TSource>.Default;
        private static readonly TSource _maximumValue = NumericType<TSource>.MaxValue;
        private static readonly int _bitsPerRawNumber = NumericType<TTarget>.BitsSize - 1;
        private static readonly TSource _bitMask = (_maximumValue >> (NumericType<TTarget>.BitsSize + 1));
        private static readonly TSource _maximumConvertableAddress = TSource.CreateChecked(Hybrid<TTarget>.ExternalZero - TTarget.One);
        private readonly IConverter<TTarget> _addressToNumberConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="NumberToLongRawNumberSequenceConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="addressToNumberConverter">
        /// <para>A address to number converter.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberToLongRawNumberSequenceConverter(ILinks<TTarget> links, IConverter<TTarget> addressToNumberConverter) : base(links) => _addressToNumberConverter = addressToNumberConverter;

        /// <summary>
        /// <para>
        /// Converts the source.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="source">
        /// <para>The source.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The target</para>
        /// <para></para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTarget Convert(TSource source)
        {
            if (source > _maximumConvertableAddress)
            {
                var numberPart = (source & _bitMask);
                var convertedNumber = _addressToNumberConverter.Convert(TTarget.CreateTruncating(numberPart));
                return Links.GetOrCreate(convertedNumber, Convert((source >> _bitsPerRawNumber)));
            }
            else
            {
                return _addressToNumberConverter.Convert(TTarget.CreateTruncating(source));
            }
        }
    }
}
