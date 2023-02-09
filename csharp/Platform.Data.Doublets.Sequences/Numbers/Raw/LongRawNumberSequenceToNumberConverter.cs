using System.Numerics;
using System.Runtime.CompilerServices;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.Numbers.Raw
{
    /// <summary>
    /// <para>
    /// Represents the long raw number sequence to number converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksDecoratorBase{TSource}"/>
    /// <seealso cref="IConverter{TSource, TTarget}"/>
    public class LongRawNumberSequenceToNumberConverter<TSource, TTarget> : LinksDecoratorBase<TSource>, IConverter<TSource, TTarget>
        where TSource : struct, IUnsignedNumber<TSource>, IComparisonOperators<TSource, TSource, bool>, IBitwiseOperators<TSource, TSource, TSource>, IShiftOperators<TSource, int, TSource>
        where TTarget : struct, INumber<TTarget>, IComparisonOperators<TTarget, TTarget, bool>, IBitwiseOperators<TTarget, TTarget, TTarget>, IShiftOperators<TTarget, int, TTarget>
    {
        private static readonly int _bitsPerRawNumber = NumericType<TSource>.BitsSize - 1;
        private static readonly UncheckedConverter<TSource, TTarget> _sourceToTargetConverter = UncheckedConverter<TSource, TTarget>.Default;
        private readonly IConverter<TSource> _numberToAddressConverter;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="LongRawNumberSequenceToNumberConverter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="numberToAddressConverter">
        /// <para>A number to address converter.</para>
        /// <para></para>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LongRawNumberSequenceToNumberConverter(ILinks<TSource> links, IConverter<TSource> numberToAddressConverter) : base(links) => _numberToAddressConverter = numberToAddressConverter;

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
            var constants = Links.Constants;
            var externalReferencesRange = constants.ExternalReferencesRange;
            if (externalReferencesRange.HasValue && externalReferencesRange.Value.Contains(source))
            {
                return _sourceToTargetConverter.Convert(_numberToAddressConverter.Convert(source));
            }
            else
            {
                var pair = Links.GetLink(source);
                var walker = new LeftSequenceWalker<TSource>(Links, new DefaultStack<TSource>(), (link) => externalReferencesRange.HasValue && externalReferencesRange.Value.Contains(link));
                TTarget result = TTarget.Zero;
                foreach (var element in walker.Walk(source))
                {
                    result = ((result << (_bitsPerRawNumber)) | Convert(element));
                }
                return result;
            }
        }
    }
}
