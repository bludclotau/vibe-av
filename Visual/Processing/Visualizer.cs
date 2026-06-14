using AVEngine.Shared;
using Visual.Capture;

namespace Visual.Processing;

/// <summary>
/// Draws RMS bar + grain envelope strip over the frame.
/// </summary>
public class Visualizer : IVisualEffect
{
    public bool Enabled { get; set; } = true;

    public VideoFrame Process(VideoFrame frame, VibeState vibe)
    {
        // TODO: Draw RMS bar + envelope strip
        return frame;
    }
}
