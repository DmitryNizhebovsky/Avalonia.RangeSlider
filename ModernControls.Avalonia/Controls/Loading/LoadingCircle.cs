using System.Reactive.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using ModernControls.Avalonia.Controls.Animators;

namespace ModernControls.Avalonia.Controls.Loading
{
    public class LoadingCircle : LoadingBase
    {
        public static readonly StyledProperty<double> DotOffSetProperty =
            AvaloniaProperty.Register<LoadingCircle, double>(nameof(DotOffSet), 20.0);

        public static readonly StyledProperty<bool> NeedHiddenProperty =
            AvaloniaProperty.Register<LoadingCircle, bool>(nameof(NeedHidden), true);

        private readonly DiscreteAnimator _discreteAnimator = new DiscreteAnimator();

        public double DotOffSet
        {
            get => GetValue(DotOffSetProperty);
            set => SetValue(DotOffSetProperty, value);
        }

        public bool NeedHidden
        {
            get => GetValue(NeedHiddenProperty);
            set => SetValue(NeedHiddenProperty, value);
        }

        static LoadingCircle()
        {
            AffectsRender<LoadingCircle>(DotOffSetProperty, NeedHiddenProperty);

            DotSpeedProperty.OverrideDefaultValue<LoadingCircle>(6.0);
            DotDelayTimeProperty.OverrideDefaultValue<LoadingCircle>(220.0);
        }

        protected sealed override void UpdateDots()
        {
            var dotCount = DotCount;
            var dotInterval = DotInterval;
            var dotSpeed = DotSpeed;
            var dotDelayTime = DotDelayTime;
            var needHidden = NeedHidden;

            if (dotCount < 1) return;
            Canvas.Children.Clear();

            Clock.PlayState = PlayState.Pause;

            for (var i = 0; i < dotCount; i++)
            {
                var ellipse = CreateEllipse(i, dotInterval, needHidden);
                var subAngle = -dotInterval * i;

                var rotateAnimation = new Animation
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
                                new Setter(RotateTransform.AngleProperty, subAngle)
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (0.75 / 7)),
                            Setters =
                            {
                                new Setter(RotateTransform.AngleProperty, 180 + subAngle)
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (2.75 / 7)),
                            Setters =
                            {
                                new Setter(RotateTransform.AngleProperty, 180 + DotOffSet + subAngle)
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (3.5 / 7)),
                            Setters =
                            {
                                new Setter(RotateTransform.AngleProperty, 360 + subAngle)
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (4.25 / 7)),
                            Setters =
                            {
                                new Setter(RotateTransform.AngleProperty, 540 + subAngle)
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed * (6.25 / 7)),
                            Setters =
                            {
                                new Setter(RotateTransform.AngleProperty, 540 + DotOffSet + subAngle)
                            }
                        },
                        new KeyFrame
                        {
                            KeyTime = TimeSpan.FromSeconds(dotSpeed),
                            Setters =
                            {
                                new Setter(RotateTransform.AngleProperty, 720 + subAngle)
                            }
                        }
                    }
                };

                rotateAnimation.Apply(ellipse, Clock, Observable.Return(true), null);

                if (NeedHidden)
                {
                    var hiddenAnimation = new Animation
                    {
                        Duration = TimeSpan.FromSeconds(dotSpeed),
                        Delay = TimeSpan.FromMilliseconds(dotDelayTime * i),
                        IterationCount = IterationCount.Infinite,
                        Easing = new LinearEasing(),
                        Children =
                        {
                            new KeyFrame
                            {
                                KeyTime = TimeSpan.FromSeconds(0),
                                Setters =
                                {
                                    new Setter
                                    {
                                        Property = OpacityProperty,
                                        Value = 1d
                                    }
                                }
                            },
                            new KeyFrame
                            {
                                KeyTime = TimeSpan.FromSeconds(dotSpeed - 0.4),
                                Setters =
                                {
                                    new Setter
                                    {
                                        Property = OpacityProperty,
                                        Value = 0d
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
                                        Property = OpacityProperty,
                                        Value = 0d
                                    }
                                }
                            }
                        }
                    };

                    Animation.RegisterAnimator<DiscreteAnimator>(prop => prop.Name == OpacityProperty.Name);
                    hiddenAnimation.Apply(ellipse, Clock, Observable.Return(true), null);
                }

                Canvas.Children.Add(ellipse);
            }

            if (IsRunning)
            {
                Clock.PlayState = PlayState.Run;
            }
        }

        private Ellipse CreateEllipse(int index, double dotInterval, bool needHidden)
        {
            var ellipse = CreateEllipse(index);
            var halfWidth = Bounds.Width / 2d;
            var bottom = Bounds.Width - DotDiameter;

            ellipse.RenderTransformOrigin = new RelativePoint(new Point(0, halfWidth), RelativeUnit.Absolute);
            ellipse.IsVisible = needHidden;

            ellipse.SetValue(Canvas.LeftProperty, halfWidth);
            ellipse.SetValue(Canvas.TopProperty, 0);

            //ellipse.SetValue(Canvas.LeftProperty, halfWidth);
            //ellipse.SetValue(Canvas.TopProperty, bottom);

            ellipse.RenderTransform = new TransformGroup
            {
                Children =
                { 
                    new RotateTransform
                    {
                        Angle = -dotInterval * index
                    }
                }
            };

            return ellipse;
        }
    }
}
