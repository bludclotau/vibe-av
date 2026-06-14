using AVEngine.Shared;
using Visual.Capture;

namespace Visual.Processing.Effects;

public class GlitchEffect : IVisualEffect
{
    public bool Enabled { get; set; } = true;

    public VideoFrame Process(VideoFrame frame, VibeState vibe)
    {
        // TODO: Horizontal row shift based on RMS
        return frame;
    }
}
