using AVEngine.Shared;
using AVEngine.Core.Audio;

namespace Vibe.Core;

public class AudioEngineLoop
{
    private readonly MediaProject _project;
    private readonly VibeState _vibeState;
    private readonly float[] _trackBuffer;
    private readonly IMediaMixer _mediaMixer;
    private TimeSpan _transportPosition;
    private const int SampleRate = 44100;

    public AudioEngineLoop(MediaProject project, VibeState vibeState,
                           int maxBufferSize, IMediaMixer mixer)
    {
        _project = project;
        _vibeState = vibeState;
        _trackBuffer = new float[maxBufferSize];
        _mediaMixer = mixer;
    }

    public void OnAudioCallback(float[] buffer, int numSamples, int numChannels)
    {
        int totalSamples = numSamples * numChannels;
        Array.Clear(buffer, 0, totalSamples);

        foreach (var track in _project.Tracks)
        {
            if (track.IsMuted) continue;

            Array.Clear(_trackBuffer, 0, totalSamples);

            _mediaMixer.RenderTrack(track, _trackBuffer, numSamples, numChannels,
                                     _transportPosition, _vibeState);

            ApplyLiveModulations(track, _vibeState);

            foreach (var dsp in track.AudioDspChain)
            {
                dsp.Process(_trackBuffer, _trackBuffer, numSamples, numChannels);
            }

            MixIn(buffer, _trackBuffer, totalSamples);
        }

        _vibeState.CurrentAudioRms = CalculateRms(buffer, totalSamples);
        _transportPosition += SamplesToTime(numSamples);
    }

    private void ApplyLiveModulations(MediaTrack track, VibeState state)
    {
        if (track.Tag is GranularDspNode granular)
        {
            granular.GrainSizeMs = state.TargetGrainSize;
            granular.PositionJitterMs = state.TargetJitter;
        }
    }

    private static void MixIn(float[] master, float[] track, int totalSamples)
    {
        for (int i = 0; i < totalSamples; i++)
        {
            master[i] += track[i];
        }
    }

    private static float CalculateRms(float[] buffer, int totalSamples)
    {
        double sumSq = 0;
        for (int i = 0; i < totalSamples; i++)
        {
            sumSq += buffer[i] * buffer[i];
        }
        return (float)Math.Sqrt(sumSq / totalSamples);
    }

    private static TimeSpan SamplesToTime(int numSamples)
    {
        return TimeSpan.FromSeconds((double)numSamples / SampleRate);
    }
}
