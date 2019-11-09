using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Desu.me
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        MangaList desu_me = new MangaList();

        class MangaList
        {
            public response response { get; set; }
        }

        class response
        {
            public string id { get; set; }
            public string russian { get; set; }
            public pages pages { get; set; }
        }


        class pages
        {
            public ch_curr ch_curr { get; set; }
            public List<list> list { get; set; }
        }

        class ch_curr
        {
            public string vol { get; set; }
            public string ch { get; set; }
            public string title { get; set; }
        }

        class list
        {
            public string img { get; set; }
        }

        public class Pic
        {
            public string ImagePath { get; set; } // путь к изображению
        }

        public Page3()
        {
            this.InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string tmp = "";
            if (e.Parameter != null)
            {
                tmp = e.Parameter.ToString();
                string id1 = tmp.Substring(0, tmp.IndexOf("+++++"));
                string id2 = tmp.Substring(tmp.IndexOf("+++++") + 5);
                ParseTitle(id1, id2);
            }
        }

        private async void ParseTitle(string id1, string id2)
        {
            string url = "http://desu.me/manga/api/" + id1 + "/chapter/" + id2;
            desu_me = await GetTitleList(url);
            int n = desu_me.response.pages.list.Count;
            Title.Text = desu_me.response.russian + "  " + desu_me.response.pages.ch_curr.vol + '-' + desu_me.response.pages.ch_curr.ch + " " + desu_me.response.pages.ch_curr.title;
            for (int i = 0; i < n; ++i)
            {
                Pages.Items.Add(new Pic { ImagePath = desu_me.response.pages.list[i].img });
            }
        }
        private async Task<MangaList> GetTitleList(string url)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response = new System.Net.Http.HttpResponseMessage();
            response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MangaList>(json);
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Page2), desu_me.response.id);
        }

        private void Pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = Pages.SelectedIndex + 1;
            int count = desu_me.response.pages.list.Count;
            Page.Text = num + "/" + count;
        }
    }
}
