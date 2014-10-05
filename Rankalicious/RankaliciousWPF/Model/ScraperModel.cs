using System.Collections.Generic;
using System.Collections.ObjectModel;
using RankaliciousScraper;

namespace RankaliciousWPF.Model
{
    public class ScraperModel
    {
#region Members
        private Scraper scraper = new Scraper();
        private string searchTerms = "Test";
        private int numOfResultsToReturn;
#endregion
        
#region Properties
        public string SearchTerms
        {
            get { return searchTerms; }
            set { searchTerms = value; }
        }

        public int NumOfResultsToReturn
        {
            get { return numOfResultsToReturn; }
            set { numOfResultsToReturn = value; }
        }
       
        public ObservableCollection<Result> Results
        {
            get
            {
                return GetSearchResults();
            }
        }
        #endregion

        private ObservableCollection<Result> GetSearchResults()
        {
            List<Result> resultsList = scraper.GetResultsList(SearchTerms, NumOfResultsToReturn);
            ObservableCollection<Result> results = new ObservableCollection<Result>(resultsList);
            return results;
        }

    }
}
