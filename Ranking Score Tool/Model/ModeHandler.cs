namespace Ranking_Score_Tool.Model;

internal class ModeHandler
{
    public ModeHandler()
    {

    }

    public enum CalcMode
    {
        Rks,
        Acc
    }

    // 初始化一个不存在的枚举成员，使其第一次设置值仍然能够触发事件
    private CalcMode _mode = (CalcMode)(-99);
    public CalcMode Mode
    {
        get => _mode;
        set
        {
            if (_mode != value)
            {
                ModeChanged?.Invoke(this, new ModeEventArgs(_mode, value));

                _mode = value;
            }
        }
    }

    public event EventHandler<ModeEventArgs>? ModeChanged;

    public class ModeEventArgs(CalcMode oldMode, CalcMode newMode) : EventArgs
    {
        public CalcMode OldMode { get; } = oldMode;
        public CalcMode NewMode { get; } = newMode;
    }


}
