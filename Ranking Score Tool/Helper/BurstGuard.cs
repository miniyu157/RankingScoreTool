using System.Diagnostics;

namespace Ranking_Score_Tool.Helper;

/// <summary>
/// 突发流量防护机制。
/// </summary>
/// <remarks>
/// 在指定时间窗口内进行操作计数，达到阈值或超时时触发事件。
/// </remarks>
public class BurstGuard(TimeSpan duration, int maxCount)
{
    /// <summary>
    /// 获取防护机制是否正在运行。
    /// </summary>
    public bool IsRunning { get; private set; } = false;

    /// <summary>
    /// 获取计数时间戳字典。
    /// </summary>
    public Dictionary<int, TimeSpan> CountTimestamps { get; private set; } = [];

    private int _count = 0;

    /// <summary>
    /// 获取当前计数。
    /// </summary>
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

    /// <summary>
    /// 启动防护机制。
    /// </summary>
    /// <remarks>
    /// 如果已启动则不会重复执行，超时后自动触发 <see cref="ThresholdTimeout"/>事件。
    /// </remarks>
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

    /// <summary>
    /// 重置防护机制状态。
    /// </summary>
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

    /// <summary>
    /// 增加计数并返回当前值。
    /// </summary>
    /// <returns>增加后的计数值。</returns>
    public int Increment() => ++Count;

    /// <summary>
    /// 达到最大计数时触发的事件。
    /// </summary>
    public event EventHandler<GuardEventArgs>? ThresholdReached;

    /// <summary>
    /// 检测周期超时时触发的事件。
    /// </summary>
    public event EventHandler<GuardEventArgs>? ThresholdTimeout;

    /// <summary>
    /// 防护事件参数。
    /// </summary>
    public class GuardEventArgs(Dictionary<int, TimeSpan> countTimestamps) : EventArgs
    {
        /// <summary>
        /// 获取计数时间戳字典。
        /// </summary>
        public Dictionary<int, TimeSpan> CountTimestamps { get; } = countTimestamps;

        /// <summary>
        /// 转换为格式化的字符串。
        /// </summary>
        /// <returns>按行格式化的计数时间信息</returns>
        public override string ToString()
        {
            return string.Join("\r\n", CountTimestamps.Select(x => $"{x.Key}: {x.Value}"));
        }
    }
}