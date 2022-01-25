using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;
using ModernControlsForAvalonia.Controls.Primitives;
using RangeBase = ModernControlsForAvalonia.Controls.Primitives.RangeBase;

namespace ModernControlsForAvalonia.Controls
{
    public enum RangeDraggedMode
    {
        MoveThumbsBoth,
        MoveThumbsSeparately
    };

    /// <summary>
    /// A control that lets the user select from a range of values by moving a Thumb control along a Track.
    /// </summary>
    [PseudoClasses(":vertical", ":horizontal", ":pressed")]
    public class RangeSlider : RangeBase
    {
        private enum TrackThumb
        {
            None,
            Upper,
            Lower,
            Middle,
            Both
        };

        private enum TrackButton
        {
            Background,
            Foreground
        };

        /// <summary>
        /// Defines the <see cref="Orientation"/> property.
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            ScrollBar.OrientationProperty.AddOwner<RangeSlider>();

        /// <summary>
        /// Defines the <see cref="IsDirectionReversed"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsDirectionReversedProperty =
            RangeTrack.IsDirectionReversedProperty.AddOwner<RangeSlider>();

        /// <summary>
        /// Defines the <see cref="IsThumbOverlapProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsThumbOverlapProperty =
            RangeTrack.IsThumbOverlapProperty.AddOwner<RangeSlider>();

        /// <summary>
        /// Defines the <see cref="IsSnapToTickEnabled"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty =
            AvaloniaProperty.Register<RangeSlider, bool>(nameof(IsSnapToTickEnabled), false);

        /// <summary>
        /// Defines the <see cref="RangeDraggedMode"/> property.
        /// </summary>
        public static readonly StyledProperty<RangeDraggedMode> RangeDraggedModeProperty =
            AvaloniaProperty.Register<RangeSlider, RangeDraggedMode>(nameof(RangeDraggedMode), RangeDraggedMode.MoveThumbsSeparately);

        /// <summary>
        /// Defines the <see cref="TickFrequency"/> property.
        /// </summary>
        public static readonly StyledProperty<double> TickFrequencyProperty =
            AvaloniaProperty.Register<RangeSlider, double>(nameof(TickFrequency), 0.0);

        /// <summary>
        /// Defines the <see cref="TickPlacement"/> property.
        /// </summary>
        public static readonly StyledProperty<TickPlacement> TickPlacementProperty =
            AvaloniaProperty.Register<TickBar, TickPlacement>(nameof(TickPlacement), 0d);

        /// <summary>
        /// Defines the <see cref="TicksProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<AvaloniaList<double>> TicksProperty =
            TickBar.TicksProperty.AddOwner<RangeSlider>();

        // Slider required parts
        private bool _isDragging = false;
        private bool _isThumbDragging = false;
        private double _previousValue = 0.0;
        private RangeTrack _track = null!;
        private Button _backgroundButton = null!;
        private Button _foregroundButton = null!;
        private TrackThumb _currentTrackThumb = TrackThumb.None;
        private IDisposable? _backgroundButtonPressDispose = null!;
        private IDisposable? _foregroundButtonPressDispose = null!;
        private IDisposable? _backgroundButtonReleaseDispose = null!;
        private IDisposable? _foregroundButtonReleaseDispose = null!;
        private IDisposable? _pointerMovedDispose = null!;
        private IDisposable? _pointerReleasedDispose = null!;
        private IDisposable? _lowerThumbPointerMovedDispose = null!;
        private IDisposable? _upperThumbPointerMovedDispose = null!;
        private IDisposable? _lowerThumbPointerPressedDispose = null!;
        private IDisposable? _lowerThumbPointerReleaseDispose = null!;
        private IDisposable? _upperThumbPointerPressedDispose = null!;
        private IDisposable? _upperThumbPointerReleaseDispose = null!;

        private const double Tolerance = 0.0001;

