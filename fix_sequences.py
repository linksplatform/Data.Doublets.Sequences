#!/usr/bin/env python3
"""
Script to fix issues in the converted Sequences.cs file.
"""

import re
import sys

def fix_sequences_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Fix comment formatting - restore /// comments
    content = re.sub(r'^    / ', '    /// ', content, flags=re.MULTILINE)
    
    # Fix malformed equality comparisons
    content = re.sub(r'if \(([^)]+) \.Equals\(([^)]+)\)', r'if (\1.Equals(\2))', content)
    content = re.sub(r'if \(([^)]+) !\.Equals\(([^)]+)\)', r'if (!\1.Equals(\2))', content)
    
    # Fix return type conversion issues
    content = re.sub(r'return 0;', 'return TLinkAddress.Zero;', content)
    content = re.sub(r'return 1UL;', 'return TLinkAddress.One;', content)
    
    # Fix lambda return type issue in Update method
    content = re.sub(r'return _sync\.DoWrite\(\(Func<ulong>\)\(() =>', r'return _sync.DoWrite(() =>', content)
    content = re.sub(r'ILinksExtensions\.EnsureLinkIsAnyOrExists<ulong>\(Links, \(IList<TLinkAddress>\)sequence\);', r'Links.EnsureLinkExists(sequence);', content)
    content = re.sub(r'return UpdateCore\(sequence, newSequence\);', r'return UpdateCore(sequence, newSequence, handler);', content)
    
    # Fix comparison operators for generic types
    content = re.sub(r'if \(([^=]+) == ([^)]+)\)', r'if (\1.Equals(\2))', content)
    content = re.sub(r'if \(([^!=]+) != ([^)]+)\)', r'if (!\1.Equals(\2))', content)
    content = re.sub(r'while \(([^!=]+) != ([^)]+) && ([^!=]+) != ([^)]+)\)', r'while (!\1.Equals(\2) && !\3.Equals(\4))', content)
    
    # Fix specific method calls
    content = re.sub(r'CompactCore\(sequence\) => UpdateCore\(sequence, sequence\);', r'CompactCore(IList<TLinkAddress> sequence) => UpdateCore(sequence, sequence, null);', content)
    
    # Fix missing method signatures and parameters
    content = re.sub(r'UpdateOneCore\(variant, bestVariant\);', r'UpdateOneCore(variant, bestVariant, handler);', content)
    
    # Fix generic parameter casts
    content = re.sub(r'\(LinkAddress<TLinkAddress>\)Constants\.Any', r'new LinkAddress<TLinkAddress>(Constants.Any)', content)
    
    # Write back to file
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 fix_sequences.py <path_to_sequences.cs>")
        sys.exit(1)
    
    file_path = sys.argv[1]
    fix_sequences_file(file_path)
    print(f"Fixed {file_path}")