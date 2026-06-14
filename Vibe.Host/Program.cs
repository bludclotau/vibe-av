using AVEngine.Engine;
using AVEngine.Core.Audio;
using Vibe.Host.Tui;

// Create engine wrapper
var engine = new AvEngine();

// Create a test track with noise feeding the granular node
var testTrack = new MediaTrack { Name = "test" };

var noise     = new NoiseDspNode { Amplitude = 0.25f };
var granular  = new GranularDspNode();

testTrack.AudioDspChain.Add(noise);
testTrack.AudioDspChain.Add(granular);

engine.Audio.Project.Tracks.Add(testTrack);

// Cancellation token for listeners + TUI
var cts = new CancellationTokenSource();

// Start listeners (HTTP :5555, WS :5556, named pipe)
_ = engine.StartListenersAsync(cts.Token);

// Run the TUI (blocks until exit)
await MainMenu.RunAsync(engine, cts.Token);
