using AVEngine.Shared;
using AVEngine.Core.Audio;
using AVEngine.Engine;
using Vibe.Core;

var project = new MediaProject();
var track = new MediaTrack { Name = "Main" };
project.Tracks.Add(track);

var vibeState = new VibeState();
var mixer = new DummyMixer();
var glitchShader = new DummyGlitchShader();
var feedbackShader = new DummyFeedbackShader();

var granular = new GranularDspNode();
track.Tag = granular;
track.AudioDspChain.Add(granular);

var audioEngine = new AudioEngineLoop(project, vibeState, 2048, mixer);
var videoEngine = new VideoRenderEngine(vibeState, glitchShader, feedbackShader);
var motionController = new MotionController(vibeState);
var vibeBridge = new VibeBridge(vibeState, project);

var audioBuffer = new float[512 * 2];

string testJson = """
{"audio":{"granular":{"grainSize":60,"jitter":5}},"video":{"effect":"vhs_glitch","intensity":0.8}}
""";

for (int i = 0; i < 100; i++)
{
    var features = new MotionFeatures
    {
        HandX = 0.5f,
        HandY = 0.3f + i * 0.002f,
        MotionSpeed = 0.2f + i * 0.001f,
        SilhouetteArea = 0.4f
    };
    motionController.UpdateFromFrame(features);

    if (i == 5)
        vibeBridge.HandleIncomingVibeJson(testJson);

    audioEngine.OnAudioCallback(audioBuffer, 512, 2);
    videoEngine.OnVideoRenderFrame();

    Console.WriteLine($"Iter {i}: RMS={vibeState.CurrentAudioRms:F6}, Effect={vibeState.ActiveVideoEffect}");
}

class DummyMixer : IMediaMixer
{
    public void RenderTrack(MediaTrack track, float[] buffer, int numSamples,
                            int numChannels, TimeSpan transportPosition, VibeState vibeState)
    {
    }
}

class DummyGlitchShader : IShaderEffect
{
    public void SetFloat(string uniformName, float value)
    {
        Console.WriteLine($"[Glitch] {uniformName} = {value:F3}");
    }

    public void Render()
    {
        Console.WriteLine("[Glitch] Render");
    }
}

class DummyFeedbackShader : IShaderEffect
{
    public void SetFloat(string uniformName, float value)
    {
        Console.WriteLine($"[Feedback] {uniformName} = {value:F3}");
    }

    public void Render()
    {
        Console.WriteLine("[Feedback] Render");
    }
}
