// using System.Collections.Generic;
// using System.Numerics;
// using Xunit;
// using Platform.Ranges;
// using Platform.Numbers;
// using Platform.Random;
// using Platform.Setters;
// using Platform.Converters;
//
// namespace Platform.Data.Doublets.Sequences.Tests
// {
//     public static class TestExtensions
//     {
//         public static void TestCRUDOperations<TLinkAddress>(this ILinks<TLinkAddress> links) where TLinkAddress: struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
//         {
//             var constants = links.Constants;
//
//             // Create Link
//             Assert.True((links.Count() == TLinkAddress.Zero));
//
//             var setter = new Setter<TLinkAddress>(constants.Null);
//             links.Each(constants.Any, constants.Any, setter.SetAndReturnTrue);
//
//             Assert.True((setter.Result == constants.Null));
//
//             var linkAddress = links.Create();
//
//             var link = new Link<TLinkAddress>(links.GetLink(linkAddress));
//
//             Assert.True(link.Count == 3);
//             Assert.True(link.Index == linkAddress);
//             Assert.True(link.Source == constants.Null);
//             Assert.True(link.Target == constants.Null);
//
//             Assert.True(links.Count() == TLinkAddress.One);
//
//             // Get first link
//             setter = new Setter<TLinkAddress>(constants.Null);
//             links.Each(constants.Any, constants.Any, setter.SetAndReturnFalse);
//
//             Assert.True(setter.Result == linkAddress);
//
//             // Update link to reference itself
//             links.Update(linkAddress, linkAddress, linkAddress);
//
//             link = new Link<TLinkAddress>(links.GetLink(linkAddress));
//
//             Assert.True(link.Source == linkAddress);
//             Assert.True(link.Target == linkAddress);
//
//             // Update link to reference null (prepare for delete)
//             var updated = links.Update(linkAddress, constants.Null, constants.Null);
//
//             Assert.True(updated == linkAddress);
//
//             link = new Link<TLinkAddress>(links.GetLink(linkAddress));
//
//             Assert.True(link.Source == constants.Null);
//             Assert.True(link.Target == constants.Null);
//
//             // Delete link
//             links.Delete(linkAddress);
//
//             Assert.True(links.Count() == TLinkAddress.Zero);
//
//             setter = new Setter<TLinkAddress>(constants.Null);
//             links.Each(constants.Any, constants.Any, setter.SetAndReturnTrue);
//
//             Assert.True(setter.Result == constants.Null);
//         }
//
//         public static void TestRawNumbersCRUDOperations<TLinkaddress>(this ILinks<TLinkaddress> links) where TLinkaddress: struct, IUnsignedNumber<TLinkaddress>, IComparisonOperators<TLinkaddress, TLinkaddress, bool>
//         {
//             // Constants
//             var constants = links.Constants;
//
//             var h106E = new Hybrid<TLinkaddress>(106L, isExternal: true);
//             var h107E = new Hybrid<TLinkaddress>(-char.ConvertFromUtf32(107)[0]);
//             var h108E = new Hybrid<TLinkaddress>(-108L);
//
//             Assert.Equal(106L, h106E.AbsoluteValue);
//             Assert.Equal(107L, h107E.AbsoluteValue);
//             Assert.Equal(108L, h108E.AbsoluteValue);
//
//             // Create Link (External -> External)
//             var linkAddress1 = links.Create();
//
//             links.Update(linkAddress1, h106E, h108E);
//
//             var link1 = new Link<TLinkaddress>(links.GetLink(linkAddress1));
//
//             Assert.True(link1.Source == h106E);
//             Assert.True(link1.Target == h108E);
//
//             // Create Link (Internal -> External)
//             var linkAddress2 = links.Create();
//
//             links.Update(linkAddress2, linkAddress1, h108E);
//
//             var link2 = new Link<TLinkaddress>(links.GetLink(linkAddress2));
//
//             Assert.True(link2.Source == linkAddress1);
//             Assert.True(link2.Target == h108E);
//
//             // Create Link (Internal -> Internal)
//             var linkAddress3 = links.Create();
//
//             links.Update(linkAddress3, linkAddress1, linkAddress2);
//
//             var link3 = new Link<TLinkaddress>(links.GetLink(linkAddress3));
//
//             Assert.True(link3.Source == linkAddress1);
//             Assert.True(link3.Target == linkAddress2);
//
//             // Search for created link
//             var setter1 = new Setter<TLinkaddress>(constants.Null);
//             links.Each(h106E, h108E, setter1.SetAndReturnFalse);
//
//             Assert.True(setter1.Result == linkAddress1);
//
//             // Search for nonexistent link
//             var setter2 = new Setter<TLinkaddress>(constants.Null);
//             links.Each(h106E, h107E, setter2.SetAndReturnFalse);
//
//             Assert.True(setter2.Result == constants.Null);
//
//             // Update link to reference null (prepare for delete)
//             var updated = links.Update(linkAddress3, constants.Null, constants.Null);
//
//             Assert.True(updated == linkAddress3);
//
//             link3 = new Link<TLinkaddress>(links.GetLink(linkAddress3));
//
//             Assert.True(link3.Source == constants.Null);
//             Assert.True(link3.Target == constants.Null);
//
//             // Delete link
//             links.Delete(linkAddress3);
//
//             Assert.True(links.Count() == two);
//
//             var setter3 = new Setter<TLinkaddress>(constants.Null);
//             links.Each(constants.Any, constants.Any, setter3.SetAndReturnTrue);
//
//             Assert.True(setter3.Result == linkAddress2);
//         }
//
//         public static void TestMultipleRandomCreationsAndDeletions<TLinkAddress>(this ILinks<TLinkAddress> links, int maximumOperationsPerCycle) where TLinkAddress: struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool>
//         {
//             var comparer = Comparer<TLinkAddress>.Default;
//             for (var N = 1; N < maximumOperationsPerCycle; N++)
//             {
//                 var random = new System.Random(N);
//                 var created = 0UL;
//                 var deleted = 0UL;
//                 for (var i = 0; i < N; i++)
//                 {
//                     var linksCount = TLinkAddress.CreateTruncating(links.Count());
//                     var createPoint = random.NextBoolean();
//                     if (linksCount >= TLinkAddress.CreateTruncating(2) && createPoint)
//                     {
//                         var linksAddressRange = new Range<ulong>(1, ulong.CreateTruncating(linksCount));
//                         TLinkAddress source = TLinkAddress.CreateTruncating(random.NextUInt64(linksAddressRange));
//                         TLinkAddress target = TLinkAddress.CreateTruncating(random.NextUInt64(linksAddressRange)); //-V3086
//                         var resultLink = links.GetOrCreate(source, target);
//                         if (comparer.Compare(resultLink, TLinkAddress.CreateTruncating(linksCount)) > 0)
//                         {
//                             created++;
//                         }
//                     }
//                     else
//                     {
//                         links.Create();
//                         created++;
//                     }
//                 }
//                 Assert.True(created == (links.Count()));
//                 for (var i = 0; i < N; i++)
//                 {
//                     TLinkAddress link = TLinkAddress.CreateTruncating.Convert((ulong)i + 1UL);
//                     if (links.Exists(link))
//                     {
//                         links.Delete(link);
//                         deleted++;
//                     }
//                 }
//                 Assert.True(addressToUInt64Converter.Convert(links.Count()) == 0L);
//             }
//         }
//     }
// }
