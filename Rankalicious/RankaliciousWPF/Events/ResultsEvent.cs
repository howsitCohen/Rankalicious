using System.Collections.ObjectModel;
using RankaliciousScraper;

namespace RankaliciousWPF.Events
{
    public class ResultsEvent
    {
        public ResultsEvent(ObservableCollection<Result> results)
        {
            
            Results = results;
        }

        public ObservableCollection<Result> Results { get; private set; }
        
    }
}
