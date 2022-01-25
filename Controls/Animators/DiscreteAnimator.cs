using Avalonia.Animation.Animators;

namespace ModernControlsForAvalonia.Controls.Animators
{
    public sealed class DiscreteAnimator : Animator<double>
    {
        private double? _oldValue = null;

        public override double Interpolate(double progress, double oldValue, double newValue)
        {
            if (_oldValue == null || _oldValue != oldValue)
                _oldValue = oldValue;

            return _oldValue.Value;
        }
    }
}
