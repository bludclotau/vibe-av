using System.Text.Json;
using AVEngine.Shared;

namespace Vibe.Core;

public class VibeBridge
{
    private readonly VibeState _vibeState;
    private readonly MediaProject _project;

    public VibeBridge(VibeState state, MediaProject project)
    {
        _vibeState = state;
        _project = project;
    }

    public void HandleIncomingVibeJson(string jsonString)
    {
        using var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;

        if (root.TryGetProperty("audio", out var audio))
        {
            if (audio.TryGetProperty("granular", out var granular))
            {
                if (granular.TryGetProperty("grainSize", out var gs))
                    _vibeState.TargetGrainSize = gs.GetSingle();
                if (granular.TryGetProperty("jitter", out var jt))
                    _vibeState.TargetJitter = jt.GetSingle();
            }
        }

        if (root.TryGetProperty("video", out var video))
        {
            if (video.TryGetProperty("effect", out var ef))
                _vibeState.ActiveVideoEffect = ef.GetString() ?? "none";
            if (video.TryGetProperty("intensity", out var vi))
                _vibeState.VisualIntensity = vi.GetSingle();
        }
    }
}
