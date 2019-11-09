using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Desu.me
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Fav_Page : Page
    {
        public Fav_Page()
        {
            this.InitializeComponent();
        }

        StorageFolder localFolder = Windows.Storage.KnownFolders.PicturesLibrary; // локальная папка
        //public StorageFolder localFolder = ApplicationData.Current.LocalFolder; // локальная папка
        StorageFile favor;

        Favorit desu_me = new Favorit();

        class Favorit
        {
            public string count { get; set; }
            public List<manga> Manga { get; set; }
        }
        class manga
        {
            public string name { get; set; }
            public string russian { get; set; }
            public string id { get; set; }
            public string image { get; set; }
        }

        public class Manga
        {
            public BitmapImage image { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadFavorite();
        }

        private async void Parse(string url)
        {
            desu_me = await GetMangaList(url);
            int n = Convert.ToInt32(desu_me.count);
            for (int i = 0; i < n; ++i)
            {
                var uri = new Uri(desu_me.Manga[i].image);
                var bitmap = new BitmapImage(uri);
                DesuMe.Items.Add(new Manga { image = bitmap, name = desu_me.Manga[i].name });
            }
        }

        private async Task<Favorit> GetMangaList(string url)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response = new System.Net.Http.HttpResponseMessage();
            response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Favorit>(json);
        }

        private async void LoadFavorite()
        {
            favor = await localFolder.CreateFileAsync("fav.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
            string fav_txt = await FileIO.ReadTextAsync(favor);
            if (JsonConvert.DeserializeObject<Favorit>(fav_txt) != null)
            {
                desu_me = JsonConvert.DeserializeObject<Favorit>(fav_txt);
                int n = Convert.ToInt32(desu_me.count);
                for (int i = 1; i <= n; ++i)
                {
                    var uri = new Uri(desu_me.Manga[i].image);
                    var bitmap = new BitmapImage(uri);
                    DesuMe.Items.Add(new Manga { image = bitmap, name = desu_me.Manga[i].name });
                }
            }
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void DesuMe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = DesuMe.SelectedIndex + 1;
                Frame.Navigate(typeof(Page2), desu_me.Manga[i].id);
            }
            catch
            {

            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {

        }
        
    }
}
