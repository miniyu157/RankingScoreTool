using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Ranking_Score_Tool
{
    public partial class FunctionViewModel : ObservableObject
    {
        [RelayCommand]
        private static void OpenLink()
        {
            Process.Start(new ProcessStartInfo("https://www.taptap.cn/moment/181012542707992850") { UseShellExecute = true });
        }
    }
}
