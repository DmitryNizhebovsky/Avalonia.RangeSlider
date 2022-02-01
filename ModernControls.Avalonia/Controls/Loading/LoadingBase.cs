using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace ModernControls.Avalonia.Controls.Loading
{
    public abstract class LoadingBase : ContentControl
    {
        public static readonly DirectProperty<LoadingBase, bool> IsRunningProperty =
            AvaloniaProperty.RegisterDirect<LoadingBase, bool>(
                nameof(IsRunning),
                o => o.IsRunning,
                (o, v) =>
                    {
                        o.IsRunning = v;
                        o.Clock.PlayState = v ? PlayState.Run : PlayState.Pause;
                    },
                true);

        public static readonly StyledProperty<int> DotCountProperty =
            AvaloniaProperty.Register<LoadingBase, int>(nameof(DotCount), 5);

        public static readonly StyledProperty<double> DotIntervalProperty =
            AvaloniaProperty.Register<LoadingBase, double>(nameof(DotInterval), 10.0);

        public static readonly StyledProperty<Brush> DotBorderBrushProperty =
            AvaloniaProperty.Register<LoadingBase, Brush>(nameof(DotBorderBrush));

        public static readonly StyledProperty<double> DotBorderThicknessProperty =
            AvaloniaProperty.Register<LoadingBase, double>(nameof(DotBorderThickness), 0.0);

        public static readonly StyledProperty<double> DotDiameterProperty =
            AvaloniaProperty.Register<LoadingBase, double>(nameof(DotDiameter), 6.0);

        public static readonly StyledProperty<double> DotSpeedProperty =
            AvaloniaProperty.Register<LoadingBase, double>(nameof(DotSpeed), 4.0);

        public static readonly StyledProperty<double> DotDelayTimeProperty =
            AvaloniaProperty.Register<LoadingBase, double>(nameof(DotDelayTime), 80.0);

        protected readonly Canvas Canvas;
        private bool _isRunning = true;

        static LoadingBase()
        {
            AffectsRender<LoadingBase>(DotDelayTimeProperty, DotSpeedProperty, DotDiameterProperty,
                DotBorderThicknessProperty, DotBorderBrushProperty, DotIntervalProperty, DotCountProperty);
        }

        protected LoadingBase()
        {
            Canvas = new()
            {
                ClipToBounds = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            Content = Canvas;
            Clock = new Clock();
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetAndRaise(IsRunningProperty, ref _isRunning, value);
        }

        public int DotCount
        {
            get => GetValue(DotCountProperty);
            set => SetValue(DotCountProperty, value);
        }

        public double DotInterval
        {
            get => GetValue(DotIntervalProperty);
            set => SetValue(DotIntervalProperty, value);
        }

        public IBrush DotBorderBrush
        {
            get => GetValue(DotBorderBrushProperty);
            set => SetValue(DotBorderBrushProperty, value);
        }

        public double DotBorderThickness
        {
            get => GetValue(DotBorderThicknessProperty);
            set => SetValue(DotBorderThicknessProperty, value);
        }

        public double DotDiameter
        {
            get => GetValue(DotDiameterProperty);
            set => SetValue(DotDiameterProperty, value);
        }

        public double DotSpeed
        {
            get => GetValue(DotSpeedProperty);
            set => SetValue(DotSpeedProperty, value);
        }

        public double DotDelayTime
        {
            get => GetValue(DotDelayTimeProperty);
            set => SetValue(DotDelayTimeProperty, value);
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);
            UpdateDots();
        }

        protected abstract void UpdateDots();

        protected virtual Ellipse CreateEllipse(int index)
        {
            var ellipse = new Ellipse();
            ellipse.Bind(WidthProperty, new Binding(DotDiameterProperty.Name) { Source = this });
            ellipse.Bind(HeightProperty, new Binding(DotDiameterProperty.Name) { Source = this });
            ellipse.Bind(Shape.FillProperty, new Binding(ForegroundProperty.Name) { Source = this });
            ellipse.Bind(Shape.StrokeThicknessProperty, new Binding(DotBorderThicknessProperty.Name) { Source = this });
            ellipse.Bind(Shape.StrokeProperty, new Binding(DotBorderBrushProperty.Name) { Source = this });
            return ellipse;
        }
    }
}
