using AVEngine.Shared;
using Visual.Capture;

namespace Visual.Processing.Effects;

public class ChromaShiftEffect : IVisualEffect
{
    public bool Enabled { get; set; } = true;

    public VideoFrame Process(VideoFrame frame, VibeState vibe)
    {
        // TODO: RGB channel offset based on jitter
        return frame;
    }
}
