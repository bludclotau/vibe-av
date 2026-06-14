using AVEngine.Shared;
using Visual.Capture;

namespace Visual.Processing.Effects;

public class FeedbackEffect : IVisualEffect
{
    public bool Enabled { get; set; } = true;

    private VideoFrame? _prev;

    public VideoFrame Process(VideoFrame frame, VibeState vibe)
    {
        // TODO: Frame echo based on vibe.VisualIntensity
        _prev = frame;
        return frame;
    }
}
