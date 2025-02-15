using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ranking_Score_Tool.Controls;

/// <summary>
/// DoubleButton.xaml 的交互逻辑
/// </summary>
public partial class DoubleButton : UserControl, INotifyPropertyChanged
{
    public DoubleButton()
    {
        InitializeComponent();
    }

    #region 依赖属性

    // Command1 依赖属性
    public static readonly DependencyProperty Command1Property =
        DependencyProperty.Register(nameof(Command1), typeof(ICommand), typeof(DoubleButton), new PropertyMetadata(null));

    public ICommand Command1
    {
        get { return (ICommand)GetValue(Command1Property); }
        set { SetValue(Command1Property, value); }
    }

    // Command2 依赖属性
    public static readonly DependencyProperty Command2Property =
        DependencyProperty.Register(nameof(Command2), typeof(ICommand), typeof(DoubleButton), new PropertyMetadata(null));

    public ICommand Command2
    {
        get { return (ICommand)GetValue(Command2Property); }
        set { SetValue(Command2Property, value); }
    }

    // Content1 依赖属性
    public static readonly DependencyProperty Content1Property =
        DependencyProperty.Register(nameof(Content1), typeof(object), typeof(DoubleButton), new PropertyMetadata(null));

    public object Content1
    {
        get { return GetValue(Content1Property); }
        set { SetValue(Content1Property, value); }
    }

    // Content2 依赖属性
    public static readonly DependencyProperty Content2Property =
        DependencyProperty.Register(nameof(Content2), typeof(object), typeof(DoubleButton), new PropertyMetadata(null));

    public object Content2
    {
        get { return GetValue(Content2Property); }
        set { SetValue(Content2Property, value); }
    }

    // CornerRadius 依赖属性，用于设置整体圆角（四个角的值）
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(DoubleButton),
            new PropertyMetadata(new CornerRadius(0), OnCornerRadiusChanged));

    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    // 当 CornerRadius 属性改变时，通知左右按钮的圆角属性更新
    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as DoubleButton;
        control?.OnPropertyChanged(nameof(LeftCornerRadius));
        control?.OnPropertyChanged(nameof(RightCornerRadius));
    }

    #endregion

    #region 新增边框相关依赖属性

    // BorderThickness 依赖属性
    public static readonly new DependencyProperty BorderThicknessProperty =
        DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(DoubleButton),
            new PropertyMetadata(new Thickness(1), OnBorderThicknessChanged));

    public new Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    // BorderBrush 依赖属性
    public static readonly new DependencyProperty BorderBrushProperty =
        DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(DoubleButton),
            new PropertyMetadata(Brushes.Gray, OnBorderBrushChanged));

    public new Brush BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    #endregion
    #region 辅助属性（边框相关）

    // 左侧按钮边框厚度
    public Thickness LeftBorderThickness
    {
        get => new(
            BorderThickness.Left,
            BorderThickness.Top,
            BorderThickness.Right / 2,
            BorderThickness.Bottom
        );
    }

    // 右侧按钮边框厚度
    public Thickness RightBorderThickness
    {
        get => new(
            BorderThickness.Left / 2,
            BorderThickness.Top,
            BorderThickness.Right,
            BorderThickness.Bottom
        );
    }

    // 左侧按钮边框颜色
    public Brush LeftBorderBrush => BorderBrush;

    // 右侧按钮边框颜色
    public Brush RightBorderBrush => BorderBrush;

    #endregion
    #region 属性变更处理

    private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as DoubleButton;
        control?.OnPropertyChanged(nameof(LeftBorderThickness));
        control?.OnPropertyChanged(nameof(RightBorderThickness));
    }

    private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as DoubleButton;
        control?.OnPropertyChanged(nameof(LeftBorderBrush));
        control?.OnPropertyChanged(nameof(RightBorderBrush));
    }

    #endregion
    #region 辅助属性（只读）

    // 左侧按钮的圆角：左侧保留 CornerRadius 的左边值，其余为 0
    public CornerRadius LeftCornerRadius
    {
        get
        {
            return new CornerRadius(CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft);
        }
    }

    // 右侧按钮的圆角：右侧保留 CornerRadius 的右边值，其余为 0
    public CornerRadius RightCornerRadius
    {
        get
        {
            return new CornerRadius(0, CornerRadius.TopRight, CornerRadius.BottomRight, 0);
        }
    }

    #endregion

    #region INotifyPropertyChanged 实现

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
