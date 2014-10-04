using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RankaliciousScraper;
using System.Text;
using System.IO;

namespace RankaliciousScraper.Tests
{
    [TestClass]
    public class ScraperTester
    {
        [TestMethod]
        public void GetGoogleSource_BlueSky()
        {
            //ARRANGE
            var testObject = new Scraper();

            //ACT
            var googleSource = testObject.GetGoogleSearchResponse();
            
            //ASSERT
            Assert.IsNotNull(googleSource);    
        }

        [TestMethod]
        public void GetGoogleSource_WithCleanParameters()
        {
            //ARRANGE
            var testObject = new Scraper();
            string searchTerms = "online title search";
            int numOfResults = 100; 

            //ACT
            var googleSource = testObject.GetGoogleSearchResponse(searchTerms, numOfResults);

            //ASSERT
            Assert.IsNotNull(googleSource);
        }

        [TestMethod]
        public void GetGoogleSource_WithDirtyParameters()
        {
            //ARRANGE
            var testObject = new Scraper();
            string searchTerms = "online  /n title #%1 search";
            int numOfResults = -10; //negative

            //ACT
            var googleSource = testObject.GetGoogleSearchResponse(searchTerms, numOfResults);

            //ASSERT
            Assert.IsNotNull(googleSource);
        }

        [TestMethod]
        public void GetGoogleSource_WriteToConsole()
        {
            //ARRANGE
            var testObject = new Scraper();
            string searchTerms = "online  /n title #%1 search";
            int numOfResults = -10; //negative

            //ACT
            var googleSource = testObject.GetGoogleSearchResponse(searchTerms, numOfResults);
            


            //ASSERT
            Assert.IsNotNull(googleSource);
        }

        [TestMethod]
        public void GetGoogleSource_GetSearchResults()
        {
            //ARRANGE
            var testObject = new Scraper();
            string searchTerms = "online title search";
            int numOfResults = 100; //negative

            //ACT
            var googleSearchResultsSource = testObject.GetGoogleSearchResponse(searchTerms, numOfResults);
            testObject.GetResponseXml(googleSearchResultsSource);
            testObject.GetResultsObject(testObject.htmlDocument);

            //ASSERT
            Assert.IsNotNull(googleSearchResultsSource);
        }



    }
}
