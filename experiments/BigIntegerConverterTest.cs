using System;
using System.Collections.Generic;
using System.Numerics;
using Platform.Converters;
using Platform.Data.Doublets.Decorators;
using Platform.Data.Doublets.Sequences.Numbers.Raw;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Memory;

namespace BigIntegerConverterTest
{
    class MockAddressToNumberConverter<T> : IConverter<T> where T : struct
    {
        public T Convert(T source) => source;
    }

    class MockListToSequenceConverter<T> : IConverter<IList<T>, T> 
        where T : struct, IUnsignedNumber<T>
    {
        public T Convert(IList<T> source)
        {
            // Simple mock - just return first element or zero
            return source.Count > 0 ? source[0] : T.Zero;
        }
    }

    class Program
    {
        static void TestConverter<T>() where T : struct, IUnsignedNumber<T>, IComparisonOperators<T, T, bool>, 
            IBitwiseOperators<T, T, T>, IShiftOperators<T, int, T>
        {
            Console.WriteLine($"\n=== Testing {typeof(T).Name} ===");
            
            var links = new UnitedMemoryLinks<T>(new HeapResizableDirectMemory());
            var addressToNumberConverter = new MockAddressToNumberConverter<T>();
            var listToSequenceConverter = new MockListToSequenceConverter<T>();
            var negativeMarker = T.One;

            var converter = new BigIntegerToRawNumberSequenceConverter<T>(
                links, addressToNumberConverter, listToSequenceConverter, negativeMarker);

            // Test different BigInteger values
            var testValues = new BigInteger[] { 
                0, 1, 127, 255, 256, 1024, 65535, 65536, 
                new BigInteger(uint.MaxValue), 
                new BigInteger(ulong.MaxValue) 
            };

            foreach (var value in testValues)
            {
                try 
                {
                    var result = converter.Convert(value);
                    Console.WriteLine($"  {value} -> {result} (OK)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {value} -> ERROR: {ex.Message}");
                }
            }
        }

        static void Main()
        {
            Console.WriteLine("Testing BigIntegerToRawNumberSequenceConverter with different numeric types:");

            try { TestConverter<byte>(); } catch (Exception ex) { Console.WriteLine($"byte test failed: {ex.Message}"); }
            try { TestConverter<ushort>(); } catch (Exception ex) { Console.WriteLine($"ushort test failed: {ex.Message}"); }
            try { TestConverter<uint>(); } catch (Exception ex) { Console.WriteLine($"uint test failed: {ex.Message}"); }
            try { TestConverter<ulong>(); } catch (Exception ex) { Console.WriteLine($"ulong test failed: {ex.Message}"); }
        }
    }
}