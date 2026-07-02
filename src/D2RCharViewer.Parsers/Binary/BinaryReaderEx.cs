using System.Text;

namespace D2RCharViewer.Parsers.Binary;

public class BinaryReaderEx : BinaryReader
{
    public BinaryReaderEx(Stream input) : base(input) { }
    public BinaryReaderEx(Stream input, Encoding encoding) : base(input, encoding) { }

    /// <summary>
    /// Reads a bit value from current position
    /// </summary>
    public bool ReadBit(ref int bitOffset)
    {
        int currentByte = PeekChar();
        if (currentByte == -1)
            throw new EndOfStreamException();

        bool value = ((currentByte >> bitOffset) & 1) == 1;
        bitOffset++;

        if (bitOffset == 8)
        {
            ReadByte();
            bitOffset = 0;
        }

        return value;
    }

    /// <summary>
    /// Reads a bit-packed unsigned integer
    /// </summary>
    public uint ReadBits(int numBits, ref int bitOffset)
    {
        uint result = 0;
        for (int i = 0; i < numBits; i++)
        {
            if (ReadBit(ref bitOffset))
                result |= (uint)(1 << i);
        }
        return result;
    }

    /// <summary>
    /// Reads a null-terminated ASCII string
    /// </summary>
    public string ReadCString(int maxLength = 256)
    {
        var bytes = new List<byte>();
        byte b;
        int count = 0;
        while ((b = ReadByte()) != 0 && count < maxLength)
        {
            bytes.Add(b);
            count++;
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }
}
