using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RankaliciousScraper.Tests
{
    [TestClass]
    public class ScraperTester
    {

        private bool updateSearchStartedEventCalled = false;
        private bool updateResultsProcessedEventCalled = false;
        private List<Result> resultsList = new List<Result>();
        
        [TestMethod]
        public void GetResultsList_BlueSky()
        {
            //ARRANGE
            var testObject = new Scraper();

            //ACT
            var googleSource = testObject.GetResultsList();
            
            //ASSERT
            Assert.IsNotNull(googleSource);
            Assert.AreEqual(googleSource.Count,100);
        }

        [TestMethod]
        public void GetResultsList_WithCleanParameters()
        {
            //ARRANGE
            var testObject = new Scraper();
            string searchTerms = "online title search";
            int numOfResults = 100; 

            //ACT
            var googleResults = testObject.GetResultsList(searchTerms, numOfResults);

            //ASSERT
            Assert.IsNotNull(googleResults);
            Assert.AreEqual(googleResults.Count,numOfResults);
        }

        [TestMethod]
        public void GetResultsList_WithDirtyParameters()
        {
            //ARRANGE
            var testObject = new Scraper();
            string searchTerms = "online  /n title #%1 search";
            int numOfResults = -10; //negative

            //ACT
            var googleResults = testObject.GetResultsList(searchTerms, numOfResults);

            //ASSERT
            Assert.IsNotNull(googleResults);
            Assert.AreEqual(googleResults.Count, 0);
        }

        [TestMethod]
        public void GetResultsList_EventsCalled()
        {
            var testObject = new Scraper();
            testObject.UpdateSearchStarted += TestOnUpdateSearchStarted;
            testObject.UpdateResultsProcessed +=TestOnUpdateResultsProcessed;
            testObject.GetResultsList();

            Assert.IsTrue(updateSearchStartedEventCalled);
            Assert.IsTrue(updateResultsProcessedEventCalled);
        }

        [TestMethod]
        public void GetResultsList_EventsCalledAndResultsPassed()
        {
            var testObject = new Scraper();
            testObject.UpdateSearchStarted += TestOnUpdateSearchStarted;
            testObject.UpdateResultsProcessed += TestOnUpdateResultsProcessed;
            var googleResults = testObject.GetResultsList();

            Assert.IsTrue(updateSearchStartedEventCalled);
            Assert.IsTrue(updateResultsProcessedEventCalled);
            Assert.AreEqual(googleResults,resultsList);
        }
        
        private void TestOnUpdateResultsProcessed(List<Result> results)
        {
            updateResultsProcessedEventCalled = true;
            resultsList = results;
        }

        private void TestOnUpdateSearchStarted()
        {
            updateSearchStartedEventCalled = true;
        }



    }
}
