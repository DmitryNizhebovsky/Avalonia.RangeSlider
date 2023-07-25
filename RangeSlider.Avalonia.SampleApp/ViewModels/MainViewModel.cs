using ReactiveUI;

namespace RangeSlider.Avalonia.SampleApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        LowerSelected = 25d;
        UpperSelected = 75d;
    }

    public double LowerSelected
    {
        get => lowerSelected;
        set
        { 
            this.RaiseAndSetIfChanged(ref lowerSelected, value);
            LowerSelectedStr = lowerSelected.ToString("0.00");
        }
    }

    public double UpperSelected
    {
        get => upperSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref upperSelected, value);
            UpperSelectedStr = upperSelected.ToString("0.00");
        }
    }

    public string? LowerSelectedStr
    {
        get => lowerSelectedStr;
        set => this.RaiseAndSetIfChanged(ref lowerSelectedStr, value);
    }

    public string? UpperSelectedStr
    {
        get => upperSelectedStr;
        set => this.RaiseAndSetIfChanged(ref upperSelectedStr, value);
    }

    double lowerSelected;
    double upperSelected;

    string? lowerSelectedStr;
    string? upperSelectedStr;
}