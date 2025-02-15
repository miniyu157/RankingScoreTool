namespace Ranking_Score_Tool.Helper;

public record class OneTick(TimeSpan TimeSpan, Action Action)
{
    public bool IsRuning = false;

    public async void Start()
    {
        IsRuning = true;

        await Task.Delay(TimeSpan);

        _ = Task.Run(Action);

        IsRuning = false;
    }
}
