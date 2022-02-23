// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using Platform.Incrementers;
//
// #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//
// namespace Platform.Data.Doublets.Incrementers
// {
//     /// <summary>
//     /// <para>
//     /// Represents the unary number incrementer.
//     /// </para>
//     /// <para></para>
//     /// </summary>
//     /// <seealso cref="LinksOperatorBase{TLinkAddress}"/>
//     /// <seealso cref="IIncrementer{TLinkAddress}"/>
//     public class UnaryNumberIncrementer<TLinkAddress> : LinksOperatorBase<TLinkAddress>, IIncrementer<TLinkAddress>
//     {
//         private static readonly EqualityComparer<TLinkAddress> _equalityComparer = EqualityComparer<TLinkAddress>.Default;
//         private readonly TLinkAddress _unaryOne;
//
//         /// <summary>
//         /// <para>
//         /// Initializes a new <see cref="UnaryNumberIncrementer"/> instance.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="links">
//         /// <para>A links.</para>
//         /// <para></para>
//         /// </param>
//         /// <param name="unaryOne">
//         /// <para>A unary one.</para>
//         /// <para></para>
//         /// </param>
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public UnaryNumberIncrementer(ILinks<TLinkAddress> links, TLinkAddress unaryOne) : base(links) => _unaryOne = unaryOne;
//
//         /// <summary>
//         /// <para>
//         /// Increments the unary number.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="unaryNumber">
//         /// <para>The unary number.</para>
//         /// <para></para>
//         /// </param>
//         /// <returns>
//         /// <para>The link</para>
//         /// <para></para>
//         /// </returns>
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public TLinkAddress Increment(TLinkAddress unaryNumber)
//         {
//             var links = _links;
//             if (_equalityComparer.Equals(unaryNumber, _unaryOne))
//             {
//                 return links.GetOrCreate(_unaryOne, _unaryOne);
//             }
//             var source = links.GetSource(unaryNumber);
//             var target = links.GetTarget(unaryNumber);
//             if (_equalityComparer.Equals(source, target))
//             {
//                 return links.GetOrCreate(unaryNumber, _unaryOne);
//             }
//             else
//             {
//                 return links.GetOrCreate(source, Increment(target));
//             }
//         }
//     }
// }
