using System;

namespace Visual.Capture;

/// <summary>
/// Animated plasma test pattern. Works without hardware.
/// </summary>
public class TestPatternSource : IFrameSource
{
    private int _t = 0;

    public VideoFrame GetFrame()
    {
        int w = 640, h = 360;
        var f = new VideoFrame(w, h);

        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            float v = (float)(
                Math.Sin(x * 0.03 + _t * 0.05) +
                Math.Sin(y * 0.04 + _t * 0.03)
            );

            byte c = (byte)((v * 0.5 + 0.5) * 255);

            int idx = (y * w + x) * 4;
            f.Pixels[idx + 0] = c;
            f.Pixels[idx + 1] = (byte)(255 - c);
            f.Pixels[idx + 2] = (byte)(c / 2);
            f.Pixels[idx + 3] = 255;
        }

        _t++;
        return f;
    }
}
