using System.Diagnostics;

namespace Ranking_Score_Tool.Helper;

public class BurstGuard(TimeSpan duration, int maxCount)
{
    public bool IsRunning { get; private set; } = false;

    public Dictionary<int, TimeSpan> CountTimestamps { get; private set; } = [];

    private int _count = 0;

    public int Count
    {
        get => _count;
        private set
        {
            _count = value;

            CountTimestamps.Add(value, timer.Elapsed);

            if (value == maxCount)
            {
                ThresholdReached?.Invoke(this, new GuardEventArgs(CountTimestamps));
                Reset();
            }
        }
    }

    private CancellationTokenSource? cts;

    private readonly Stopwatch timer = new();

    public async void Start()
    {
        if (IsRunning) return;

        IsRunning = true;
        cts = new CancellationTokenSource();
        timer.Start();

        try
        {
            await Task.Delay(duration, cts.Token);

            ThresholdTimeout?.Invoke(this, new GuardEventArgs(CountTimestamps));
            Reset();
        }
        catch (TaskCanceledException) { }
    }

    public void Reset()
    {
        if (IsRunning)
        {
            cts?.Cancel();
        }
        Count = 0;
        timer.Reset();
        CountTimestamps.Clear();
        IsRunning = false;
    }

    public int Increment() => ++Count;

    public event EventHandler<GuardEventArgs>? ThresholdReached;

    public event EventHandler<GuardEventArgs>? ThresholdTimeout;

    public class GuardEventArgs(Dictionary<int, TimeSpan> countTimestamps) : EventArgs
    {
        public Dictionary<int, TimeSpan> CountTimestamps { get; } = countTimestamps;

        public override string ToString()
        {
            return string.Join("\r\n", CountTimestamps.Select(x => $"{x.Key}: {x.Value}"));
        }
    }
}