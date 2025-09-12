using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Bit Size Analysis ===");
        Console.WriteLine($"byte bits: {sizeof(byte) * 8}");
        Console.WriteLine($"ushort bits: {sizeof(ushort) * 8}");
        Console.WriteLine($"uint bits: {sizeof(uint) * 8}");
        Console.WriteLine($"ulong bits: {sizeof(ulong) * 8}");
        
        Console.WriteLine("\n=== Current Logic (BitsSize - 1) ===");
        Console.WriteLine($"byte shift: {sizeof(byte) * 8 - 1}");
        Console.WriteLine($"ushort shift: {sizeof(ushort) * 8 - 1}");
        Console.WriteLine($"uint shift: {sizeof(uint) * 8 - 1}");
        Console.WriteLine($"ulong shift: {sizeof(ulong) * 8 - 1}");
        
        Console.WriteLine("\n=== Bit Masks (MaxValue >> 1) ===");
        Console.WriteLine($"byte mask: {(byte.MaxValue >> 1):X} ({Convert.ToString((byte.MaxValue >> 1), 2).PadLeft(8, '0')})");
        Console.WriteLine($"ushort mask: {(ushort.MaxValue >> 1):X} ({Convert.ToString((ushort.MaxValue >> 1), 2).PadLeft(16, '0')})");
        Console.WriteLine($"uint mask: {(uint.MaxValue >> 1):X} ({Convert.ToString((uint.MaxValue >> 1), 2).PadLeft(32, '0')})");
        Console.WriteLine($"ulong mask: {(ulong.MaxValue >> 1):X}");
        
        Console.WriteLine("\n=== Problem Demonstration ===");
        
        // For byte (8 bits), we want 7 usable bits
        // Current: shift by 7, mask with 7F (0111 1111)
        Console.WriteLine("Byte example:");
        byte testByte = 200; // 1100 1000
        Console.WriteLine($"  Original: {testByte} ({Convert.ToString(testByte, 2).PadLeft(8, '0')})");
        Console.WriteLine($"  Masked: {(testByte & (byte.MaxValue >> 1))} ({Convert.ToString((testByte & (byte.MaxValue >> 1)), 2).PadLeft(8, '0')})");
        Console.WriteLine($"  After shift by 7: {(testByte >> 7)} (should be 1, but we lose data)");
        
        // For uint (32 bits), we want 31 usable bits
        // Current: shift by 31, mask with 7FFFFFFF
        Console.WriteLine("\nUint example:");
        uint testUint = 0x80000001; // 1000 0000 0000 0000 0000 0000 0000 0001
        Console.WriteLine($"  Original: {testUint:X} ({Convert.ToString(testUint, 2).PadLeft(32, '0')})");
        Console.WriteLine($"  Masked: {(testUint & (uint.MaxValue >> 1)):X}");
        Console.WriteLine($"  After shift by 31: {(testUint >> 31)} (should be 1, but we lose data)");
    }
}
