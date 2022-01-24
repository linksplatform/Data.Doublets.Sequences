using System;
using System.Collections.Generic;
using System.Numerics;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Reflection;
using Platform.Unsafe;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Numbers.Raw
{
    /// <summary>
    /// <para>
    /// Represents the raw number sequence to big integer converter.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="LinksDecoratorBase{TLink}"/>
    /// <seealso cref="IConverter{TLink, BigInteger}"/>
    public class RawNumberSequenceToBigIntegerConverter<TLink> : LinksDecoratorBase<TLink>, IConverter<TLink, BigInteger>
        where TLink : struct
    {
        /// <summary>
        /// <para>
        /// The default.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly EqualityComparer<TLink> EqualityComparer = EqualityComparer<TLink>.Default;
        /// <summary>
        /// <para>
        /// The number to address converter.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly IConverter<TLink, TLink> NumberToAddressConverter;
        /// <summary>
        /// <para>
        /// The left sequence walker.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly LeftSequenceWalker<TLink> LeftSequenceWalker;
        /// <summary>
        /// <para>
        /// The negative number marker.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly TLink NegativeNumberMarker;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="RawNumberSequenceToBigIntegerConverter"/> instance.
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
        /// <param name="negativeNumberMarker">
        /// <para>A negative number marker.</para>
        /// <para></para>
        /// </param>
        public RawNumberSequenceToBigIntegerConverter(ILinks<TLink> links, IConverter<TLink, TLink> numberToAddressConverter, TLink negativeNumberMarker) : base(links)
        {
            NumberToAddressConverter = numberToAddressConverter;
            LeftSequenceWalker = new(links, new DefaultStack<TLink>());
            NegativeNumberMarker = negativeNumberMarker;
        }

        /// <summary>
        /// <para>
        /// Converts the big integer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="bigInteger">
        /// <para>The big integer.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>Raw number sequence cannot be empty.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The big integer</para>
        /// <para></para>
        /// </returns>
        public BigInteger Convert(TLink bigInteger)
        {
            var sign = 1;
            var bigIntegerSequence = bigInteger;
            if (EqualityComparer.Equals(_links.GetSource(bigIntegerSequence), NegativeNumberMarker))
            {
                sign = -1;
                bigIntegerSequence = _links.GetTarget(bigInteger);
            }
            using var enumerator = LeftSequenceWalker.Walk(bigIntegerSequence).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new Exception("Raw number sequence cannot be empty.");
            }
            var nextPart = NumberToAddressConverter.Convert(enumerator.Current);
            BigInteger currentBigInt = new(nextPart.ToBytes());
            while (enumerator.MoveNext())
            {
                currentBigInt <<= (NumericType<TLink>.BitsSize - 1);
                nextPart = NumberToAddressConverter.Convert(enumerator.Current);
                currentBigInt |= new BigInteger(nextPart.ToBytes());
            }
            return sign == -1 ? BigInteger.Negate(currentBigInt) : currentBigInt;
        }      
    }
}
