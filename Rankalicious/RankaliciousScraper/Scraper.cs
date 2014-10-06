using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace RankaliciousScraper
{
    public class Scraper
    {

        /// <summary>
        /// Triggered when Results have been processed
        /// </summary>
        public delegate void ResultsProcessed(List<Result> results );

        /// <summary>
        /// Triggered when we initiate a search
        /// </summary>
        /// <param name="uri"></param>
        public delegate void SearchStarted();

        /// <summary>
        /// This event is fired when the Results have been scrapped and processed
        /// </summary>
        public event ResultsProcessed UpdateResultsProcessed;

        /// <summary>
        /// This event is fired when we initiate a search
        /// </summary>
        public event SearchStarted UpdateSearchStarted;


        private XmlDocument htmlDocument = new XmlDocument();
        /// <summary>
        /// Used to get the response stream of a google search request.
        /// </summary>
        /// <param name="searchTerms">The search terms that will be used for the google search request</param>
        /// <param name="numOfResults">The number of results returned in the response</param>
        /// <returns></returns>
        private string GetGoogleSearchResponse(string searchTerms, int numOfResults)
        {
            string googleSearchUrl = "https://www.google.com.au/search?num="+numOfResults+"&q="+searchTerms.Trim().Replace(' ','+');
            
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(googleSearchUrl);

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return StreamToString(response.GetResponseStream());
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (HttpListenerException ex)
            {
                throw ex;
            }
            return "";
        }

        private string StreamToString(Stream stream)
        {
            Encoding encode = Encoding.GetEncoding("utf-8");

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(stream, encode);
            Debug.WriteLine("\r\nResponse stream received.");
            string result = "";
            Char[] read = new Char[256];

            // Reads 256 characters at a time.     
            int count = readStream.Read(read, 0, 256);
            Debug.WriteLine("HTML...\r\n");

            while (count > 0)
            {
                // Dumps the 256 characters on a string
                String str = new String(read, 0, count);
                result += str;
                count = readStream.Read(read, 0, 256);
            }
            return result;
        }

        /// <summary>
        /// Recursive function that hijacks MS exceptions throw and their detailed Exception messages to fix non compliant tags
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fixMalformedXML"></param>
        private void GetResponseXml(string response, bool fixMalformedXML = false)
        {
            //Inital source is passed in, since is a recursive function which attempts to fix all xml parsing error, we can only extract the google search result nodes on the first pass.
            if (fixMalformedXML == false)
            {
                response = response.Substring(15);
                response = response.Substring(response.IndexOf("<div id=\"search\""),
                response.Length - response.IndexOf("<div id=\"search\""));
                //Remove all the rubbish html encodings which messing with response parsing to Xml
                response = response.Replace("&nbsp;", " ");
                response = response.Replace("&amp;", "and");
                response = response.Replace("&quot;", "\"");
                response = response.Replace("&middot;", "-");
                response = response.Replace("<br>", " ");
                response = response.Replace("<\br>", " ");
                response = response.Replace("\r", " ");
                response = response.Replace("\n", " ");
            }
            try
            {
                //try to parse the response.
                htmlDocument.Load(new StringReader(response));
            }
            catch (XmlException ex)
            {
                string message = ex.Message;
                int linePositionToFix = ex.LinePosition;

                // Get the non compliant tags out of the exception message -HAha- Thanks MS Guys
                var result = from Match match in Regex.Matches(message, "\'([^\']*)\'")
                    select match.ToString();

                if (message.Contains("Unexpected end tag"))
                {
                    response = response.Remove(linePositionToFix - 3);
                }
                else if (message.Contains("cannot be included in a name"))
                {
                    response = response.Remove(linePositionToFix-1,1);
                }
                else if (message.Contains("is an unexpected token."))
                {
                    if (message.Contains("Expecting white space"))
                    {
                        response = response.Remove(linePositionToFix - 1, result.First().Length).Insert(linePositionToFix - 1, " ");
                    }
                    else
                    {
                        response = response.Remove(linePositionToFix - 1, result.First().Length).Insert(linePositionToFix - 1, result.Last().Trim('\'')); 
                    }
                }
                else if (message.Contains("Name cannot begin with the"))
                {
                    response = response.Remove(linePositionToFix - 1, result.First().Length).Insert(linePositionToFix - 1, "");
                }
                else if (message.Contains("An error occurred while parsing EntityName"))
                {
                    response = response.Remove(linePositionToFix - 2, 1);
                }
                else
                {
                    response = response.Insert(linePositionToFix - 3, "</" + result.First().Trim('\'') + ">");
                }
                GetResponseXml(response, true);
            }
        }


        /// <summary>
        /// Performs the heavingGiven the two parameters, will return a List of Res
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <param name="numOfResults"></param>
        /// <returns></returns>
        public List<Result> GetResultsList(string searchTerms = "online+title+search", int numOfResults = 100)
        {
            string googleSearchResponseSource = "";
            var results = new List<Result>();
            if (UpdateSearchStarted != null)
            {
                UpdateSearchStarted();
            } 
            try
            {
                googleSearchResponseSource = GetGoogleSearchResponse(searchTerms, numOfResults);
            }
            catch (WebException ex)
            {
                results.Add(new Result(){DateForResult = DateTime.Now, Description = "",Position = null, Status = ResultStatus.WebRepsonseError,Title = "Check that you have Internet Access",Url=""});
            }

            try
            {
                GetResponseXml(googleSearchResponseSource, false);
                var xDoc = htmlDocument.ToXDocument();

                var listNodes = from nodes in xDoc.Descendants("li") where nodes.GetAttributeValue("class") == "g" select nodes;
                int position = 1;
                foreach (var li in listNodes)
                {
                    var result = new Result();
                    var title = from nodes in li.Descendants("h3") where nodes.GetAttributeValue("class") == "r" select nodes.GetElementValue("a");
                    if (title.Any())
                    {
                        result.Title = title.ElementAt(0);
                    }
                    var url = from nodes in li.Descendants("div") where (nodes.GetAttributeValue("class") == "kv") select nodes.GetElementValue("cite");
                    var url2 = from nodes in li.Descendants("cite") where (nodes.GetAttributeValue("class") == "kv") select nodes.Value;
                    if (url.Any())
                    {
                        result.Url = url.ElementAt(0);
                    }
                    else if (url2.Any())
                    {
                        result.Url = url2.ElementAt(0);
                    }
                    else
                    {
                        result.Url = "";
                    }
                    var description = from nodes in li.Descendants("div") where nodes.GetAttributeValue("class") == "s" select nodes.GetElementValue("span");
                    if (description.Any())
                    {
                        result.Description = description.ElementAt(0);
                    }
                    if ((result.Title.Length > 0) && (results.Count < 100))
                    {
                        result.DateForResult = DateTime.UtcNow;
                        result.Position = position;
                        position++;
                        results.Add(result);
                    }
                }
            }
            //Hack job to have the error messages displayed in the results datagrid
            catch (InvalidOperationException ex)
            {
                results.Add(new Result() { DateForResult = DateTime.Now, Description = "", Position = null, Status = ResultStatus.XmlParseError, Title = "Error Parsing Reponse stream", Url = "" });
            }
            catch (ArgumentNullException ex)
            {
                results.Add(new Result(){DateForResult = DateTime.Now,Description = "",Position = null,Status = ResultStatus.XmlParseError,Title = "Error Parsing Reponse stream",Url = ""});
            }
            catch (ArgumentOutOfRangeException ex)
            {
                results.Add(new Result() { DateForResult = DateTime.Now, Description = "", Position = null, Status = ResultStatus.XmlParseError, Title = "Error Parsing Reponse stream", Url = "" });
            }
            
            if (UpdateResultsProcessed != null)
            {
                UpdateResultsProcessed(results);
            }
            return results;
        }
    }

    /// <summary>
    /// Helper class for XML Doc type conversions and getting element values
    /// </summary>
    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        public static string GetAttributeValue(this XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(attributeName);
            return attribute != null ? attribute.Value : string.Empty;
        }

        public static string GetElementValue(this XElement element)
        {
            return element != null ? element.Value : string.Empty;
        }

        public static string GetElementValue(this XElement element, string elementName)
        {
            XElement child = element.Element(elementName);
            return child != null ? child.Value : string.Empty;
        }


    }

    /// <summary>
    /// Search Result class
    /// </summary>
    public class Result
    {
        public int? Position { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateForResult { get; set; }
        public ResultStatus Status { get; set; }
    }

    public enum ResultStatus
    {
        Ok,
        XmlParseError,
        WebRepsonseError
        
    }

}
