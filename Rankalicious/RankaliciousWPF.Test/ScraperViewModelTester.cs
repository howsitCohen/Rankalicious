using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RankaliciousScraper;
using RankaliciousWPF.Events;
using RankaliciousWPF.Model;
using RankaliciousWPF.ViewModels;


namespace RankaliciousWPF.Test
{
    [TestClass]
    public class ScraperViewModelTester
    {

        [TestMethod]
        public void doScrap_Test()
        {
            //Arrange
            var testObject = new ScraperViewModel();
            
            //Act
            testObject.SearchEnabled = true;
            testObject.DoScrap();
            

            //Assert
            Assert.IsFalse(testObject.SearchEnabled);
            Assert.IsTrue(testObject.IsSearchInProgress);
        }

        [TestMethod]
        public void doScrap_SearchDisabled()
        {
            //Arrange
            var testObject = new ScraperViewModel();

            //Ac
            testObject.SearchEnabled = false;
            testObject.DoScrap();
            

            //Assert
            Assert.IsFalse(testObject.SearchEnabled);
            Assert.IsFalse(testObject.IsSearchInProgress);
        }
        
    }
}
