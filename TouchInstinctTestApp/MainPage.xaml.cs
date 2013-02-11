using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TouchInstinctTestApp.Classes;

namespace TouchInstinctTestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static int pageId = 1;
        public static ObservableCollection<FilmData> filmCollection;

        #region scrollable list box properties
        private readonly DependencyProperty ListVerticalOffsetProperty = DependencyProperty.Register("ListVerticalOffset", typeof(double), typeof(MainPage), new PropertyMetadata(new PropertyChangedCallback(OnListVerticalOffsetChanged)));
        private double ListVerticalOffset
        {
            get { return (double)this.GetValue(ListVerticalOffsetProperty); }
            set { this.SetValue(ListVerticalOffsetProperty, value); }
        }
        public ScrollViewer scrollViewer;
        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            listBoxFilms.Loaded += new RoutedEventHandler(listBoxFilms_Loaded);
            filmCollection = new ObservableCollection<FilmData>();
            listBoxFilms.ItemsSource = filmCollection;

            getItemsFromUrl();
        }

        #region scrollable list box methods
        private void listBoxFilms_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= listBoxFilms_Loaded;
            scrollViewer = FindChildOfType<ScrollViewer>(element);

            Binding binding = new Binding();
            binding.Source = scrollViewer;
            binding.Path = new PropertyPath("VerticalOffset");
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(ListVerticalOffsetProperty, binding);
        }
        private static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }
        private static void OnListVerticalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TouchInstinctTestApp.MainPage page = obj as TouchInstinctTestApp.MainPage;
            ScrollViewer viewer = page.scrollViewer;

            if (viewer != null)
            {
                if (Convert.ToDouble(e.NewValue) > viewer.ScrollableHeight * 0.90)
                {
                    pageId++;
                    getItemsFromUrl();
                }
            }
        }
        #endregion

        private static void getItemsFromUrl()
        {
            var bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                Networking.getHtmlFromUrl("http://stream.ru/films/index/page/" + MainPage.pageId + "/");
            };
            bw.RunWorkerAsync();
        }

        private void ButtonClearCash_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageHelper.deleteAllSavedData();
        }
    }
}