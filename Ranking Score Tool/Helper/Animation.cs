namespace Ranking_Score_Tool.Helper;

internal class Animation
{
    public static double Lerp(double current, double target, double factor)
    {
        return current + (target - current) * factor;
    }
}
