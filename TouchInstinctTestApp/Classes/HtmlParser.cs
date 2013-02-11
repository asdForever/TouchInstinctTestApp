using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TouchInstinctTestApp.Classes
{
    static class HtmlParser
    {
        public static List<FilmData> tempList = new List<FilmData>();

        public static void OnCallback(object s, HtmlDocumentLoadCompleted htmlDocumentLoadCompleted)
        {
            if (thereIsNoErrors(htmlDocumentLoadCompleted))
            {
                var htmlDocument = htmlDocumentLoadCompleted.Document;
                HtmlNodeCollection catalogItems = htmlDocument.DocumentNode.SelectNodes(
                    "//*[@class='catalog_item_middle_center']//a[starts-with(@href,'/films/movie/id')]//img[starts-with(@src,'http://')]");

                tempList = new List<FilmData>();

                if (catalogItems != null)
                {
                    foreach (HtmlNode item in catalogItems)
                    {
                        FilmData fd = new FilmData();
                        if (item.Attributes.Contains("src"))
                        {
                            fd.imageUrl = item.Attributes["src"].Value;
                        }
                        if (item.Attributes.Contains("alt"))
                        {
                            fd.name = item.Attributes["alt"].Value.Substring(6);
                        }
                        tempList.Add(fd);
                    }
                    Networking.getImagesAsync(tempList);
                }
            }
        }

        private static bool thereIsNoErrors(HtmlDocumentLoadCompleted htmlDocumentLoadCompleted)
        {
            if (htmlDocumentLoadCompleted.Error != null)
            {
                Networking.getHtmlFromUrl("http://stream.ru/films/index/page/" + MainPage.pageId + "/");
                return false;
            }
            else if (htmlDocumentLoadCompleted.Document.DocumentNode.InnerText.Contains("Ошибка 500"))
            {
                //Ошибка 500. Внутреняя ошибка сервера. Попробуйте перезагрузить страницу или вернуться на неё позднее (возможно, произошел случайный сбой).
                Networking.getHtmlFromUrl("http://stream.ru/films/index/page/" + MainPage.pageId + "/");
                return false;
            }

            return true;
        }
    }
}
