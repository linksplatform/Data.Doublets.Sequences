// using Xunit;
// using Platform.Random;
// using Platform.Data.Doublets.Numbers.Unary;
//
// namespace Platform.Data.Doublets.Sequences.Tests
// {
//     public static class UnaryNumberConvertersTests
//     {
//         [Fact]
//         public static void ConvertersTest()
//         {
//             using (var scope = new TempLinksTestScope())
//             {
//                 const int N = 10;
//                 var links = scope.Links;
//                 var meaningRoot = links.CreatePoint();
//                 var powerOf2ToUnaryNumberConverter = new PowerOf2ToUnaryNumberConverter<ulong>(links, TLinkAddress.One);
//                 var toUnaryNumberConverter = new AddressToUnaryNumberConverter<ulong>(links, powerOf2ToUnaryNumberConverter);
//                 var random = new System.Random(0);
//                 ulong[] numbers = new ulong[N];
//                 ulong[] unaryNumbers = new ulong[N];
//                 for (int i = 0; i < N; i++)
//                 {
//                     numbers[i] = random.NextUInt64();
//                     unaryNumbers[i] = toUnaryNumberConverter.Convert(numbers[i]);
//                 }
//                 var fromUnaryNumberConverterUsingOrOperation = new UnaryNumberToAddressOrOperationConverter<ulong>(links, powerOf2ToUnaryNumberConverter);
//                 var fromUnaryNumberConverterUsingAddOperation = new UnaryNumberToAddressAddOperationConverter<ulong>(links, TLinkAddress.One);
//                 for (int i = 0; i < N; i++)
//                 {
//                     Assert.Equal(numbers[i], fromUnaryNumberConverterUsingOrOperation.Convert(unaryNumbers[i]));
//                     Assert.Equal(numbers[i], fromUnaryNumberConverterUsingAddOperation.Convert(unaryNumbers[i]));
//                 }
//             }
//         }
//     }
// }
