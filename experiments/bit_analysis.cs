using System;
using Platform.Reflection;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Bit Size Analysis ===");
        Console.WriteLine($"byte BitsSize: {NumericType<byte>.BitsSize}");
        Console.WriteLine($"ushort BitsSize: {NumericType<ushort>.BitsSize}");
        Console.WriteLine($"uint BitsSize: {NumericType<uint>.BitsSize}");
        Console.WriteLine($"ulong BitsSize: {NumericType<ulong>.BitsSize}");
        
        Console.WriteLine("\n=== Current Logic (BitsSize - 1) ===");
        Console.WriteLine($"byte shift: {NumericType<byte>.BitsSize - 1}");
        Console.WriteLine($"ushort shift: {NumericType<ushort>.BitsSize - 1}");
        Console.WriteLine($"uint shift: {NumericType<uint>.BitsSize - 1}");
        Console.WriteLine($"ulong shift: {NumericType<ulong>.BitsSize - 1}");
        
        Console.WriteLine("\n=== Bit Masks (MaxValue >> 1) ===");
        Console.WriteLine($"byte mask: {(NumericType<byte>.MaxValue >> 1):X} ({Convert.ToString((NumericType<byte>.MaxValue >> 1), 2).PadLeft(8, '0')})");
        Console.WriteLine($"ushort mask: {(NumericType<ushort>.MaxValue >> 1):X} ({Convert.ToString((NumericType<ushort>.MaxValue >> 1), 2).PadLeft(16, '0')})");
        Console.WriteLine($"uint mask: {(NumericType<uint>.MaxValue >> 1):X} ({Convert.ToString((NumericType<uint>.MaxValue >> 1), 2).PadLeft(32, '0')})");
        Console.WriteLine($"ulong mask: {(NumericType<ulong>.MaxValue >> 1):X} ({Convert.ToString((long)(NumericType<ulong>.MaxValue >> 1), 2).PadLeft(64, '0')})");
    }
}