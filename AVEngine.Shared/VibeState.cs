using AVEngine.Core.Audio;
namespace AVEngine.Shared;

public class VibeState
{
    public float CurrentAudioRms;

    public volatile float            TargetGrainSize     = 80f;
    public volatile float            TargetJitter        = 10f;
    public volatile float            TargetGrainRate     = 20f;
    public volatile float            TargetPitch         = 0f;
    public          GrainEnvelopeType TargetEnvelope     = GrainEnvelopeType.Hann;

    public volatile string ActiveVideoEffect = "none";
    public volatile float  VisualIntensity   = 0f;
}

public struct VibeSnapshot
{
    public float             CurrentAudioRms;
    public float             TargetGrainSize;
    public float             TargetJitter;
    public GrainEnvelopeType TargetEnvelope;
    public string            ActiveVideoEffect;
    public float             VisualIntensity;

    public static VibeSnapshot From(VibeState s) => new()
    {
        CurrentAudioRms   = s.CurrentAudioRms,
        TargetGrainSize   = s.TargetGrainSize,
        TargetJitter      = s.TargetJitter,
        TargetEnvelope    = s.TargetEnvelope,
        ActiveVideoEffect = s.ActiveVideoEffect,
        VisualIntensity   = s.VisualIntensity,
    };
}