        /// <summary>
        /// Initializes static members of the <see cref="RangeSlider"/> class. 
        /// </summary>
        static RangeSlider()
        {
            PressedMixin.Attach<RangeSlider>();
            FocusableProperty.OverrideDefaultValue<RangeSlider>(true);
            OrientationProperty.OverrideDefaultValue(typeof(RangeSlider), Orientation.Horizontal);

            LowerSelectedValueProperty.OverrideMetadata<RangeSlider>(new DirectPropertyMetadata<double>(enableDataValidation: true));
            UpperSelectedValueProperty.OverrideMetadata<RangeSlider>(new DirectPropertyMetadata<double>(enableDataValidation: true));
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="RangeSlider"/> class. 
        /// </summary>
        public RangeSlider()
        {
            UpdatePseudoClasses(Orientation);
        }

        /// <summary>
        /// Defines the ticks to be drawn on the tick bar.
        /// </summary>
        public AvaloniaList<double> Ticks
        {
            get => GetValue(TicksProperty);
            set => SetValue(TicksProperty, value);
        }

        /// <summary>
        /// Gets or sets the orientation of a <see cref="RangeSlider"/>.
        /// </summary>
        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SelectedRangeDraggedMode of a <see cref="RangeSlider"/>.
        /// </summary>
        public RangeDraggedMode RangeDraggedMode
        {
            get { return GetValue(RangeDraggedModeProperty); }
            set { SetValue(RangeDraggedModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction of increasing value.
        /// </summary>
        /// <value>
        /// true if the direction of increasing value is to the left for a horizontal slider or
        /// down for a vertical slider; otherwise, false. The default is false.
        /// </value>
        public bool IsDirectionReversed
        {
            get { return GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="RangeSlider"/> automatically moves the <see cref="Thumb"/> to the closest tick mark.
        /// </summary>
        public bool IsSnapToTickEnabled
        {
            get { return GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="Thumb"/> can overlap.
        /// </summary>
        public bool IsThumbOverlap
        {
            get { return GetValue(IsThumbOverlapProperty); }
            set { SetValue(IsThumbOverlapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interval between tick marks.
        /// </summary>
        public double TickFrequency
        {
            get { return GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates where to draw 
        /// tick marks in relation to the track.
        /// </summary>
        public TickPlacement TickPlacement
        {
            get { return GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _track = e.NameScope.Find<RangeTrack>("PART_Track");
            _backgroundButton = e.NameScope.Find<Button>("PART_BackgroundButton");
            _foregroundButton = e.NameScope.Find<Button>("PART_ForegroundButton");

            _backgroundButtonPressDispose?.Dispose();
            _foregroundButtonPressDispose?.Dispose();
            _backgroundButtonReleaseDispose?.Dispose();
            _foregroundButtonReleaseDispose?.Dispose();
            _pointerMovedDispose?.Dispose();
            _pointerReleasedDispose?.Dispose();
            _lowerThumbPointerMovedDispose?.Dispose();
            _upperThumbPointerMovedDispose?.Dispose();
            _lowerThumbPointerPressedDispose?.Dispose();
            _lowerThumbPointerReleaseDispose?.Dispose();
            _upperThumbPointerPressedDispose?.Dispose();
            _upperThumbPointerReleaseDispose?.Dispose();

            _backgroundButtonPressDispose = _backgroundButton?
                .AddDisposableHandler(PointerPressedEvent, (s, e) => TrackPressed(s, e, TrackButton.Background), RoutingStrategies.Tunnel);
            _backgroundButtonReleaseDispose = _backgroundButton?
                .AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);

            _foregroundButtonPressDispose = _foregroundButton?
                .AddDisposableHandler(PointerPressedEvent, (s, e) => TrackPressed(s, e, TrackButton.Foreground), RoutingStrategies.Tunnel);
            _foregroundButtonReleaseDispose = _foregroundButton?
                .AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);

            _lowerThumbPointerMovedDispose = _track.LowerThumb?
                .AddDisposableHandler(PointerMovedEvent, ThumbMoved, RoutingStrategies.Tunnel);
            _upperThumbPointerMovedDispose = _track.UpperThumb?
                .AddDisposableHandler(PointerMovedEvent, ThumbMoved, RoutingStrategies.Tunnel);

            _lowerThumbPointerPressedDispose = _track.LowerThumb?
                .AddDisposableHandler(PointerPressedEvent, (s, e) => ThumbPressed(s, e, TrackThumb.Lower), RoutingStrategies.Tunnel);
            _lowerThumbPointerReleaseDispose = _track.LowerThumb?
                .AddDisposableHandler(PointerReleasedEvent, ThumbReleased, RoutingStrategies.Tunnel);

            _upperThumbPointerPressedDispose = _track.UpperThumb?
                .AddDisposableHandler(PointerPressedEvent, (s, e) => ThumbPressed(s, e, TrackThumb.Upper), RoutingStrategies.Tunnel);
            _upperThumbPointerReleaseDispose = _track.UpperThumb?
                .AddDisposableHandler(PointerReleasedEvent, ThumbReleased, RoutingStrategies.Tunnel);

            _pointerMovedDispose = this.AddDisposableHandler(PointerMovedEvent, TrackMoved, RoutingStrategies.Tunnel);
            _pointerReleasedDispose = this.AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled || e.KeyModifiers != KeyModifiers.None) return;

            var handled = true;

            switch (e.Key)
            {
                case Key.Down:
                case Key.Left:
                    MoveToNextTick(IsDirectionReversed ? SmallChange : -SmallChange);
                    break;

                case Key.Up:
                case Key.Right:
                    MoveToNextTick(IsDirectionReversed ? -SmallChange : SmallChange);
                    break;

                case Key.PageUp:
                    MoveToNextTick(IsDirectionReversed ? -LargeChange : LargeChange);
                    break;

                case Key.PageDown:
                    MoveToNextTick(IsDirectionReversed ? LargeChange : -LargeChange);
                    break;

                case Key.Home:
                    LowerSelectedValue = Minimum;
                    break;

                case Key.End:
                    UpperSelectedValue = Maximum;
                    break;

                default:
                    handled = false;
                    break;
            }

            e.Handled = handled;
        }

        private void MoveToNextTick(double direction)
        {
            if (direction == 0.0) return;

            var value = LowerSelectedValue;

            // Find the next value by snapping
            var next = SnapToTick(Math.Max(Minimum, Math.Min(Maximum, value + direction)));

            var greaterThan = direction > 0; //search for the next tick greater than value?

            // If the snapping brought us back to value, find the next tick point
            if (Math.Abs(next - value) < Tolerance
                && !(greaterThan && Math.Abs(value - Maximum) < Tolerance) // Stop if searching up if already at Max
                && !(!greaterThan && Math.Abs(value - Minimum) < Tolerance)) // Stop if searching down if already at Min
            {
                var ticks = Ticks;

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if (ticks != null && ticks.Count > 0)
                {
                    foreach (var tick in ticks)
                    {
                        // Find the smallest tick greater than value or the largest tick less than value
                        if (greaterThan && MathUtilities.GreaterThan(tick, value) &&
                            (MathUtilities.LessThan(tick, next) || Math.Abs(next - value) < Tolerance)
                            || !greaterThan && MathUtilities.LessThan(tick, value) &&
                            (MathUtilities.GreaterThan(tick, next) || Math.Abs(next - value) < Tolerance))
                        {
                            next = tick;
                        }
                    }
                }
                else if (MathUtilities.GreaterThan(TickFrequency, 0.0))
                {
                    // Find the current tick we are at
                    var tickNumber = Math.Round((value - Minimum) / TickFrequency);

                    if (greaterThan)
                        tickNumber += 1.0;
                    else
                        tickNumber -= 1.0;

                    next = Minimum + tickNumber * TickFrequency;
                }
            }

            // Update if we've found a better value
            if (Math.Abs(next - value) > Tolerance)
            {
                LowerSelectedValue = next;
            }
        }

        private void ThumbPressed(object? sender, PointerPressedEventArgs e, TrackThumb trackThumb)
        {
            _isThumbDragging = true;
            _currentTrackThumb = trackThumb;
        }

        private void ThumbReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isThumbDragging = false;
            _currentTrackThumb = TrackThumb.None;
        }

        private void ThumbMoved(object? sender, PointerEventArgs e)
        {
            if (_isThumbDragging)
                MoveToPoint(e.GetCurrentPoint(this), _currentTrackThumb);
        }

        private void TrackPressed(object? sender, PointerPressedEventArgs e, TrackButton trackButton)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
                _previousValue = GetValueByPointOnTrack(e.GetCurrentPoint(this));
                _currentTrackThumb = GetNearestTrackThumb(e.GetCurrentPoint(this));

                if (trackButton == TrackButton.Background)
                    MoveToPoint(e.GetCurrentPoint(this), _currentTrackThumb);
                else if (RangeDraggedMode == RangeDraggedMode.MoveThumbsSeparately)
                    MoveToPoint(e.GetCurrentPoint(this), TrackThumb.Middle);
            }
        }

        private void TrackReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDragging = false;
            _currentTrackThumb = TrackThumb.None;
        }

        private void TrackMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging)
            {
                if (RangeDraggedMode == RangeDraggedMode.MoveThumbsSeparately)
                    MoveToPoint(e.GetCurrentPoint(this), _currentTrackThumb);
                else
                    MoveToPoint(e.GetCurrentPoint(this), TrackThumb.Both);
            }
        }

        private void MoveToPoint(PointerPoint x, TrackThumb trackThumb)
        {
            var value = GetValueByPointOnTrack(x);

            switch (trackThumb)
            {
                case TrackThumb.Upper:
                    UpperSelectedValue = IsSnapToTickEnabled ? SnapToTick(value) : value;
                    break;
                case TrackThumb.Lower:
                    LowerSelectedValue = IsSnapToTickEnabled ? SnapToTick(value) : value;
                    break;
                case TrackThumb.Middle:
                    if (Math.Abs(LowerSelectedValue - value) < Math.Abs(UpperSelectedValue - value))
                        LowerSelectedValue = IsSnapToTickEnabled ? SnapToTick(value) : value;
                    else
                        UpperSelectedValue = IsSnapToTickEnabled ? SnapToTick(value) : value;
                    break;
                case TrackThumb.Both:
                    var delta = value - _previousValue;
                    _previousValue = value;

                    if ((Math.Abs(LowerSelectedValue - Minimum) <= double.Epsilon && delta < 0.0)
                        || (Math.Abs(UpperSelectedValue - Maximum) <= double.Epsilon && delta > 0.0))
                        return;

                    LowerSelectedValue += delta;
                    UpperSelectedValue += delta;
                    break;
            }
        }

        private double GetValueByPointOnTrack(PointerPoint point)
        {
            var orient = Orientation == Orientation.Horizontal;
            var trackLength = orient ? _track.Bounds.Width : _track.Bounds.Height;
            var pointNum = orient ? point.Position.X : point.Position.Y;
            var thumbLength = orient ? _track.LowerThumb.Width : _track.LowerThumb.Height;

            // Just add epsilon to avoid NaN in case 0/0
            trackLength += double.Epsilon;

            if (IsThumbOverlap)
                thumbLength /= 2.0;

            if (pointNum <= thumbLength)
                return orient ? Minimum : Maximum;
            if (pointNum > trackLength - thumbLength)
                return orient ? Maximum : Minimum;

            trackLength -= 2.0 * thumbLength;
            pointNum -= thumbLength;

            var logicalPos = MathUtilities.Clamp(pointNum / trackLength, 0.0d, 1.0d);
            var invert = orient
                ? IsDirectionReversed ? 1 : 0
                : IsDirectionReversed ? 0 : 1;
            var calcVal = Math.Abs(invert - logicalPos);
            var range = Maximum - Minimum;
            var finalValue = calcVal * range + Minimum;

            return finalValue;
        }

        private TrackThumb GetNearestTrackThumb(PointerPoint x)
        {
            var value = GetValueByPointOnTrack(x);

            if (IsThumbOverlap && Math.Abs(LowerSelectedValue - UpperSelectedValue) < double.Epsilon
                && Math.Abs(LowerSelectedValue - Minimum) < double.Epsilon)
            {
                return TrackThumb.Upper;
            }
            else if (IsThumbOverlap && Math.Abs(LowerSelectedValue - UpperSelectedValue) < double.Epsilon
                && Math.Abs(UpperSelectedValue - Maximum) < double.Epsilon)
            {
                return TrackThumb.Lower;
            }
            else if (!IsThumbOverlap && Math.Abs(LowerSelectedValue - UpperSelectedValue) < double.Epsilon
                && value < LowerSelectedValue)
            {
                return TrackThumb.Lower;
            }
            else if (!IsThumbOverlap && Math.Abs(LowerSelectedValue - UpperSelectedValue) < double.Epsilon
                && value > UpperSelectedValue)
            {
                return TrackThumb.Upper;
            }
            else if (Math.Abs(LowerSelectedValue - value) < Math.Abs(UpperSelectedValue - value))
            {
                return TrackThumb.Lower;
            }
            else
            {
                return TrackThumb.Upper;
            }
        }

        protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
        {
            if (property == LowerSelectedValueProperty || property == UpperSelectedValueProperty)
            {
                DataValidationErrors.SetError(this, value.Error);
            }
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == OrientationProperty)
            {
                UpdatePseudoClasses(change.NewValue.GetValueOrDefault<Orientation>());
            }
        }

        /// <summary>
        /// Snap the input 'value' to the closest tick.
        /// </summary>
        /// <param name="value">Value that want to snap to closest Tick.</param>
        private double SnapToTick(double value)
        {
            if (IsSnapToTickEnabled)
            {
                var previous = Minimum;
                var next = Maximum;

                // This property is rarely set so let's try to avoid the GetValue
                var ticks = Ticks;

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if (ticks != null && ticks.Count > 0)
                {
                    foreach (var tick in ticks)
                    {
                        if (MathUtilities.AreClose(tick, value))
                        {
                            return value;
                        }

                        if (MathUtilities.LessThan(tick, value) && MathUtilities.GreaterThan(tick, previous))
                        {
                            previous = tick;
                        }
                        else if (MathUtilities.GreaterThan(tick, value) && MathUtilities.LessThan(tick, next))
                        {
                            next = tick;
                        }
                    }
                }
                else if (MathUtilities.GreaterThan(TickFrequency, 0.0))
                {
                    previous = Minimum + Math.Round((value - Minimum) / TickFrequency) * TickFrequency;
                    next = Math.Min(Maximum, previous + TickFrequency);
                }

                // Choose the closest value between previous and next. If tie, snap to 'next'.
                value = MathUtilities.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
            }

            return value;
        }

        private void UpdatePseudoClasses(Orientation o)
        {
            PseudoClasses.Set(":vertical", o == Orientation.Vertical);
            PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
        }
    }
}
