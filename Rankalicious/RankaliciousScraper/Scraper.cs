using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RankaliciousScraper
{
    public class Scraper
    {
        public XmlDocument htmlDocument = new XmlDocument();
        /// <summary>
        /// Used to get the response stream of a google search request.
        /// </summary>
        /// <param name="searchTerms">The search terms that will be used for the google search request</param>
        /// <param name="numOfResults">The number of results returned in the response</param>
        /// <returns></returns>
        public string GetGoogleSearchResponse(string searchTerms = "online+title+search", int numOfResults = 100)
        {
            string googleSearchUrl = "https://www.google.com.au/search?num="+numOfResults+"&q="+searchTerms.Trim().Replace(' ','+');
            
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(googleSearchUrl);

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return SaveGoogleRepsonse(response.GetResponseStream());
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                throw;
            }
            return "";
        }

        private string SaveGoogleRepsonse(Stream stream)
        {
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
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
                // Dumps the 256 characters on a string and displays the string to the console.
                String str = new String(read, 0, count);
                result += str;
                count = readStream.Read(read, 0, 256);
            }
            return result;
        }

        public void GetResponseXml(string response, bool fixMalformedXML = false)
        {
            if (fixMalformedXML == false)
            {

                int length = response.IndexOf("<div style=\"clear:both;margin-bottom:17px;overflow:hidden\">") -
                             response.IndexOf("<div id=\"search\"");
                response = response.Substring(response.IndexOf("<div id=\"search\""), length);
            }
            response = response.Replace("&nbsp;", " ");
            response = response.Replace("&amp;", "and");
            response = response.Replace("&quot;", "\"");
            response = response.Replace("&middot;", ".");
            response = response.Replace("&#39;", "'");
            response = response.Replace("&#257;", " ");
            response = response.Replace("&#8206;", " ");
            response = response.Replace("<br>", " ");
            response = response.Replace("<\br>", " ");
            response = response.Replace("\r", " ");
            response = response.Replace("\n", " ");
            try
            {
                htmlDocument.Load(new StringReader(response));
            }
            catch (XmlException ex)
            {
                string message = ex.Message;
                int linenumberToFix = ex.LinePosition;
                int linePositionToFix = ex.LinePosition;
                var result = from Match match in Regex.Matches(message, "\'([^\']*)\'")
                    select match.ToString();

                if (message.Contains("Unexpected end tag"))
                {
                    response = response.Remove(linePositionToFix - 3);
                }
                else
                {
                    response = response.Insert(linePositionToFix - 3, "</" + result.First().Trim('\'') + ">");
                }
                GetResponseXml(response, true);
            }
        }

        public void GetResultsObject(XmlDocument xDocument)
        {
            XmlNode resultNode;
            var results = new List<Result>();
            var xDoc = xDocument.ToXDocument();
            // Grab all the div tags
            var bodyNode = xDocument.GetElementsByTagName("div");
            var ListNodes = from nodes in xDoc.Descendants("li") where nodes.Attribute("class").Value == "g" select nodes;
            int position = 1;
            foreach (var li in ListNodes)
            {
                var result = new Result();
                var title = from nodes in li.Descendants("h3") where nodes.Attribute("class").Value == "r" select nodes.Element("a").Value;
                if (title != null)
                {
                    result.Title = title.ElementAt(0);
                }
                var url = from nodes in li.Descendants("div") where nodes.Attribute("class").Value == "kv" select nodes.Element("cite").Value;
                if (url != null)
                {
                    result.Url = url.ElementAt(0);
                }
                var description = from nodes in li.Descendants("div") where nodes.Attribute("class").Value == "s" select nodes.Element("span").Value;
                if (description != null)
                {
                    result.Description = description.ElementAt(0);
                }
                result.Position = position;
                results.Add(result);
                position++;
            }
            // Enumerate them for the search results group class "srg".. this is the div where google puts the organic results 
           
            string test = "";

           Debug.WriteLine(htmlDocument.InnerXml);

        }

    }

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
    }

    public class Result
    {
        public int Position { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateForResult { get; set; }
    }

}
