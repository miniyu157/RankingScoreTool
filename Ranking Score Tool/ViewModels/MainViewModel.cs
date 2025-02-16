using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ranking_Score_Tool.Helper;
using Ranking_Score_Tool.Model;
using System.Windows;
using System.Windows.Media;

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
    private Brush _specialBrush = StyleChange.DefaultControlFore;

    [ObservableProperty]
    private double _rotate = 0, _offsetX = 0, _offsetY = 0;
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

        easterEggTrigger.ThresholdReached += BurstGuard_Success;
    }

    private void BurstGuard_Success(object? sender, BurstGuard.GuardEventArgs e)
    {
        IsEasterEgg = !IsEasterEgg;


    }

    private void InitializeMode()
    {
        modeHandler.ModeChanged += ModeHandler_ModeChanged;
        Mode = CalcMode.Rks;
    }

    private void ModeHandler_ModeChanged(object? sender, ModeHandler.ModeEventArgs e)
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

    private readonly BurstGuard easterEggTrigger = new(new TimeSpan(0, 0, 1), 5);
    private readonly double elasticity = -2.5; // 单击按钮弹力
    private readonly TimeSpan timeout = new(0, 0, 1);

    [RelayCommand]
    private void EasterEgg()
    {
        if (IsEasterEgg) //已在彩蛋状态直接关闭
        {
            IsEasterEgg = !IsEasterEgg;
        }
        else
        {
            if (!isBootAnimation)
            {
                StartAnimation();
            }

            if (!easterEggTrigger.IsRunning)
            {
                easterEggTrigger.Start();
            }

            Rotate = elasticity * easterEggTrigger.Increment(); // 弹
        }
    }
    #endregion

    #region 彩蛋
    private double _targetRotate, _targetOffsetX, _targetOffsetY;
    private const double SmoothFactor = 0.1; // 跟随速度

    private bool _isEasterEgg = false;

    private bool IsEasterEgg
    {
        get => _isEasterEgg;
        set
        {
            if (_isEasterEgg != value)
            {
                _isEasterEgg = value;

                OnPropertyChanged(nameof(EggVisibility));

                UpdateColor();
            }
        }
    }

    public Visibility EggVisibility => IsEasterEgg ? Visibility.Visible : Visibility.Hidden;

    public double scale = 1.2; // 移动数值缩放倍率

    private void UpdateColor()
    {
        dynamic brush;
        if (IsEasterEgg)
        {
            brush = StyleChange.GenerateColorfulBrush(rand);
        }
        else
        {
            brush = StyleChange.DefaultThemeBrush;
        }

        StyleChange.SetThemeColor(brush);

        SpecialBrush = IsEasterEgg ? brush : StyleChange.DefaultControlFore;
    }

    public void StartAnimation()
    {
        CompositionTarget.Rendering += UpdateAnimation;

        isBootAnimation = true;
    }

    bool isBootAnimation = false;

    private void UpdateAnimation(object? sender, EventArgs e)
    {
        UpdateTarget();

        Rotate = Animation.Lerp(Rotate, _targetRotate, SmoothFactor);
        OffsetX = Animation.Lerp(OffsetX, _targetOffsetX, SmoothFactor);
        OffsetY = Animation.Lerp(OffsetY, _targetOffsetY, SmoothFactor);
    }

    private void UpdateTarget()
    {
        if (!IsEasterEgg)
        {
            _targetRotate = 0;
            _targetOffsetX = 0;
            _targetOffsetY = 0;
            return;
        }

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
    }

    #endregion

}
