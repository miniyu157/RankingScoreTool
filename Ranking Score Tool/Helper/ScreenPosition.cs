using System.Runtime.InteropServices;
using System.Windows;

namespace Ranking_Score_Tool.Helper;

internal partial class ScreenPosition
{
    // 获取鼠标位置（屏幕坐标）
    public static Point GetScreenPosition()
    {
        Win32Point w32Mouse = new();
        GetCursorPos(ref w32Mouse);
        return new Point(w32Mouse.X, w32Mouse.Y);
    }

    public static Point GetRelativePosition(Window element)
    {
        Point screenPosition = GetScreenPosition();
        Point relativePosition = new(screenPosition.X - element.Left, screenPosition.Y - element.Top);
        return relativePosition;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(ref Win32Point pt);

    [StructLayout(LayoutKind.Sequential)]
    private struct Win32Point
    {
        public int X;
        public int Y;
    }
}
