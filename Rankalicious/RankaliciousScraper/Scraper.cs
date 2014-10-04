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
            //Inital source is passed in, since is a recursive function which attempts to fix all xml parsing error, we can only extract the google search result nodes on the first pass.
            if (fixMalformedXML == false)
            {
               response = response.Substring(15);
               response = response.Substring(response.IndexOf("<div id=\"search\""), response.Length - response.IndexOf("<div id=\"search\""));
               response = response.Replace("&nbsp;", " ");
               response = response.Replace("&amp;", "and");
               response = response.Replace("&quot;", "\"");
               response = response.Replace("&middot;", "-");
               response = response.Replace("<br>", " ");
               response = response.Replace("<\br>", " ");
               response = response.Replace("\r", " ");
               response = response.Replace("\n", " ");

            }

            //Remove all the rubbish html encodings which messing with response parsing to Xml
            
            
            
            try
            {
                //try to parse the response.
                htmlDocument.Load(new StringReader(response));
            }
            catch (XmlException ex)
            {
                // 
                string message = ex.Message;
                int linenumberToFix = ex.LinePosition;
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
                }else
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
            var ListNodes = from nodes in xDoc.Descendants("li") where nodes.GetAttributeValue("class") == "g" select nodes;
            int position = 1;
            foreach (var li in ListNodes)
            {
                var result = new Result();
                var title = from nodes in li.Descendants("h3") where nodes.GetAttributeValue("class") == "r" select nodes.GetElementValue("a");
                if (title.Any())
                {
                    result.Title = title.ElementAt(0);
                }
                var url = from nodes in li.Descendants("div") where nodes.GetAttributeValue("class") == "kv" select nodes.GetElementValue("cite");
                if (url.Any())
                {
                    result.Url = url.ElementAt(0);
                }
                var description = from nodes in li.Descendants("div") where nodes.GetAttributeValue("class") == "s" select nodes.GetElementValue("span");
                if (description.Any())
                {
                    result.Description = description.ElementAt(0);
                }
                result.Position = position;
                result.DateForResult = DateTime.UtcNow;
                if (result.Title.Length > 0)
                {
                    results.Add(result);
                }
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

    public class Result
    {
        public int Position { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateForResult { get; set; }
    }

}
