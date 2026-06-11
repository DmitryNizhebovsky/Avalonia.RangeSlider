namespace RangeSlider.Avalonia.Utils;

internal static class MathUtils
{
    /// <summary>
    /// AreClose - Returns whether or not two doubles are "close".  That is, whether or
    /// not they are within epsilon of each other.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool AreClose(double value1, double value2)
    {
        if (value1 == value2)
            return true;
        double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.220446049250313E-16;
        double num2 = value1 - value2;
        return -num1 < num2 && num1 > num2;
    }
    
    /// <summary>
    /// LessThanOrClose - Returns whether or not the first double is less than or close to
    /// the second double.  That is, whether or not the first is strictly less than or within
    /// epsilon of the other number.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool LessThanOrClose(double value1, double value2)
    {
        return value1 < value2 || AreClose(value1, value2);
    }
    
    /// <summary>
    /// LessThan - Returns whether or not the first double is less than the second double.
    /// That is, whether or not the first is strictly less than *and* not within epsilon of
    /// the other number.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool LessThan(double value1, double value2)
    {
        return value1 < value2 && !AreClose(value1, value2);
    }
    
    /// <summary>
    /// GreaterThan - Returns whether or not the first double is greater than the second double.
    /// That is, whether or not the first is strictly greater than *and* not within epsilon of
    /// the other number.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool GreaterThan(double value1, double value2)
    {
        return value1 > value2 && !AreClose(value1, value2);
    }
    
    /// <summary>
    /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to
    /// the second double.  That is, whether or not the first is strictly greater than or within
    /// epsilon of the other number.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool GreaterThanOrClose(double value1, double value2)
    {
        return value1 > value2 || AreClose(value1, value2);
    }
}