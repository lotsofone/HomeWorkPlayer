using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.Media.Core;
using System.Linq;
using System.IO;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace HomeworkPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem mfi = (MenuFlyoutItem)sender;
            switch (mfi.Tag)
            {
                case "选择本地文件":
                    FileOpenPicker picker = new FileOpenPicker();
                    picker.ViewMode = PickerViewMode.Thumbnail;

                    picker.FileTypeFilter.Add(".mp4");
                    picker.FileTypeFilter.Add(".mp3");
                    StorageFile file = await picker.PickSingleFileAsync();

                    if (file != null)
                    {
                        var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                        mainplayer.SetSource(stream, file.ContentType);
                        musicpic.Opacity = 100;
                    }
                    else
                    {
                        musicpic.Opacity = 0;
                        return;
                    }
                    break;
                case "选择在线文件":
                    selectfile.ShowAt(mainplayer);
                    break;
            }
        }

        private void mainplayer_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            mainmenu.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
            //Windows.UI.Xaml.Controls.Primitives.FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void selectfilebutton_ClickAsync(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(selectfiletext.Text);
            if (((Button)sender).Name== "selectfileplay")
            {
                mainplayer.Source = uri;
                musicpic.Opacity = 100;
            }
            else//下载文件
            {
                var myMusics = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
                var myMusic = myMusics.SaveFolder;
                //var myMusic = KnownFolders.MusicLibrary;
                var fileName = Path.GetFileName(uri.LocalPath);
                try {
                    StorageFile musicFile = await myMusic.CreateFileAsync(fileName , Windows.Storage.CreationCollisionOption.FailIfExists);
                    if (musicFile != null)
                    {
                        Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                        
                        IBuffer buffer;
                        try
                        {
                            buffer = await httpClient.GetBufferAsync(uri);
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    
                        await FileIO.WriteBufferAsync(musicFile, buffer);
                    }
                }
                catch(Exception ex)
                {

                }
            }
            selectfile.Hide();
        }
    }
}
