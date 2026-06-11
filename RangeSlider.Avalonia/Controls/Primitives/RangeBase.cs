using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace RangeSlider.Avalonia.Controls.Primitives;

/// <summary>
/// Base class for controls that display a value within a range.
/// </summary>
public abstract class RangeBase : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DirectProperty<RangeBase, double> MinimumProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(Minimum),
            o => o.Minimum,
            (o, v) => o.Minimum = v);

    /// <summary>
    /// Defines the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DirectProperty<RangeBase, double> MaximumProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(Maximum),
            o => o.Maximum,
            (o, v) => o.Maximum = v);

    /// <summary>
    /// Defines the <see cref="LowerSelectedValue"/> property.
    /// </summary>
    public static readonly DirectProperty<RangeBase, double> LowerSelectedValueProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(LowerSelectedValue),
            o => o.LowerSelectedValue,
            (o, v) => o.LowerSelectedValue = v,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="UpperSelectedValue"/> property.
    /// </summary>
    public static readonly DirectProperty<RangeBase, double> UpperSelectedValueProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(UpperSelectedValue),
            o => o.UpperSelectedValue,
            (o, v) => o.UpperSelectedValue = v,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="SmallChange"/> property.
    /// </summary>
    public static readonly StyledProperty<double> SmallChangeProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(SmallChange), 1);

    /// <summary>
    /// Defines the <see cref="LargeChange"/> property.
    /// </summary>
    public static readonly StyledProperty<double> LargeChangeProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(LargeChange), 10);

    private bool _upperValueInitializedNonZeroValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeBase"/> class.
    /// </summary>
    public RangeBase()
    {
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double Minimum
    {
        get;

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                SetAndRaise(MinimumProperty, ref field, value);
                Maximum = ValidateMaximum(Maximum);
                LowerSelectedValue = ValidateLowerValue(LowerSelectedValue);
                UpperSelectedValue = ValidateUpperValue(UpperSelectedValue);
            }
            else
            {
                SetAndRaise(MinimumProperty, ref field, value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Maximum
    {
        get;

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                value = ValidateMaximum(value);
                SetAndRaise(MaximumProperty, ref field, value);
                LowerSelectedValue = ValidateLowerValue(LowerSelectedValue);
                UpperSelectedValue = ValidateUpperValue(UpperSelectedValue);
            }
            else
            {
                SetAndRaise(MaximumProperty, ref field, value);
            }
        }
    } = 100.0;

    /// <summary>
    /// Gets or sets the lower selected value.
    /// </summary>
    public double LowerSelectedValue
    {
        get;

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                value = ValidateLowerValue(value);
                SetAndRaise(LowerSelectedValueProperty, ref field, value);
            }
            else
            {
                SetAndRaise(LowerSelectedValueProperty, ref field, value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the upper selected value.
    /// </summary>
    public double UpperSelectedValue
    {
        get;

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                value = ValidateUpperValue(value);
                _upperValueInitializedNonZeroValue = value > 0.0;
                SetAndRaise(UpperSelectedValueProperty, ref field, value);
            }
            else
            {
                SetAndRaise(UpperSelectedValueProperty, ref field, value);
            }
        }
    }

    public double SmallChange
    {
        get => GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    public double LargeChange
    {
        get => GetValue(LargeChangeProperty);
        set => SetValue(LargeChangeProperty, value);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Maximum = ValidateMaximum(Maximum);
        LowerSelectedValue = ValidateLowerValue(LowerSelectedValue);
        UpperSelectedValue = ValidateUpperValue(UpperSelectedValue);
    }

    /// <summary>
    /// Checks if the double value is not inifinity nor NaN.
    /// </summary>
    /// <param name="value">The value.</param>
    private static bool ValidateDouble(double value)
    {
        return !double.IsInfinity(value) || !double.IsNaN(value);
    }

    /// <summary>
    /// Validates/coerces the <see cref="Maximum"/> property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The coerced value.</returns>
    private double ValidateMaximum(double value)
    {
        return Math.Max(value, Minimum);
    }

    /// <summary>
    /// Validates/coerces the <see cref="LowerSelectedValue"/> property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The coerced value.</returns>
    private double ValidateLowerValue(double value)
    {
        return _upperValueInitializedNonZeroValue
            ? Math.Clamp(value, Minimum, UpperSelectedValue)
            : Math.Clamp(value, Minimum, Maximum);
    }

    /// <summary>
    /// Validates/coerces the <see cref="UpperSelectedValue"/> property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The coerced value.</returns>
    private double ValidateUpperValue(double value)
    {
        return Math.Clamp(value, LowerSelectedValue, Maximum);
    }
}