using Visual.Capture;

namespace Visual.Output;

/// <summary>
/// SDL2 window stub. TODO: Implement pixel blit.
/// </summary>
public class SdlWindow : IFrameSink
{
    public void Submit(VideoFrame frame)
    {
        // TODO: SDL2 blit
    }
}
