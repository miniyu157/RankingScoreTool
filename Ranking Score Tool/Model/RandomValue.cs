using static Ranking_Score_Tool.Model.ModeHandler;

namespace Ranking_Score_Tool.Model;

internal static class RandomValue
{
    public static (double, double) Generate(CalcMode mode, Random random, out string suffix)
    {
        double randX1 = random.Next(130, 170) / (double)10;

        (double randX2, suffix) = mode switch
        {
            CalcMode.Rks => (random.Next(9600, 10000) / (double)100, "%"), // 生成 acc
            CalcMode.Acc => ((int)randX1, string.Empty),                   // 生成 rks
            _ => (double.NaN, string.Empty)
        };
        return (randX1, randX2);
    }
}
