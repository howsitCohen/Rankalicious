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
        private string urlToFind;
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

        public string UrlToFind
        {
            get { return urlToFind; }
            set { urlToFind = value; }
        }
        #endregion

        public ObservableCollection<Result> GetSearchResults()
        {
            scraper.GetResponseXml(scraper.GetGoogleSearchResponse(searchTerms, numOfResultsToReturn));
            List<Result> resultsList = scraper.GetResultsObject(scraper.htmlDocument);
            ObservableCollection<Result> results = new ObservableCollection<Result>(resultsList);
            return results;
        }

    }
}
