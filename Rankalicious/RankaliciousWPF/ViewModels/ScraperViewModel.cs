using System.Collections.ObjectModel;
using System.ComponentModel;
using Caliburn.Micro;
using RankaliciousWPF.Events;
using RankaliciousWPF.Model;

namespace RankaliciousWPF.ViewModels
{

    public class ScraperViewModel : Caliburn.Micro.PropertyChangedBase, INotifyPropertyChanged, IDataErrorInfo,IHandle<ResultsEvent>
    {
        

        #region Construction
        public ScraperViewModel()
        {
            eventAggregator = AggregatorProvider.Aggregator;
            eventAggregator.Subscribe(this);
            scraperModel = new ScraperModel { SearchTerms = "online title search", NumOfResultsToReturn = 100};
        }
        #endregion 

        #region Members
        private ObservableCollection<RankaliciousScraper.Result> resultCollection;
        private readonly IEventAggregator eventAggregator;
        private ScraperModel scraperModel;
        private string urlToFind;
        private bool searchEnabled;
        private bool isSearchInProgress;
       
        #endregion

        #region Properties

        public string this[string columnName]
        {
            get
            {
                if (columnName == "SearchTerms" && this.SearchTerms.Length == 0)
                {
                    SearchEnabled = false;
                    NotifyOfPropertyChange(() => SearchEnabled);
                    return "Search terms required for search";
                }
                else if (columnName == "SearchTerms" && this.SearchTerms.Length > 0)
                {
                    SearchEnabled = true;
                    NotifyOfPropertyChange(() => SearchEnabled);
                }
                return null;
            }
        }

        public bool IsSearchInProgress
        {
            get { return scraperModel.Running; }
            set
            {
                scraperModel.Running = value;
                NotifyOfPropertyChange(() => IsSearchInProgress);
                RaisePropertyChanged("IsSearchInProgress");
                if (isSearchInProgress)
                {
                    SearchEnabled = false;
                    NotifyOfPropertyChange(() => SearchEnabled);
                    RaisePropertyChanged("SearchEnabled");
                }
               
            }

        }

        public int NumOfResultsToReturn
        {
            get { return scraperModel.NumOfResultsToReturn; }
            set
            {
                scraperModel.NumOfResultsToReturn = value;
                NotifyOfPropertyChange(() => NumOfResultsToReturn);
            }
        }

        public ObservableCollection<RankaliciousScraper.Result> ResultCollection
        {
            get
            {
                return scraperModel.Results; 
            }
        }

        public ScraperModel ScraperModel
        {
            get { return scraperModel; }
            set { scraperModel = value; }
        }

        public bool SearchEnabled
        {
            get { return searchEnabled; }
            set
            {
                searchEnabled = value;
                NotifyOfPropertyChange(() => SearchEnabled);
                RaisePropertyChanged("SearchEnabled");
            }
        }

        public string SearchTerms
        {
            get { return scraperModel.SearchTerms; }
            set
            {
                if (Equals(value, scraperModel.SearchTerms))
                {
                    return;
                }
                scraperModel.SearchTerms = value;
                NotifyOfPropertyChange(() => SearchTerms);
                RaisePropertyChanged("SearchTermsEmpty");
            }
        }

        public string UrlToFind
        {
            get { return urlToFind; }
            set
            {
                urlToFind = value;
                NotifyOfPropertyChange(() => UrlToFind);
                AggregatorProvider.Aggregator.PublishOnUIThread(new UrlChangedEvent(urlToFind));
            }
        }

        #endregion

        #region INotifyPropertyChanged memebers

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region memebers
        public void DoScrap()
        {
            if (SearchEnabled == true)
            {
                SearchEnabled = false;
                IsSearchInProgress = true;
                ScraperModel.GetSearchResults();
            }
        }
        #endregion

        public string Error { get { return string.Empty; } }

        // When we receive a SelectionChangedMessage...
        public void Handle(ResultsEvent events)
        {
            SearchEnabled = true;
            IsSearchInProgress = false;
        }

    }
}