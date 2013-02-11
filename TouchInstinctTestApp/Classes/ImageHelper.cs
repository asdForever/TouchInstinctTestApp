using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;

namespace TouchInstinctTestApp.Classes
{
    static class ImageHelper
    {
        public static void DownloadImageCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)result.AsyncState;
                HttpWebResponse responce = (HttpWebResponse)req.EndGetResponse(result);
                Stream stream = responce.GetResponseStream();

                string fileName = Path.GetFileName(responce.ResponseUri.AbsoluteUri.ToString());

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.SetSource(stream);
                    if (!IsolatedStorageHelper.isFileAlreadySaved(responce.ResponseUri.AbsoluteUri.ToString()))
                    {
                        IsolatedStorageHelper.saveImage(bmp, fileName);
                    }
                });
                addDownloadedItemsToFilmCollection(responce.ResponseUri.AbsoluteUri.ToString());
            }
            catch (System.Net.WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Message.ToString());
                });
            }
        }
        public static void addDownloadedItemsToFilmCollection(string url)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    if (HtmlParser.tempList.Any<FilmData>(x => x.imageUrl == url))
                    {
                        FilmData fd = HtmlParser.tempList.First<FilmData>(x => x.imageUrl == url);
                        ImageHelper.setImageUrlToCashedFile(fd, url);
                        MainPage.filmCollection.Add(fd);
                    }
                }
                catch (System.ArgumentNullException e) { }
            });
        }
        public static void setImageUrlToCashedFile(FilmData fd, string url)
        {
            Image image = ImageHelper.getImage(url);
            if (image != null && image.Source != null)
            {
                BitmapImage tn = new BitmapImage();
                tn = (BitmapImage)image.Source;
                if (tn.UriSource.IsAbsoluteUri)
                    fd.imageUrl = tn.UriSource.AbsoluteUri.ToString();
            }
        }

        private static Image getImage(string url)
        {
            if (IsolatedStorageHelper.isFileAlreadySaved(url))
            {
                return IsolatedStorageHelper.getImageFromLocalStorage(url);
            }
            return null;
        }
    }
}
