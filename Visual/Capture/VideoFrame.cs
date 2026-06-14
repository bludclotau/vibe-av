namespace Visual.Capture;

/// <summary>
/// RGBA32 frame container. Caller must Dispose().
/// </summary>
public sealed class VideoFrame : IDisposable
{
    public readonly int Width;
    public readonly int Height;
    public readonly byte[] Pixels; // RGBA32

    public VideoFrame(int width, int height)
    {
        Width  = width;
        Height = height;
        Pixels = new byte[width * height * 4];
    }

    public void Dispose()
    {
        // No unmanaged resources yet
    }
}
