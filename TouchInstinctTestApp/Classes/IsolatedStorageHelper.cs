using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TouchInstinctTestApp.Classes
{
    static class IsolatedStorageHelper
    {
        public static bool isFileAlreadySaved(string url)
        {
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            return myIsolatedStorage.FileExists(Path.GetFileName(url));
        }
        public static Image getImageFromLocalStorage(string url)
        {
            string fileName = Path.GetFileName(url);

            Image img = new Image();
            BitmapImage bi = new BitmapImage();

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isFileAlreadySaved(url))
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        bi.UriSource = new Uri(fileStream.Name, UriKind.Absolute);
                    }
                    img.Source = bi;
                    return img;
                }
                else
                    return null;
            }
        }
        public static void saveImage(BitmapImage bmp, string fileName)
        {
            using (IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageFileStream FS = ISF.CreateFile(fileName);
                WriteableBitmap Wr_B = new WriteableBitmap(bmp);
                Extensions.SaveJpeg(Wr_B, FS, Wr_B.PixelWidth, Wr_B.PixelHeight, 0, 85); // Furthe we have to encode WriteableBitmap object to a JPEG stream.
                Wr_B.SaveJpeg(FS, Wr_B.PixelWidth, Wr_B.PixelHeight, 0, 85);
                FS.Close();
            }
        }
        public static void deleteAllSavedData()
        {
            try
            {
                IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                string[] files = myIsolatedStorage.GetFileNames();
                foreach (string fileName in files)
                {
                    myIsolatedStorage.DeleteFile(fileName);
                }
                MessageBox.Show("Кэш очищен");
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException e)
            {
                MessageBox.Show("Дождитесь окончания скачивания картинок с сервера и повторите операцию");
            }
        }
    }
}
