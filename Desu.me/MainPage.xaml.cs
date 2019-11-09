using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Desu.me
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string starturl = "http://desu.me/manga/api/?";
        string limit = "limit=";
        int limitvalue = 50;
        string order = "&order=";
        string ordervalue = "popular";
        string page = "&page=";
        int pagevalue = 1;
        string search = "&search=";

        //public StorageFolder localFolder = ApplicationData.Current.LocalFolder; // локальная папка
        public StorageFolder localFolder = ApplicationData.Current.LocalFolder; // локальная папка
        public StorageFile data;


        MangaList desu_me = new MangaList();
        MangaList search123 = new MangaList();
        // Избранное
        class FavoritePage
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
        // Прочитанное
        class Read
        {
            public string count { get; set; }
            public string name { get; set; }
            public string russian { get; set; }
            public string id { get; set; }
            public string image { get; set; }
            public List<chapters> Chapters { get; set; }
        }
        class chapters
        {
            public string id { get; set; }
            public string vol { get; set; }
            public string ch { get; set; }
            public string title { get; set; }
        }

        class MangaList
        {
            public List <response> Response{ get; set; }
        }

        class response
        {
            public string name { get; set; }
            public string id { get; set; }
            public image image { get; set; }
            public string description { get; set; }
        }

        class image
        {
            public string original { get; set; }
        }

        public class Manga
        {
            public BitmapImage image { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
        
        public MainPage()
        {
            this.InitializeComponent();
            SaveFile("data.json");
            string url = starturl + limit + limitvalue + order + ordervalue + page + pagevalue;
            Parse(url);
        }

        private async void SaveFile(string filename)
        {
            data = await localFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
        }

        private async void Parse(string url)
        {
            desu_me = await GetMangaList(url);
            int n = desu_me.Response.Count;
            for (int i = 0; i < n; ++i)
            {
                var uri = new Uri(desu_me.Response[i].image.original);
                var bitmap = new BitmapImage(uri);
                DesuMe.Items.Add(new Manga { image = bitmap, name = desu_me.Response[i].name, description = desu_me.Response[i].description });
            }
        }

        private async void ParseSearch(string url)
        {
            search123 = await GetMangaList(url);
            int n = search123.Response.Count;
            for (int i = 0; i < n; ++i)
            {
                var uri = new Uri(search123.Response[i].image.original);
                var bitmap = new BitmapImage(uri);
                Search.Items.Add(new Manga { name = "\t" + search123.Response[i].name, image = bitmap });
            }
        }

        private async Task<MangaList> GetMangaList(string url)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response = new System.Net.Http.HttpResponseMessage();
            response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MangaList>(json);
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            DesuMe.Items.Clear();
            --pagevalue;
            if (pagevalue == 1)
            {
                Back_btn.IsEnabled = false;
            }
            
            string url = starturl + limit + limitvalue + order + ordervalue + page + pagevalue;
            Parse(url);
        }

        private void Next_btn_Click(object sender, RoutedEventArgs e)
        {
            Back_btn.IsEnabled = true;
            DesuMe.Items.Clear();
            ++pagevalue;
            string url = starturl + limit + limitvalue + order + ordervalue + page + pagevalue;
            Parse(url);
        }

        private void DesuMe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = DesuMe.SelectedIndex;
                Frame.Navigate(typeof(Page2), desu_me.Response[i].id);
            }
            catch
            {
                
            }
        }
        
        private void Search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = Search.SelectedIndex;
                Frame.Navigate(typeof(Page2), search123.Response[i].id);
            }
            catch
            {

            }
        }

        private void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search.Items.Clear();
            if (Search_tb.Text != "")
            {
                string searchvalue = Search_tb.Text;
                string url = starturl + limit + limitvalue + order + ordervalue + page + pagevalue + search + searchvalue;
                ParseSearch(url);
            }
            
        }

        private void Search_tb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Search_btn_Click(sender, e);
            }
        }

        private void Favorite_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Fav_Page));
        }
    }
}
