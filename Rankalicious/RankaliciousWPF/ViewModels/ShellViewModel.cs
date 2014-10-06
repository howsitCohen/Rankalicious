using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
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

        public void Close()
        {
            this.TryClose();
        }

        private const string WindowTitleDefault = "Rankalicious - By Will Cohen for InfoTrack";
 
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