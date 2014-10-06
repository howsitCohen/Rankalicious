using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RankaliciousScraper;
using RankaliciousWPF.Events;
using RankaliciousWPF.Services;
using RankaliciousWPF.ViewModels;

namespace RankaliciousWPF.Model
{
    public class ScraperModel
    {
        
        public ScraperModel()
        {
            scraper.UpdateSearchStarted += ScraperOnUpdateSearchStarted;
            scraper.UpdateResultsProcessed += ScraperOnUpdateResultsProcessed;
        }

        private void ScraperOnUpdateResultsProcessed(List<Result> results)
        {
            var resultsCollection = new ObservableCollection<Result>(results);
            AggregatorProvider.Aggregator.PublishOnUIThread(new ResultsEvent(resultsCollection));            
            Results = resultsCollection;
        }

        private void ScraperOnUpdateSearchStarted()
        {
            Running = true;
        }

        #region Members
        private Thread WorkerThread;
        private Scraper scraper = new Scraper();
        private string searchTerms = "";
        private int numOfResultsToReturn;
        private ObservableCollection<Result> results ;
        private bool running = false;
        #endregion
        
        #region Properties
        
        public int NumOfResultsToReturn
        {
            get { return numOfResultsToReturn; }
            set { numOfResultsToReturn = value; }
        }

        public bool Running
        {
            get
            {
                return running;
            }
            set { running = value; }
        }

        public string SearchTerms
        {
            get { return searchTerms; }
            set { searchTerms = value; }
        }
        public ObservableCollection<Result> Results
        {
            get
            {
                return results;
            }
            set { results = value; }
        }
        #endregion

        #region Methods
        public void GetSearchResults()
        {
            WorkerThread = new Thread(BackgroundScrap);
            if (!WorkerThread.IsAlive)
            {
                Running = true;
                WorkerThread.Start();
            }
        }

        private void BackgroundScrap()
        {
            scraper.GetResultsList(SearchTerms, NumOfResultsToReturn);
            Running = false;
        }
        #endregion 
    }
}
