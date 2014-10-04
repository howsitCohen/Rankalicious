using System.Collections.ObjectModel;
using System.ComponentModel;
using RankaliciousScraper;
using RankaliciousWPF.Model;

namespace RankaliciousWPF.ViewModels
{
    public class ScraperViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Construction
        public ScraperViewModel()
        {
            scraperModel = new ScraperModel { SearchTerms = "online title search", NumOfResultsToReturn = 100, UrlToFind = "www.infotrack.com.au" };
        }
        #endregion 

        #region Members
        private ObservableCollection<Result> resultCollection;
        private ScraperModel scraperModel;
        #endregion

        #region Properties
        public ObservableCollection<Result> ResultCollection
        {
            get { return ScraperModel.GetSearchResults(); }
        }

        public ScraperModel ScraperModel
        {
            get { return scraperModel; }
            set { scraperModel = value; }
        }
        #endregion

        #region INotifyPropertyChanged memebers

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region memebers
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            } 
        }

        public void DoScrap()
        {
            ScraperModel.GetSearchResults();
        }

        private bool _canDisplayNote;
        private string _note = string.Empty;
        private string _noteDisplay = string.Empty;

        ///
        /// Indicates whether or not the Note can be displayed.
        ///
        public bool CanDisplayNote
        {
            get { return _canDisplayNote; }
            set
            {
                _canDisplayNote = value;
                NotifyOfPropertyChange(() => CanDisplayNote);
            }
        }

        ///
        /// The Journal's Note.
        ///
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                NotifyOfPropertyChange(() => Note);
            }
        }

        ///
        /// The note to display.
        ///
        public string NoteDisplay
        {
            get { return _noteDisplay; }
            set
            {
                _noteDisplay = value;
                NotifyOfPropertyChange(() => NoteDisplay);
            }
        }

        ///
        /// Displays the note.
        ///
        public void DisplayNote()
        {
            NoteDisplay = Note;
        }
        #endregion
    }
}