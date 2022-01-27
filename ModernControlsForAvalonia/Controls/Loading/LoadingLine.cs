using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace ModernControlsForAvalonia.Controls.Loading
{
    public class LoadingLine : LoadingBase
    {
        private const double MoveLength = 80;
        private const double UniformScale = 0.6;

        public LoadingLine()
        {
            this.Bind(HeightProperty, new Binding(DotDiameterProperty.Name) { Source = this });
        }

        protected sealed override void UpdateDots()
        {
            var dotCount = DotCount;
            var dotInterval = DotInterval;
            var dotDiameter = DotDiameter;
            var dotSpeed = DotSpeed;
            var dotDelayTime = DotDelayTime;

            if (dotCount < 1) return;
            Canvas.Children.Clear();

            var centerWidth = dotDiameter * dotCount + dotInterval * (dotCount - 1) + MoveLength;
            var speedDownLength = (Bounds.Width - MoveLength) / 2;
            var speedUniformLength = centerWidth / 2;

            Clock.PlayState = PlayState.Pause;

            for (var i = 0; i < dotCount; i++)
            {
                var ellipse = CreateEllipse(i, dotInterval, dotDiameter);

                var lineAnimation = new Animation
                {
                    Duration = TimeSpan.FromSeconds(dotSpeed),
                    Delay = TimeSpan.FromMilliseconds(dotDelayTime * i),
                    IterationCount = IterationCount.Infinite,
                    Easing = new LinearEasing(),
                    Children =
                    {
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.Zero,
                            Setters =
                            {
                                new Setter
                                {
                                    Property = MarginProperty,
                                    Value = new Thickness(ellipse.Margin.Left, 0, 0, 0)
                                }
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (1 - UniformScale) / 2),
                            Setters =
                            {
                                new Setter
                                {
                                    Property = MarginProperty,
                                    Value = new Thickness(speedDownLength + ellipse.Margin.Left, 0, 0, 0)
                                }
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (1 + UniformScale) / 2),
                            Setters =
                            {
                                new Setter
                                {
                                    Property = MarginProperty,
                                    Value = new Thickness(speedDownLength + speedUniformLength + ellipse.Margin.Left, 0, 0, 0)
                                }
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed),
                            Setters =
                            {
                                new Setter
                                {
                                    Property = MarginProperty,
                                    Value = new Thickness(Bounds.Width + ellipse.Margin.Left + speedUniformLength, 0, 0, 0)
                                }
                            }
                        }
                    }
                };

                lineAnimation.Apply(ellipse, Clock, Observable.Return(true), null);

                Canvas.Children.Add(ellipse);
            }

            if (IsRunning)
            {
                Clock.PlayState = PlayState.Run;
            }
        }

        private Ellipse CreateEllipse(int index, double dotInterval, double dotDiameter)
        {
            var ellipse = base.CreateEllipse(index);
            ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            ellipse.VerticalAlignment = VerticalAlignment.Top;
            ellipse.Margin = new Thickness(-(dotInterval + dotDiameter) * index, 0, 0, 0);
            return ellipse;
        }
    }
}
