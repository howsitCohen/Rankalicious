using System.Collections.ObjectModel;
using System.ComponentModel;
using Caliburn.Micro;
using RankaliciousScraper;
using RankaliciousWPF.Model;
using RankaliciousWPF.Services;

namespace RankaliciousWPF.ViewModels
{

    public class ScraperViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Construction
        public ScraperViewModel()
        {
            scraperModel = new ScraperModel { SearchTerms = "online title search", NumOfResultsToReturn = 100};
        }
        #endregion 

        #region Members
        private ObservableCollection<Result> resultCollection;
        private ScraperModel scraperModel;
        private string urlToFind;
       
        #endregion

        #region Properties
        public ObservableCollection<Result> ResultCollection
        {
            get
            {
                return resultCollection; 
            }
            set
            {
                resultCollection = value;
                NotifyOfPropertyChange(() => ResultCollection);
                AggregatorProvider.Aggregator.PublishOnUIThread(new ResultsEvent(value));
                AggregatorProvider.Aggregator.PublishOnUIThread(new UrlChangedEvent(urlToFind));
            }

        }

        public ScraperModel ScraperModel
        {
            get { return scraperModel; }
            set { scraperModel = value; }
        }

        public string SearchTerms
        {
            get { return scraperModel.SearchTerms; }
            set
            {
                scraperModel.SearchTerms = value;
                NotifyOfPropertyChange(() => SearchTerms);
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

        #region memebers
        public void DoScrap()
        {
            ResultCollection = ScraperModel.Results;
            
        }
        #endregion
    }
}