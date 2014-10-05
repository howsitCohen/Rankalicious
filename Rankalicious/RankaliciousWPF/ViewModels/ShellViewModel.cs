using System.ComponentModel.Composition;
using Caliburn.Micro;
using RankaliciousWPF.Views;

namespace RankaliciousWPF.ViewModels
{

    static class AggregatorProvider
    {
        // The event aggregator
        public static EventAggregator Aggregator = new EventAggregator();
    }

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
        private ResultsViewModel _resultsView;
        private string _windowTitle = WindowTitleDefault;

        public ShellViewModel()
        {
            ScraperView = new ScraperViewModel();
            ResultsView = new ResultsViewModel();
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

        public ResultsViewModel ResultsView
        {
            get { return _resultsView; }
            set
            {
                _resultsView = value;
                NotifyOfPropertyChange(() => ResultsView);
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