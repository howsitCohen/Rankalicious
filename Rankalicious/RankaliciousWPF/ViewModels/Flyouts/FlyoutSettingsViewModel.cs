using MahApps.Metro.Controls;

namespace RankaliciousWPF.ViewModels.Flyouts
{
    public class FlyoutSettingsViewModel : FlyoutBaseViewModel
    {
        public FlyoutSettingsViewModel()
        {
            this.Header = "settings";
            this.Position = Position.Right;
        }
    }
}