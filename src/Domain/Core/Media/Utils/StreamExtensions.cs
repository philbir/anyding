namespace Anyding;

public static class StreamExtensions
{
    public static byte[] ToByteArray(this Stream stream)
    {
        if (stream is MemoryStream s)
        {
            return s.ToArray();
        }

        using var ms = new MemoryStream();
        stream.CopyTo(ms);

        return ms.ToArray();
    }
}
