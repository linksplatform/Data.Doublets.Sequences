#!/usr/bin/env python3
"""
Complete conversion script for Sequences.cs to make it generic.
This script processes the entire file more carefully than the previous version.
"""

import re

def convert_sequences_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Apply conversions line by line to avoid issues
    lines = content.split('\n')
    result_lines = []
    
    for line in lines:
        # Skip lines that are already processed (not commented out)
        if not line.strip().startswith('//'):
            result_lines.append(line)
            continue
        
        # Remove comment prefix for processing
        if line.strip() == '//':
            result_lines.append('')
            continue
            
        # Process commented lines
        uncommented = line[2:]  # Remove the first // only
        
        # Basic type conversions
        uncommented = re.sub(r'\bLinkIndex\b', 'TLinkAddress', uncommented)
        
        # Fix specific type conversions
        uncommented = re.sub(r'new Link<ulong>', 'new Link<TLinkAddress>', uncommented)
        uncommented = re.sub(r'IList<ulong>', 'IList<TLinkAddress>', uncommented)
        
        # Fix returns for generic types  
        uncommented = re.sub(r'\breturn 0;', 'return TLinkAddress.Zero;', uncommented)
        uncommented = re.sub(r'\breturn 1UL;', 'return TLinkAddress.One;', uncommented)
        
        # Fix method calls for ClearGarbage - add if not present
        if 'ClearGarbage(' in uncommented and 'void ClearGarbage(' not in uncommented:
            pass  # Will handle ClearGarbage method separately
        
        result_lines.append(uncommented)
    
    # Join all lines back together
    converted_content = '\n'.join(result_lines)
    
    # Add missing ClearGarbage method at the end of the class (before closing brace)
    clear_garbage_method = '''
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearGarbage(TLinkAddress element)
        {
            // Implementation for garbage collection of individual elements
            // This method should be implemented based on the specific garbage collection strategy
        }'''
    
    # Insert the method before the last closing braces
    converted_content = converted_content.replace(
        '        #endregion\n    }\n}',
        '        #endregion\n' + clear_garbage_method + '\n    }\n}'
    )
    
    # Write the result back
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(converted_content)

if __name__ == "__main__":
    file_path = './csharp/Platform.Data.Doublets.Sequences/Sequences.cs'
    convert_sequences_file(file_path)
    print(f"Conversion completed for {file_path}")