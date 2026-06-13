using AVEngine.Shared;

namespace AVEngine.Engine;

public class MediaMixer : IMediaMixer
{
    public void RenderTrack(MediaTrack track, float[] buffer, int numSamples,
                            int numChannels, TimeSpan transportPosition, VibeState vibeState)
    {
    }
}
