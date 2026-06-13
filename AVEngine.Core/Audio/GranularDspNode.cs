using AVEngine.Shared;
namespace AVEngine.Core.Audio;

// ─── Envelope types ──────────────────────────────────────────────────────────

public enum GrainEnvelopeType { Hann, Hamming, Blackman, Sine, Triangle, Gaussian }

// ─── Node ────────────────────────────────────────────────────────────────────

public class GranularDspNode : IDspNode
{
    public string Name => "Granular";

    // === Parameters (written by VibeState / MotionController) ===
    public float             GrainSizeMs      = 80f;
    public float             PositionJitterMs = 10f;
    public float             Density          = 20f;   // grains/sec
    public float             PitchSemitones   = 0f;
    public GrainEnvelopeType EnvelopeType     = GrainEnvelopeType.Hann;

    public float GrainRate { get => Density; set => Density = value; }

    // === Internal state ===
    private const int MaxGrains = 128;
    private readonly Grain[] _grains = new Grain[MaxGrains];
    private float _sampleRate         = 44100f;
    private float _timeSinceLastSpawn = 0f;

    private readonly Dictionary<GrainEnvelopeType, float[]> _envelopes = new();

    public GranularDspNode()
    {
        for (int i = 0; i < MaxGrains; i++) _grains[i] = new Grain();
        BuildAllEnvelopes(2048);
    }

    public void SetSampleRate(float sr) => _sampleRate = sr;

    public void SyncFromVibe(VibeState v)
    {
        GrainSizeMs      = v.TargetGrainSize;
        PositionJitterMs = v.TargetJitter;
        Density          = v.TargetGrainRate;
        PitchSemitones   = v.TargetPitch;
        EnvelopeType     = v.TargetEnvelope;
    }

    public void Process(float[] input, float[] output, int numSamples, int numChannels)
    {
        Array.Clear(output, 0, numSamples * numChannels);
        SpawnGrains(numSamples);
        for (int i = 0; i < MaxGrains; i++)
            if (_grains[i].Active)
                ProcessGrain(_grains[i], input, output, numSamples, numChannels);
    }

    private void SpawnGrains(int numSamples)
    {
        _timeSinceLastSpawn += numSamples / _sampleRate;
        float interval = 1f / MathF.Max(Density, 0.01f);
        while (_timeSinceLastSpawn >= interval)
        {
            TrySpawnGrain();
            _timeSinceLastSpawn -= interval;
        }
    }

    private void TrySpawnGrain()
    {
        for (int i = 0; i < MaxGrains; i++)
            if (!_grains[i].Active) { InitGrain(_grains[i]); return; }
    }

    private void InitGrain(Grain g)
    {
        g.Active        = true;
        g.LengthSamples = (int)(_sampleRate * GrainSizeMs / 1000f);
        float jitterSmp = PositionJitterMs / 1000f * _sampleRate;
        g.StartPosition = jitterSmp * (Random.Shared.NextSingle() * 2f - 1f);
        g.Position      = g.StartPosition;
        g.PitchRatio    = MathF.Pow(2f, PitchSemitones / 12f);
        g.Age           = 0;
        g.Envelope      = _envelopes[EnvelopeType];
    }

    private static void ProcessGrain(Grain g, float[] input, float[] output,
                                        int numSamples, int numChannels)
    {
        int inputFrames = input.Length / numChannels;

        for (int n = 0; n < numSamples; n++)
        {
            if (g.Age >= g.LengthSamples) { g.Active = false; break; }

            float readPos = g.StartPosition + g.Age * g.PitchRatio;
            int   idx     = (int)readPos;
            if (idx < 0 || idx >= inputFrames) { g.Active = false; break; }

            float t      = g.Age / (float)(g.LengthSamples - 1);
            float window = g.Envelope[(int)(t * (g.Envelope.Length - 1))];
            float sample = input[idx * numChannels] * window;

            output[n * numChannels] += sample;
            if (numChannels > 1) output[n * numChannels + 1] += sample;

            g.Age++;
        }
    }

    private void BuildAllEnvelopes(int size)
    {
        _envelopes[GrainEnvelopeType.Hann]     = BuildHann(size);
        _envelopes[GrainEnvelopeType.Hamming]  = BuildHamming(size);
        _envelopes[GrainEnvelopeType.Blackman] = BuildBlackman(size);
        _envelopes[GrainEnvelopeType.Sine]     = BuildSine(size);
        _envelopes[GrainEnvelopeType.Triangle] = BuildTriangle(size);
        _envelopes[GrainEnvelopeType.Gaussian] = BuildGaussian(size);
    }

    private static float[] BuildHann(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
            w[i] = 0.5f * (1f - MathF.Cos(2f * MathF.PI * i / (size - 1)));
        return w;
    }

    private static float[] BuildHamming(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
            w[i] = 0.54f - 0.46f * MathF.Cos(2f * MathF.PI * i / (size - 1));
        return w;
    }

    private static float[] BuildBlackman(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
        {
            float phase = 2f * MathF.PI * i / (size - 1);
            w[i] = 0.42f - 0.5f * MathF.Cos(phase) + 0.08f * MathF.Cos(2f * phase);
        }
        return w;
    }

    private static float[] BuildSine(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
            w[i] = MathF.Sin(MathF.PI * i / (size - 1));
        return w;
    }

    private static float[] BuildTriangle(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
        {
            float t = i / (float)(size - 1);
            w[i] = 1f - MathF.Abs(2f * t - 1f);
        }
        return w;
    }

    private static float[] BuildGaussian(int size, float sigma = 0.4f)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
        {
            float t = (i - (size - 1) * 0.5f) / ((size - 1) * 0.5f);
            w[i] = MathF.Exp(-0.5f * t * t / (sigma * sigma));
        }
        return w;
    }

    private class Grain
    {
        public bool    Active;
        public int     Age;
        public int     LengthSamples;
        public float   StartPosition;
        public float   Position;
        public float   PitchRatio;
        public float[] Envelope = Array.Empty<float>();
    }
}
