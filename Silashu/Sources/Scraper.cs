using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace EmailScraper
{
    class Scraper
    {
        HtmlDocument htmlDoc = new HtmlDocument();

        //string pattern = @"^(?(")(".+?(?<!\\)"@)|(([0 - 9a - z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        string pattern = "[a-zA-z0-9\\.]+@[a-zA-z0-9\\.]+\\.[A-Za-z]+";

    //scrap a website
        public Target scrapUrl(String url)
        {
            Target result = new Target();
            Console.WriteLine(url);
            HtmlWeb web = new HtmlWeb();

            if (!url.Contains("http://") && !url.Contains("https://"))
                url = "http://" + url;
            htmlDoc = web.Load(url);
            String html = htmlDoc.DocumentNode.OuterHtml;

            result.Name = htmlDoc.DocumentNode.Element("html").Element("head").Element("title").InnerText;
            result.Url = url;
            result.Email = parse(html);

            return (result);
        }
        //parse the page with a string reference
        private String parse(String html)
        {
            String res;

            if (!String.IsNullOrEmpty(res = Regex.Match(html, pattern).Value))
                return res;
            return null;
        }
    }
}
