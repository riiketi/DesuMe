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
    public sealed partial class Page2 : Page
    {

        StorageFolder localFolder = Windows.Storage.KnownFolders.PicturesLibrary; // локальная папка
        //public StorageFolder localFolder = ApplicationData.Current.LocalFolder; // локальная папка
        StorageFile favor;
        StorageFile readFile;
        
        ////////////////////////////////////////////////////
        Favorite fav = new Favorite();
        Read readClass = new Read();
        manga man = new manga();

        // Избранное
        class Favorite
        {
            public int count { get; set; }
            public List<manga> Manga { get; set; }
        }
        class manga
        {
            public string name { get; set; }
            public string russian { get; set; }
            public string id { get; set; }
            public string image { get; set; }
        }





        class Read
        {
            public int count { get; set; }
            public List<ReadManga> ReadManga { get; set; }
        }
        class ReadManga
        {
            public string name { get; set; }
            public string russian { get; set; }
            public string id { get; set; }
            public int count { get; set; }
            public List<list> readchapters { get; set; }
        }
        ////////////////////////////////////////////////////




        MangaList desu_me = new MangaList();
        int[] nums;
        int[] inv_nums;

        class MangaList
        {
            public response response { get; set; }
        }

        class response
        {
            public string id { get; set; }
            public string name { get; set; }
            public string russian { get; set; }
            public image image { get; set; }
            public string description { get; set; }
            public chapters chapters { get; set; }
        }

        class image
        {
            public string original { get; set; }
        }

        class chapters
        {
            public int count { get; set; }
            public List<list> list { get; set; }
        }

        class list
        {
            public string id { get; set; }
            public string vol { get; set; }
            public string ch { get; set; }
            public string title { get; set; }
        }

        class FirstLast
        {
            public string vol { get; set; }
            public string ch { get; set; }
            public string name { get; set; }
        }

        public class ChaptersList
        {
            public string vol { get; set; }
            public string ch { get; set; }
            public string title { get; set; }
        }

        public class wtf
        {
            public string Title { get; set; }
            public string isRead { get; set; }
        }

        string IsRead(bool isread)
        {
            if (isread)
            {
                return "Прочитано";
            }
            return "Не прочитано";
        }

        public Page2()
        {
            this.InitializeComponent();
            FirstCreateFile();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string id = "";
            if (e.Parameter != null)
            {
                id = e.Parameter.ToString();
                ParseTitle(id);
            }
            
        }

        private async void FirstCreateFile()
        {
            try
            {
                StorageFile file = await localFolder.GetFileAsync("fav.json");
            }
            catch
            {
                favor = await localFolder.CreateFileAsync("fav.json", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(favor, "{\"count\": 0,\"Manga\": [{\"name\": \"\",\"russian\": \"\",\"id\": \"-1\",\"image\": \"\",\"count\": 0,\"Chapters\": [{\"id\": \"-1\",\"vol\": \"\",\"ch\": \"\",\"title\": \"\"}]}]}");   //"{\"count\": 0,\"manga\": [{\"name\": \"\",\"russian\": \"\",\"id\": -1,\"image\": \"\"}]}");
            }
            
        }

        private async void LoadFavorite()
        {
            favor = await localFolder.GetFileAsync("fav.json");
            string fav_txt = await FileIO.ReadTextAsync(favor);
            if (JsonConvert.DeserializeObject<Favorite>(fav_txt) != null)
            {
                fav = JsonConvert.DeserializeObject<Favorite>(fav_txt);
                for (int i = 0; i < fav.count; ++i)
                {
                    if (fav.Manga[i+1].id == desu_me.response.id)
                    {
                        favorite.IsChecked = true;
                        break;
                    }
                }
            }
        }

        private async void ParseTitle(string id)
        {
            string url = "http://desu.me/manga/api/" + id;
            desu_me = await GetTitleList(url);
            int n = desu_me.response.chapters.count;
            nums = new int[n];
            inv_nums = new int[n];
            Title.Text = desu_me.response.russian;
            //RotateMatrixs(n);
            //Cover.Source = new BitmapImage(new Uri(desu_me.response.image.original));
            for (int i = 0; i < n; ++i)
            {
                inv_nums[i] = i;
                //Chapters.Items.Add("sd");
                Chapters.Items.Add(new wtf() { Title = "  " + desu_me.response.chapters.list[i].vol + '-' + desu_me.response.chapters.list[i].ch + " " + desu_me.response.chapters.list[i].title , isRead = await IsReaded(desu_me.response.id, desu_me.response.chapters.list[i]) });
                
                //DesuMe.Items.Add(new Manga { image = bitmap, name = desu_me.Response[i].name, description = desu_me.Response[i].description });
            }
            Reverse(n);
            LoadFavorite();
        }
        private async Task<MangaList> GetTitleList(string url)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response = new System.Net.Http.HttpResponseMessage();
            response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MangaList>(json);
        }

        private void Chapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sort.IsChecked == true))
            {
                //Readed(desu_me.response.id, desu_me.response.chapters.list[Chapters.Items.Count - Chapters.SelectedIndex - 1]);
                Frame.Navigate(typeof(Page3), desu_me.response.id + "+++++" + desu_me.response.chapters.list[Chapters.Items.Count - Chapters.SelectedIndex - 1].id);
            }
            else
            {
                //Readed(desu_me.response.id, desu_me.response.chapters.list[Chapters.SelectedIndex]);
                Frame.Navigate(typeof(Page3), desu_me.response.id + "+++++" + desu_me.response.chapters.list[Chapters.SelectedIndex].id);
            }
        }

        async Task <string> IsReaded(string MangaID, list ChapterID)
        {
            readFile= await localFolder.GetFileAsync("read.json");
            string read_txt = await FileIO.ReadTextAsync(readFile);
            if (JsonConvert.DeserializeObject<Favorite>(read_txt) != null)
            {
                readClass = JsonConvert.DeserializeObject<Read>(read_txt);
                for (int i = 0; i < readClass.count; ++i)
                {
                    if (readClass.ReadManga[i + 1].id == desu_me.response.id)
                    {
                        if (readClass.ReadManga[i + 1].readchapters.Contains(ChapterID))
                        {
                            return IsRead(true);    // прочитано
                        }
                    }
                }
            }
            return IsRead(false);   // не прочитано
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Reverse(int N)
        {
            for (int i = 0; i < N ; ++i)
            {
                int a = N - i - 1;
                nums[i] = inv_nums[a];
            }
        }

        private void sort_Click(object sender, RoutedEventArgs e)
        {
            if ((sort.IsChecked == true))
            {
                Chapters.Items.Clear();
                int n = desu_me.response.chapters.count;
                for (int i = 0; i < n; ++i)
                {
                    Chapters.Items.Add(new wtf() { Title = "  " + desu_me.response.chapters.list[nums[i]].vol + '-' + desu_me.response.chapters.list[nums[i]].ch + " " + desu_me.response.chapters.list[nums[i]].title, isRead = IsRead(false) });
                    //Chapters.Items.Add("  " + desu_me.response.chapters.list[nums[i]].vol + '-' + desu_me.response.chapters.list[nums[i]].ch + " " + desu_me.response.chapters.list[nums[i]].title);
                }
            }
            else
            {
                Chapters.Items.Clear();
                int n = desu_me.response.chapters.count;
                for (int i = 0; i < n; ++i)
                {
                    Chapters.Items.Add(new wtf() { Title = "  " + desu_me.response.chapters.list[inv_nums[i]].vol + '-' + desu_me.response.chapters.list[inv_nums[i]].ch + " " + desu_me.response.chapters.list[inv_nums[i]].title, isRead = IsRead(false) });
                    //Chapters.Items.Add("  " + desu_me.response.chapters.list[inv_nums[i]].vol + '-' + desu_me.response.chapters.list[inv_nums[i]].ch + " " + desu_me.response.chapters.list[inv_nums[i]].title);
                }
            }
        }

        private void favorite_Click(object sender, RoutedEventArgs e)
        {
            if (favorite.IsChecked == true)
            {
                AddFav("fav.json");
            }
            else
            {
                DelFav("fav.json");
            }
        }


        private async void AddFav(string filename)
        {
            bool flag = false;
            for (int i = 0; i < fav.count; ++i)
            {
                if (fav.Manga[i + 1].id == desu_me.response.id)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                favor = await localFolder.GetFileAsync(filename);
                string fav_txt = await FileIO.ReadTextAsync(favor);
                /*
                if (JsonConvert.DeserializeObject<Favorite>(fav_txt) == null)
                {
                    await FileIO.WriteTextAsync(favor, "{\"count\": 0,\"manga\": [{\"name\": \"\",\"russian\": \"\",\"id\": -1,\"image\": \"\"}]}");
                    //fav = JsonConvert.DeserializeObject<Favorite>("{\"Favorite\": {\"count\": 0,\"manga\": [{\"name\": \"\",\"russian\": \"\",\"id\": -1,\"image\": \"\"}]}}");
                    fav_txt = await FileIO.ReadTextAsync(favor);
                }
                */
                fav = JsonConvert.DeserializeObject<Favorite>(fav_txt);

                // Добавляем текущий тайтл в избранное
                ++fav.count;
                fav.Manga.Add(new manga()
                {
                    name = desu_me.response.name,
                    russian = desu_me.response.russian,
                    id = desu_me.response.id,
                    image = desu_me.response.image.original
                });
                string fav_s = "";
                fav_s = JsonConvert.SerializeObject(fav);
                await FileIO.WriteTextAsync(favor, fav_s);
            }
        }

        private async void DelFav(string filename)
        {
            StorageFile data = await localFolder.GetFileAsync(filename);
            string fav_txt = await FileIO.ReadTextAsync(data);
            fav = JsonConvert.DeserializeObject<Favorite>(fav_txt);

            for (int i = 0; i<fav.count; ++i)
            {
                if (fav.Manga[i+1].id == desu_me.response.id)
                {
                    fav.Manga.Remove(fav.Manga[i+1]);
                    break;
                }
            }

            // Добавляем текущий тайтл в избранное
            --fav.count;
            string s = JsonConvert.SerializeObject(fav);
            await FileIO.WriteTextAsync(favor, s);
        }

        private void read_chb_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
