using AVEngine.Shared;
using Visual.Capture;

namespace Visual.Processing;

public interface IVisualEffect
{
    bool Enabled { get; set; }
    VideoFrame Process(VideoFrame frame, VibeState vibe);
}
