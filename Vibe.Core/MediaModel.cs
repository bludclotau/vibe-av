namespace AVEngine.Shared;

public interface IMediaMixer
{
    void RenderTrack(MediaTrack track, float[] buffer, int numSamples, int numChannels,
                     TimeSpan transportPosition, VibeState vibeState);
}

public interface IDspNode
{
    void Process(float[] input, float[] output, int numSamples, int numChannels);
}

public interface IVideoEffectNode
{
    void Render(VibeSnapshot state, float dynamicIntensity);
}

public interface IShaderEffect
{
    void SetFloat(string uniformName, float value);
    void Render();
}

public class MediaProject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<MediaTrack> Tracks { get; set; } = new();
    public double Tempo { get; set; } = 120.0;
    public TimeSpan PlaybackPosition { get; set; } = TimeSpan.Zero;
}

public class MediaTrack
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Track";
    public bool IsMuted { get; set; }
    public List<MediaClip> Clips { get; set; } = new();
    public List<IDspNode> AudioDspChain { get; set; } = new();
    public List<IVideoEffectNode> VideoChain { get; set; } = new();
    public object? Tag { get; set; }
}

public class MediaClip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Clip";
    public TimeSpan StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string? AudioFilePath { get; set; }
    public TimeSpan AudioOffset { get; set; }
    public string? VideoEffectType { get; set; }
    public Dictionary<string, float> VisualParameters { get; set; } = new();
}
