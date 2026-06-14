using Visual.Capture;

namespace Visual.Output;

public interface IFrameSink
{
    void Submit(VideoFrame frame);
}
