using AVEngine.Shared;
using Visual.Capture;

namespace Visual.Processing;

public class EffectChain
{
    private readonly List<IVisualEffect> _effects = new();

    public void Add(IVisualEffect fx) => _effects.Add(fx);

    public VideoFrame Process(VideoFrame frame, VibeState vibe)
    {
        var current = frame;

        foreach (var fx in _effects)
        {
            if (!fx.Enabled) continue;
            current = fx.Process(current, vibe);
        }

        return current;
    }
}
