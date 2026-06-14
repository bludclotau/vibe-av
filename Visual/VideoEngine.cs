using AVEngine.Engine;
using Visual.Capture;
using Visual.Output;
using Visual.Processing;

namespace Visual;

public class VideoEngine
{
    private readonly AvEngine _audio;
    private readonly IFrameSource _source;
    private readonly EffectChain _effects = new();
    private readonly List<IFrameSink> _sinks = new();
    private readonly CancellationTokenSource _cts = new();

    public EffectChain Effects => _effects;

    public VideoEngine(AvEngine audio, IFrameSource source)
    {
        _audio  = audio;
        _source = source;
    }

    public void AddSink(IFrameSink sink) => _sinks.Add(sink);

    public void Start()
    {
        Task.Run(RunLoop, _cts.Token);
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    private async Task RunLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            using var frame = _source.GetFrame();
            if (frame == null) continue;

            // Apply visual effects
            var processed = _effects.Process(frame, _audio.Vibe);

            // Send to sinks
            foreach (var sink in _sinks)
                sink.Submit(processed);

            await Task.Delay(16); // ~60fps
        }
    }
}
