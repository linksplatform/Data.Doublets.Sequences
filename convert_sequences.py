#!/usr/bin/env python3
"""
Script to convert the commented Sequences.cs file to generic version.
Converts LinkIndex to TLinkAddress and uncomments the code.
"""

import re
import sys

def convert_sequences_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Split content at the point where conversion is needed
    lines = content.split('\n')
    
    # Process each line
    converted_lines = []
    for line in lines:
        # Skip already converted lines (those without comment prefix)
        if not line.lstrip().startswith('//'):
            converted_lines.append(line)
            continue
            
        # Remove comment prefix
        if line.strip() == '//':
            converted_lines.append('')
            continue
        
        # Handle commented lines
        if line.lstrip().startswith('//'):
            uncommented = line.replace('//', '', 1)
            
            # Convert LinkIndex to TLinkAddress
            uncommented = re.sub(r'\bLinkIndex\b', 'TLinkAddress', uncommented)
            
            # Convert ulong to TLinkAddress in specific contexts
            uncommented = re.sub(r'new Link<ulong>', 'new Link<TLinkAddress>', uncommented)
            uncommented = re.sub(r'IList<ulong>', 'IList<TLinkAddress>', uncommented)
            
            # Fix equality comparisons for generic types
            uncommented = re.sub(r'== Options\.SequenceMarkerLink', '.Equals(Options.SequenceMarkerLink)', uncommented)
            uncommented = re.sub(r'!= Options\.SequenceMarkerLink', '!.Equals(Options.SequenceMarkerLink)', uncommented)
            uncommented = re.sub(r'== Constants\.', '.Equals(Constants.', uncommented)
            uncommented = re.sub(r'!= Constants\.', '!.Equals(Constants.', uncommented)
            
            # Fix method call issues that might arise from generic constraints
            uncommented = re.sub(r'ZeroOrMany = LinkIndex\.MaxValue', 'ZeroOrMany = TLinkAddress.CreateSaturating(ulong.MaxValue)', uncommented)
            
            converted_lines.append(uncommented)
        else:
            converted_lines.append(line)
    
    # Write back to file
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write('\n'.join(converted_lines))

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 convert_sequences.py <path_to_sequences.cs>")
        sys.exit(1)
    
    file_path = sys.argv[1]
    convert_sequences_file(file_path)
    print(f"Converted {file_path}")