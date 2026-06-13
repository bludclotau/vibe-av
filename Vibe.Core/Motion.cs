namespace AVEngine.Shared;

public enum InputSourceSignal
{
    HandX,
    HandY,
    MotionSpeed,
    SilhouetteArea
}

public class SignalMapping
{
    public InputSourceSignal Source { get; set; }
    public float MinIn { get; set; }
    public float MaxIn { get; set; }
    public float MinOut { get; set; }
    public float MaxOut { get; set; }
}

public struct MotionFeatures
{
    public float HandX;
    public float HandY;
    public float MotionSpeed;
    public float SilhouetteArea;
}

public class MotionController
{
    private readonly VibeState _vibe;

    public Dictionary<string, SignalMapping> AudioMappings { get; } = new();
    public Dictionary<string, SignalMapping> VideoMappings { get; } = new();

    public MotionController(VibeState vibe)
    {
        _vibe = vibe;

        AudioMappings["grainSize"] = new SignalMapping
        {
            Source = InputSourceSignal.HandY,
            MinIn = 0f,
            MaxIn = 1f,
            MinOut = 20f,
            MaxOut = 200f
        };

        AudioMappings["jitter"] = new SignalMapping
        {
            Source = InputSourceSignal.MotionSpeed,
            MinIn = 0f,
            MaxIn = 1f,
            MinOut = 0f,
            MaxOut = 0.8f
        };
    }

    public void UpdateFromFrame(MotionFeatures features)
    {
        _vibe.TargetGrainSize = ProcessMapping(AudioMappings["grainSize"], features);
        _vibe.TargetJitter = ProcessMapping(AudioMappings["jitter"], features);
    }

    private static float ProcessMapping(SignalMapping map, MotionFeatures f)
    {
        float raw = map.Source switch
        {
            InputSourceSignal.HandX => f.HandX,
            InputSourceSignal.HandY => f.HandY,
            InputSourceSignal.MotionSpeed => f.MotionSpeed,
            InputSourceSignal.SilhouetteArea => f.SilhouetteArea,
            _ => 0f
        };

        float t = (raw - map.MinIn) / (map.MaxIn - map.MinIn);
        t = Math.Clamp(t, 0f, 1f);
        return map.MinOut + t * (map.MaxOut - map.MinOut);
    }
}
