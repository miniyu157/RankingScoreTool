using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EleCho.WpfSuite.Helpers;
using KlxPiao.ColorTool;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ranking_Score_Tool
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Mode = CalcMode.Rks;
            UpdateUI();
            StartAnimation();

            oneTick = new(timeout, () => { clickCount = 0; });
        }

        #region UI绑定的属性
        [ObservableProperty]
        private string _x1 = string.Empty;

        [ObservableProperty]
        private string _x2 = string.Empty;

        [ObservableProperty]
        private string _x1Title = string.Empty;

        [ObservableProperty]
        private string _x2Title = string.Empty;

        [ObservableProperty]
        private string _tip = string.Empty;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _x1Placeholder = "在这里输入 Dif";

        [ObservableProperty]
        private string _x2Placeholder = string.Empty;

        [ObservableProperty]
        private Brush _themeBrush = Brushes.Transparent;

        #endregion

        #region 模式切换
        private CalcMode _mode;
        private CalcMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;

                    UpdateUI();
                }
            }
        }

        private enum CalcMode
        {
            Rks,
            Acc
        }

        [RelayCommand]
        private void RksMode()
        {
            Mode = CalcMode.Rks;
        }

        [RelayCommand]
        private void AccMode()
        {
            Mode = CalcMode.Acc;
        }
        #endregion

        private void UpdateUI()
        {
            Title = $"单曲 {Mode} 计算";
            (X1Title, X2Title) = Mode switch
            {
                CalcMode.Rks => ("Dif", "Acc"),
                CalcMode.Acc => ("Dif", "Rks"),
                _ => default
            };
            Tip = "将在此处显示计算结果";
            X2 = "";
            X2Placeholder = $"在这里输入 {(CalcMode)((int)Mode ^ 1)}";
        }


        #region 计算
        [RelayCommand]
        private void Calculate()
        {
            X1 = X1.Trim();
            X2 = X2.Trim();

            if (double.TryParse(X1, out var x1) && double.TryParse(X2.Replace("%", ""), out var x2))
            {
                switch (Mode)
                {
                    case CalcMode.Rks:
                        Tip = $"计算结果:\r\n您的单曲 Rks 为 {CalcuateRks(x1, x2)}";
                        break;

                    case CalcMode.Acc:
                        Tip = $"计算结果:\r\nAcc 至少需要 {CalcuateAcc(x1, x2)}% 才能使单曲 Rks 达到 {x2}";
                        break;
                }
            }
            else
            {
                Tip = $"输入内容有误";
            }
        }

        private static double CalcuateRks(double dif, double acc)
        {
            if (acc < 70) return 0;

            return Math.Round(Math.Pow((acc - 55) / 45, 2) * dif, 2);
        }

        private static double CalcuateAcc(double dif, double rks)
        {
            return Math.Round(55 + 45 * Math.Sqrt(rks / dif), 2);
        }
        #endregion

        #region 底栏按钮的命令
        [RelayCommand]
        private static void OpenFunctionView()
        {
            new FunctionView().ShowDialog();
        }

        private readonly Random rand = new();

        [RelayCommand]
        private void GenerateRandomNum()
        {
            double randDif = rand.Next(130, 170) / (double)10;
            X1 = $"{randDif}";
            X2 = Mode switch
            {
                CalcMode.Rks => $"{rand.Next(9600, 10000) / (double)100}%", // 生成 acc
                CalcMode.Acc => $"{(int)randDif}",                          // 生成 rks
                _ => string.Empty
            };
        }

        private readonly OneTick oneTick;
        private int clickCount = 0;
        private readonly double elasticity = -2.5; //单击按钮弹力
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
        private double _rotate = 60;

        [ObservableProperty]
        private double _offsetX = 0;

        [ObservableProperty]
        private double _offsetY = -500;

        private double _targetRotate;
        private double _targetOffsetX;
        private double _targetOffsetY;
        private const double SmoothFactor = 0.1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EggVisibility))]
        private bool isEasterEgg = false;

        public Visibility EggVisibility => IsEasterEgg ? Visibility.Visible : Visibility.Hidden;

        public double scale = 1.2;

        [RelayCommand]
        private void MouseMove(MouseEventArgs e)
        {
            if (!IsEasterEgg) return;

            Window window = Application.Current.MainWindow;
            Point p = e.GetPosition(window);

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

            // 更新目标值而不是直接设置
            _targetRotate = -((progressX + progressY) / 2 - 0.5) * 360 * scale;
            _targetOffsetX = -(pixelWidth - lengthWidth / 2) * scale;
            _targetOffsetY = -(pixelHeight - lengthHeight / 2) * scale;

            colorProgressX = progressX;
        }

        private record class OneTick(TimeSpan TimeSpan, Action Action)
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

        private double colorProgressX;

        public void StartAnimation()
        {
            CompositionTarget.Rendering += UpdateAnimation;
        }

        private void UpdateAnimation(object? sender, EventArgs e)
        {
            // 使用线性插值平滑过渡
            Rotate = Lerp(Rotate, _targetRotate, SmoothFactor);
            OffsetX = Lerp(OffsetX, _targetOffsetX, SmoothFactor);
            OffsetY = Lerp(OffsetY, _targetOffsetY, SmoothFactor);

            if (!IsEasterEgg) return;

            Color themeColor = HsvColor.FromString($"{colorProgressX * 360}, 24.705881%, 100%");
            SolidColorBrush brush = new(themeColor);

            //Application.Current.Resources["ThemeColor"] = themeColor; //改这个没用的
            Application.Current.Resources["CardBorder"] = brush;
            Application.Current.Resources["Selection"] = brush;
            Application.Current.Resources["FocusedBorder"] = brush;
            Application.Current.Resources["WindowBorder"] = (WindowOptionColor)themeColor;
            ThemeBrush = IsEasterEgg ? brush : (Brush)Application.Current.Resources["ControlFore"];
        }

        private static double Lerp(double current, double target, double factor)
        {
            return current + (target - current) * factor;
        }
        #endregion



    }
}
