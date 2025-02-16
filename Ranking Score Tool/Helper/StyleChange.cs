using KlxPiao.ColorTool;
using System.Windows;
using System.Windows.Media;

namespace Ranking_Score_Tool.Helper
{
    internal class StyleChange
    {
        public static void SetThemeColor(object brush)
        {
            string[] keys = ["CardBorder", "Selection", "FocusedBorder"];
            foreach (string key in keys)
            {
                Application.Current.Resources[key] = brush;
            }
        }

        public static Brush DefaultThemeBrush => new SolidColorBrush((Color)Application.Current.Resources["ThemeColor"]);
        public static Brush DefaultControlFore => (Brush)Application.Current.Resources["ControlFore"];

        public static LinearGradientBrush GenerateColorfulBrush(Random random)
        {
            float hue = random.Next(0, 360);
            Color themeColor = HsvColor.FromHsv(hue, 0.24705881f, 1);
            Color themeColor2 = HsvColor.FromHsv(hue + 90, 0.24705881f, 1);
            Color themeColor3 = HsvColor.FromHsv(hue + 180, 0.24705881f, 1);
            Color themeColor4 = HsvColor.FromHsv(hue + 270, 0.24705881f, 1);
            Color themeColor5 = HsvColor.FromHsv(hue + 360, 0.24705881f, 1);

            LinearGradientBrush brush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            brush.GradientStops.Add(new GradientStop(themeColor, 0));
            brush.GradientStops.Add(new GradientStop(themeColor2, 0.25));
            brush.GradientStops.Add(new GradientStop(themeColor3, 0.5));
            brush.GradientStops.Add(new GradientStop(themeColor4, 0.75));
            brush.GradientStops.Add(new GradientStop(themeColor5, 1));

            return brush;
        }
    }
}
