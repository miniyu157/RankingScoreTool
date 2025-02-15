namespace Ranking_Score_Tool.Model;

internal static class Function
{
    public static double CalcuateRks(double dif, double acc)
    {
        if (acc < 70) return 0;

        return Math.Round(Math.Pow((acc - 55) / 45, 2) * dif, 2);
    }

    public static double CalcuateAcc(double dif, double rks)
    {
        return Math.Round(55 + 45 * Math.Sqrt(rks / dif), 2);
    }
}
