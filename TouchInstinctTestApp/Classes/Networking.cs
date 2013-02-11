using Amib.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Linq;

namespace TouchInstinctTestApp.Classes
{
    class Networking
    {
        public static void getHtmlFromUrl(string url)
        {
            if (isNetworkAvailable())
            {
                HtmlAgilityPack.HtmlWeb.LoadAsync(url, HtmlParser.OnCallback);
            }
        }
        public static void getImagesAsync(List<FilmData> tempFilmList)
        {
            if (isNetworkAvailable())
            {

                SmartThreadPool smartThreadPool = new SmartThreadPool(
                                                                        15 * 1000,   // Idle timeout in milliseconds
                                                                        25,         // Threads upper limit
                                                                        1);         // Threads lower limit
                var bw = new System.ComponentModel.BackgroundWorker();
                bw.DoWork += (s, args) =>
                {
                    foreach (FilmData item in tempFilmList)
                    {
                        if (IsolatedStorageHelper.isFileAlreadySaved(item.imageUrl))
                            ImageHelper.addDownloadedItemsToFilmCollection(item.imageUrl);
                        else
                            smartThreadPool.QueueWorkItem(() => getImageFromUrl(item.imageUrl));
                    }

                    smartThreadPool.WaitForIdle(); // Wait for the completion of all work items
                };
                bw.RunWorkerAsync();
            }
        }
        
        private static void getImageFromUrl(string url)
        {
            if (isNetworkAvailable())
            {
                if (!IsolatedStorageHelper.isFileAlreadySaved(url))
                {
                    Uri uri = new Uri(url, UriKind.Absolute);
                    HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(uri);
                    reqest.BeginGetResponse(ImageHelper.DownloadImageCallback, reqest);
                }
            }
        }
        private static bool isNetworkAvailable()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                System.Windows.MessageBox.Show("No Network is Available. Try again later.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
