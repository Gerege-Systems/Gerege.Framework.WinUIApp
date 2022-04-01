using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Newtonsoft.Json;

using SampleApp;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIAppExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public struct Welcome
        {
            public static int GeregeMessage() => 101;

            [JsonProperty("title", Required = Required.Always)]
            public string Title { get; set; }
        }

        public HomePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var MyApp = this.App();
                string dllName = "SampleModule.dll";
                object? partners = MyApp.ModuleStart(
                    MyApp.CurrentDirectory + dllName,
                    new { conclusion = "Loading module is easy and peasy" });

                if (partners is not Page)
                    throw new Exception(dllName + ": Module.Start функц нь Page буцаасангүй!");

                MyApp.RaiseEvent("load-page", partners);
            }
            catch (Exception ex)
            {
                ((Button)sender).Content = ex.Message;
                Debug.WriteLine("SampleModule.dll-ийг ачаалах үед алдаа гарлаа -> " + ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Welcome t = this.UserCacheRequest<Welcome>();
                TitleBox.Text = t.Title;
            }
            catch (Exception ex)
            {
                TitleBox.Text = ex.Message;
                Debug.WriteLine("Welcome-ийг авах үед алдаа гарлаа -> " + ex.Message);
            }
            finally
            {
                LoadModuleBtn.Visibility = Visibility.Visible;
            }
        }
    }
}
