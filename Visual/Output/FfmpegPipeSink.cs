using System.Diagnostics;
using Visual.Capture;

namespace Visual.Output;

/// <summary>
/// Sends raw RGBA frames to ffmpeg via stdin.
/// </summary>
public class FfmpegPipeSink : IFrameSink
{
    private readonly Process _ffmpeg;

    public FfmpegPipeSink(string args)
    {
        _ffmpeg = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                RedirectStandardInput = true,
                UseShellExecute = false
            }
        };
        _ffmpeg.Start();
    }

    public void Submit(VideoFrame frame)
    {
        _ffmpeg.StandardInput.BaseStream.Write(frame.Pixels, 0, frame.Pixels.Length);
    }
}
