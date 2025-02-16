using System.Windows;

namespace Ranking_Score_Tool
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            LoadedAnimation.Completed += LoadedAnimation_Completed;
        }

        private void LoadedAnimation_Completed(object? sender, EventArgs e)
        {
            MainGrid.RenderTransformOrigin = new Point(0.5, 0.5);
        }
    }
}