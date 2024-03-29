using System.Text;
using Platform.Reflection;

namespace Platform.Data.Doublets.Sequences;

public class TestExtensions
{
    public static string PrettifyBinary<TLinkAddress>  (string binaryRepresentation) 
    {
        var bitsCount = NumericType<TLinkAddress>.BitsSize;
        var sb = new StringBuilder().Append('0', bitsCount - binaryRepresentation.Length).Append(binaryRepresentation).ToString();
        for (var i = 4; i < sb.Length; i += 5)
        {
            sb = sb.Insert(i, " ");
        }
        return sb.ToString();
    }
}
