using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ModernControls.Avalonia.Demo.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
