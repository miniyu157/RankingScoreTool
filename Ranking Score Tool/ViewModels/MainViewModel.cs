using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EleCho.WpfSuite.Helpers;
using KlxPiao.ColorTool;
using Ranking_Score_Tool.Helper;
using Ranking_Score_Tool.Model;
using System.Windows;
using System.Windows.Media;
using static Ranking_Score_Tool.Model.ModeHandler;

namespace Ranking_Score_Tool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region UI绑定的属性
    [ObservableProperty]
    private string _x1 = string.Empty, _x1Title = string.Empty, _x1Placeholder = "在这里输入 Dif";

    [ObservableProperty]
    private string _x2 = string.Empty, _x2Title = string.Empty, _x2Placeholder = string.Empty;

    [ObservableProperty]
    private string _tip = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private Brush _themeBrush = Brushes.Transparent;
    #endregion

    private readonly ModeHandler modeHandler = new();
    private readonly Random rand = new();

    private CalcMode Mode
    {
        get => modeHandler.Mode;
        set => modeHandler.Mode = value;
    }

    public MainViewModel()
    {
        InitializeMode();
        StartAnimation();

        oneTick = new(timeout, () => { clickCount = 0; });
    }

    private void InitializeMode()
    {
        modeHandler.ModeChanged += ModeHandler_ModeChanged;
        Mode = CalcMode.Rks;
    }

    private void ModeHandler_ModeChanged(object? sender, ModeEventArgs e)
    {
        Title = $"单曲 {e.NewMode} 计算";
        (X1Title, X2Title) = e.NewMode switch
        {
            CalcMode.Rks => ("Dif", "Acc"),
            CalcMode.Acc => ("Dif", "Rks"),
            _ => default
        };
        Tip = "将在此处显示计算结果";
        X2 = string.Empty;
        X2Placeholder = $"在这里输入 {(CalcMode)((int)e.NewMode ^ 1)}";
    }

    #region 主要按钮命令
    [RelayCommand]
    private void RksMode() => Mode = CalcMode.Rks;

    [RelayCommand]
    private void AccMode() => Mode = CalcMode.Acc;

    [RelayCommand]
    private void Calculate()
    {
        X1 = X1.Trim();
        X2 = X2.Trim();

        if (double.TryParse(X1, out var x1) && double.TryParse(X2.Replace("%", ""), out var x2))
        {
            Tip = Mode switch
            {
                CalcMode.Rks => $"计算结果:\r\n您的单曲 Rks 为 {Function.CalcuateRks(x1, x2)}",
                CalcMode.Acc => $"计算结果:\r\nAcc 至少需要 {Function.CalcuateAcc(x1, x2)}% 才能使单曲 Rks 达到 {x2}",
                _ => string.Empty,
            };
        }
        else
        {
            Tip = $"输入内容有误";
        }
    }
    #endregion

    #region 底栏按钮的命令
    [RelayCommand]
    private static void OpenFunctionView() => new FunctionView().ShowDialog();

    [RelayCommand]
    private void GenerateRandomNum()
    {
        (double x1, double x2) = RandomValue.Generate(Mode, rand, out string suffix);

        X1 = $"{x1}";
        X2 = $"{x2}{suffix}";
    }

    private readonly OneTick oneTick;
    private int clickCount = 0;
    private readonly double elasticity = -2.5; // 单击按钮弹力
    private readonly TimeSpan timeout = new(0, 0, 1);

    [RelayCommand]
    private void EasterEgg()
    {
        if (IsEasterEgg)
        {
            _targetRotate = 0;
            _targetOffsetX = 0;
            _targetOffsetY = 0;
            IsEasterEgg = !IsEasterEgg;
            return;
        }

        if (oneTick.IsRuning)
        {
            Rotate = elasticity * ++clickCount;
            if (clickCount == 5)
            {
                clickCount = 0;
                IsEasterEgg = !IsEasterEgg;
            }
        }
        else
        {
            Rotate = elasticity * ++clickCount;
            oneTick.Start();
        }
    }
    #endregion

    #region 彩蛋
    [ObservableProperty]
    private double _rotate = 61, _offsetX = 0, _offsetY = -500; // _rotate为60会导致Random按钮的图标消失

    private double _targetRotate, _targetOffsetX, _targetOffsetY; // 默认状态
    private const double SmoothFactor = 0.1; // 跟随速度

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EggVisibility))]
    private bool isEasterEgg = false;

    public Visibility EggVisibility => IsEasterEgg ? Visibility.Visible : Visibility.Hidden;

    public double scale = 1.2; // 数值缩放倍率

    void UpdateTargetValueAndColor()
    {
        Window window = Application.Current.MainWindow;
        Point p = ScreenPosition.GetRelativePosition(window);

        double minX = 4;
        double maxX = window.Width - 5;
        double minY = 27;
        double maxY = window.Height - 5;

        double lengthWidth = maxX - minX;
        double lengthHeight = maxY - minY;
        double pixelWidth = p.X - minX;
        double pixelHeight = p.Y - minY;
        double progressX = pixelWidth / lengthWidth;
        double progressY = pixelHeight / lengthHeight;

        _targetRotate = -((progressX + progressY) / 2 - 0.5) * 360 * scale;
        _targetOffsetX = -(pixelWidth - lengthWidth / 2) * scale;
        _targetOffsetY = -(pixelHeight - lengthHeight / 2) * scale;

        Color themeColor = HsvColor.FromString($"{(progressX + progressY) / 2 * 360}, 24.705881%, 100%");
        SolidColorBrush brush = new(themeColor);

        //Application.Current.Resources["ThemeColor"] = themeColor; // 改这个没用的

        string[] keys = ["CardBorder", "Selection", "FocusedBorder"];
        foreach (string key in keys)
        {
            Application.Current.Resources[key] = brush;
        }

        Application.Current.Resources["WindowBorder"] = (WindowOptionColor)themeColor;

        ThemeBrush = IsEasterEgg ? brush : (Brush)Application.Current.Resources["ControlFore"];
    }

    public void StartAnimation()
    {
        CompositionTarget.Rendering += UpdateAnimation;
    }

    private void UpdateAnimation(object? sender, EventArgs e)
    {
        //  使用线性插值平滑过渡
        Rotate = Animation.Lerp(Rotate, _targetRotate, SmoothFactor);
        OffsetX = Animation.Lerp(OffsetX, _targetOffsetX, SmoothFactor);
        OffsetY = Animation.Lerp(OffsetY, _targetOffsetY, SmoothFactor);

        if (IsEasterEgg)
        {
            UpdateTargetValueAndColor();
        }
    }
    #endregion

}
