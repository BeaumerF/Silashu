using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;

namespace EmailScraper
{
    class Program
    {
        public static String urlPattern = @"www..*/";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.Error.WriteLine("No arguments found");
                    Thread.Sleep(1000);
                    return;
                }
                List<String> searchUrls = findUrls(args);
                if (searchUrls.Count == 0)
                {
                    Console.Error.WriteLine("No accessible links found");
                    Thread.Sleep(1000);
                    return;
                }
                List<Target> targets = findTargets(searchUrls);
                if (targets.Count == 0)
                {
                    Console.Error.WriteLine("No emails found");
                    Thread.Sleep(1000);
                    return;
                }
                Spreadsheet sheet = new Spreadsheet();
                sheet.SpreadsheetConvert(targets);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        //check if urls contain targets and return them in a list
        private static List<Target> findTargets(List<String> searchUrls)
        {
            List<Target> targets = new List<Target>();
            HtmlWeb hw = new HtmlWeb();

            String lastUrl = "lastUrl";
            foreach (var searchUrl in searchUrls)
            {
                // in comment, code to test a page localy

                //WebClient wc = new WebClient();
                //var page = wc.DownloadString(searchUrl);
                //HtmlDocument doc = hw.Load(page);
                //File.AppendAllText("D:/EmailScraper/EmailScraper/gpage4.html", doc.DocumentNode.OuterHtml);
                //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                //doc.Load("D:/EmailScraper/ConstructeurScraper/htmldebug/grandegooglepage2.html");

                HtmlDocument doc = hw.Load(searchUrl);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    Scraper scrap = new Scraper();
                    String url = link.Attributes["href"].Value;

                    if (!String.IsNullOrEmpty(url = Regex.Match(link.OuterHtml, urlPattern).Value))
                    {
                        int charLocation = url.IndexOf('/');
                        if (charLocation > 6)
                        {
                            url = url.Substring(0, charLocation);

                            if (url != lastUrl
                                &&
                                Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)
                                &&
                                PingWebsite("http://" + url))
                            {
                                lastUrl = url;
                                Target toCheck = scrap.scrapUrl(url);
                                if (!String.IsNullOrEmpty(toCheck.Email))
                                {
                                    targets.Add(toCheck);
                                    Console.WriteLine(toCheck.Email);
                                }
                            }
                        }
                    }
                }
            }
            return targets;
        }

        //check if an url is valid
        private static bool urlCheck(String arg)
        {

            Uri uriResult;
            bool result = Uri.TryCreate(arg, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(arg);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
            }
            return false;
        }

        //return urls which work in a list
        private static List<String> findUrls(String[] args)
        {
            List<String> searchUrls = new List<String>();

            foreach (var arg in args)
            {
                if (urlCheck(arg))
                    searchUrls.Add(arg);
            }
            return searchUrls;
        }

        //ping website
        private static bool PingWebsite(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = WebRequestMethods.Http.Head;

                using (var response = request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
