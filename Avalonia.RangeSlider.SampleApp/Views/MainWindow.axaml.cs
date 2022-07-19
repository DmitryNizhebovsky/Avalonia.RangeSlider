using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.RangeSlider.Enums;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace Avalonia.RangeSlider.SampleApp.Views
{
    public class MainWindow : Window
    {
        private bool _isMatherial;

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

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            App.Current.Styles.Clear();
            var appUri = new Uri("avares://Avalonia.RangeSlider.SampleApp/App.axaml");
            var rStyle = new RangeSliderStyle(appUri);
            if (!_isMatherial)
            {
                var matherialUri = new Uri("avares://avares://Avalonia.RangeSlider.SampleApp");
                var styleInclude = new StyleInclude(matherialUri)
                {
                    Source = new Uri("avares://Material.Avalonia/Material.Avalonia.Templates.xaml")
                };
                Application.Current.Styles.Add(styleInclude);
                rStyle.Theme = StyleTheme.Material;
                Application.Current.Styles.Add(rStyle);
            }
            else
            {
                var fluent = new FluentTheme(appUri);
                Application.Current.Styles.Add(fluent);
                Application.Current.Styles.Add(rStyle);
            }


            _isMatherial = !_isMatherial;
        }
    }
}