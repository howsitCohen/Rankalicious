using System.ComponentModel.Composition;
using Caliburn.Micro;
using RankaliciousWPF.Views;

namespace RankaliciousWPF.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
        private readonly IObservableCollection<FlyoutBaseViewModel> flyouts =
            new BindableCollection<FlyoutBaseViewModel>();

        public IObservableCollection<FlyoutBaseViewModel> Flyouts
        {
            get
            {
                return this.flyouts;
            }
        }

        public void Close()
        {
            this.TryClose();
        }

        public void ToggleFlyout(int index)
        {
            var flyout = this.flyouts[index];
            flyout.IsOpen = !flyout.IsOpen;
        }

        private const string WindowTitleDefault = "Rankalicious";
 
        private ScraperViewModel _scraperView;
        private string _windowTitle = WindowTitleDefault;

        public ShellViewModel()
        {
            ScraperView = new ScraperViewModel();
        }

        public ScraperViewModel ScraperView
        {
            get { return _scraperView; }
            set
            {
                _scraperView = value;
                NotifyOfPropertyChange(() => ScraperView);
            }
        }
 
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

    }
}