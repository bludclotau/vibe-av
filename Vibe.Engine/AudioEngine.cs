using AVEngine.Core.Audio;
using AVEngine.Shared;

namespace AVEngine.Engine;

public class AudioEngine
{
    public MediaProject Project { get; } = new();
    public TimeSpan Position { get; private set; }
    public bool Playing { get; private set; }

    private readonly MediaMixer _mixer = new();
    private readonly float[] _trackBuf = new float[512 * 2];
    private VibeState _vibe = new();

    private float _sampleRate = 44100f;

    public void SetVibeState(VibeState v) => _vibe = v;
    public void SetSampleRate(float sr) => _sampleRate = sr;

    public void Play() => Playing = true;
    public void Stop() { Playing = false; Position = TimeSpan.Zero; }
    public void Pause() => Playing = false;

    public void OnAudioCallback(float[] buffer, int numSamples, int numChannels)
    {
        Array.Clear(buffer, 0, buffer.Length);
        if (!Playing) return;

        foreach (var track in Project.Tracks)
        {
            if (track.IsMuted) continue;

            Array.Clear(_trackBuf, 0, _trackBuf.Length);

            _mixer.RenderTrack(track, _trackBuf, numSamples, numChannels, Position, _vibe);

            foreach (var node in track.AudioDspChain)
            {
                if (node is GranularDspNode g)
                {
                    g.SetSampleRate(_sampleRate);
                    g.SyncFromVibe(_vibe);
                }
            }

            foreach (var node in track.AudioDspChain)
                node.Process(_trackBuf, _trackBuf, numSamples, numChannels);

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] += _trackBuf[i];
        }

        _vibe.CurrentAudioRms = CalculateRms(buffer, numSamples * numChannels);
        Position += TimeSpan.FromSeconds((double)numSamples / _sampleRate);
    }

    private static float CalculateRms(float[] buffer, int count)
    {
        float sum = 0f;
        for (int i = 0; i < count; i++)
            sum += buffer[i] * buffer[i];

        return MathF.Sqrt(sum / count);
    }

    public MediaTrack? FindTrack(string name) =>
        Project.Tracks.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public MediaTrack? FindTrack(Guid id) =>
        Project.Tracks.FirstOrDefault(t => t.Id == id);
}
