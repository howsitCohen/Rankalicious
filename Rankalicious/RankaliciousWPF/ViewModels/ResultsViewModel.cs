using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using RankaliciousScraper;
using RankaliciousWPF.Events;
using RankaliciousWPF.Model;
using RankaliciousWPF.Services;

namespace RankaliciousWPF.ViewModels
{
    public class ResultsViewModel : PropertyChangedBase, IHandle<ResultsEvent>, IHandle<UrlChangedEvent>
    {

        #region memebers
        private readonly IEventAggregator eventAggregator;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Result> resultCollection;
        private string urlFilter;
        #endregion

        public ResultsViewModel()
        {
            eventAggregator = AggregatorProvider.Aggregator;
            AggregatorProvider.Aggregator.Subscribe(this);

        }
        

        #region Properties
        public ObservableCollection<RankaliciousScraper.Result> ResultCollection
        {
            get
            {
                return resultCollection;
            }
            set
            {
                resultCollection = value;
                eventAggregator.PublishOnUIThread(true);
                NotifyOfPropertyChange(() => ResultCollection);
            }
        }
       

        public ICollectionView ResultCollectionView { get; set; }

        public string UrlFilter
        {
            get { return urlFilter; }
            set
            {
                urlFilter = value;
                //Remove inserted  result element which displays desired URL to Find isn't found in google. 
                if (ResultCollection != null)
                {
                    if (ResultCollection.ElementAt(0).Position == 0)
                    {
                        ResultCollection.Remove(ResultCollection.ElementAt(0));
                    }

                    ResultCollectionView = CollectionViewSource.GetDefaultView(ResultCollection);


                    if (!String.IsNullOrEmpty(urlFilter))
                    {

                        ResultCollectionView.Filter =
                            x =>
                                CultureInfo.CurrentCulture.CompareInfo.IndexOf(((RankaliciousScraper.Result)x).Url, UrlFilter,
                                    CompareOptions.IgnoreCase) != -1;
                    }
                    else
                    {
                        ResultCollectionView.Filter = null;
                    }

                    if (ResultCollectionView.IsEmpty && ResultCollection != null)
                    {
                        ResultCollection.Insert(0,
                            new RankaliciousScraper.Result()
                            {
                                DateForResult = DateTime.Now,
                                Description = "",
                                Position = 0,
                                Title = "No Google ranking available for:",
                                Url = UrlFilter
                            });
                    }
                }
            }
        }

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


        #region Event Handlers
        // When we receive a SelectionChangedMessage...
        public void Handle(ResultsEvent events)
        {
            ResultCollection = events.Results;
        }
        // When we receive a SelectionChangedMessage...
        public void Handle(UrlChangedEvent events)
        {
            UrlFilter = events.Url;
        }
        #endregion
    }
}