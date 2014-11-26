using System;
using System.Collections.Generic;
using System.Linq;
using EasyHttp.Http;
using HtmlAgilityPack;

namespace Spidie
{
    class Program
    {
        static void Main(string[] args)
        {
            var top100Websites = GetTop100WebsiteLinks();
            var httpClient = CreateHttpClient();

            Console.WriteLine("Starting Cache-Control verification. Press CTRL + C to quit...");
            Console.WriteLine();

            foreach (var link in top100Websites)
            {
                string cacheControlSetting;

                try
                {
                    Console.WriteLine(link);

                    var response = httpClient.Get(link);

                    cacheControlSetting = !string.IsNullOrEmpty(response.RawHeaders["Cache-Control"])
                                              ? response.RawHeaders["Cache-Control"]
                                              : "Cache-Control header not found";
                }
                catch (Exception)
                {
                    cacheControlSetting = "Host unreachable";
                }

                Console.WriteLine(cacheControlSetting);
                Console.WriteLine();
            }

            Console.WriteLine("...End of Cache-Control verification");
        }

        private static IEnumerable<string> GetTop100WebsiteLinks()
        {
            var doc = new HtmlWeb().Load("http://afrodigit.com/visited-websites-world/");

            return doc.DocumentNode.SelectSingleNode("//table")
                                              .Descendants("a")
                                              .Select(a => a.GetAttributeValue("href", null))
                                              .Where(u => !String.IsNullOrEmpty(u));
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.Request.Accept = HttpContentTypes.TextHtml;
            httpClient.Request.Timeout = 5000;
            httpClient.Request.UserAgent = "SpidYApp";
            return httpClient;
        }
    }
}