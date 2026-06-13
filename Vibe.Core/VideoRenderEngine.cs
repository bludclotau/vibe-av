using AVEngine.Shared;

namespace Vibe.Core;

public class VideoRenderEngine
{
    private readonly VibeState _vibeState;
    private readonly IShaderEffect _glitchShader;
    private readonly IShaderEffect _feedbackShader;

    public VideoRenderEngine(VibeState vibeState, IShaderEffect glitch, IShaderEffect feedback)
    {
        _vibeState = vibeState;
        _glitchShader = glitch;
        _feedbackShader = feedback;
    }

    public void OnVideoRenderFrame()
    {
        VibeSnapshot state = VibeSnapshot.From(_vibeState);
        float dynamicIntensity = state.VisualIntensity * (1.0f + state.CurrentAudioRms * 2.0f);

        switch (state.ActiveVideoEffect)
        {
            case "vhs_glitch":
                _glitchShader.SetFloat("Intensity", dynamicIntensity);
                _glitchShader.Render();
                break;
            case "feedback_loop":
                _feedbackShader.SetFloat("FeedbackAmount", dynamicIntensity);
                _feedbackShader.Render();
                break;
            default:
                RenderCleanVideo();
                break;
        }
    }

    private void RenderCleanVideo()
    {
    }
}
