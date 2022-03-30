using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using Newtonsoft.Json;

using SampleApp;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SampleModule.PartnerPage
{
    public class Partner
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("logo", Required = Required.Always)]
        public string Logo { get; set; }

        [JsonProperty("href", Required = Required.Always)]
        public string WebAddress { get; set; }
    }

    public class PartnerList
    {
        public virtual int GeregeMessage() => 102;

        [JsonProperty("partners", Required = Required.Always)]
        public List<Partner> Data { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Partners : Page
    {
        public Partners()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(GetPartners).Start();
        }
        
        private void GetPartners()
        {
            string title = "";
            PartnerList partnerList = new();
            try
            {
                partnerList = this.UserRequest<PartnerList>();
                title = "Successfully retrieved partners list.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on fetching data -> " + ex);
                title = ex.Message;
            }
            finally
            {
                TitleBox.Text = title;

                PartnersPanel.Children.Clear();

                if (partnerList?.Data != null)
                {

                    foreach (Partner partner in partnerList.Data)
                    {
                        Border border = new Border
                        {
                            Tag = partner.WebAddress,
                            Margin = new Thickness(20, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Top
                        };
                        border.PointerPressed += MenuItemClick;

                        try
                        {
                            BitmapImage? logoImg = (BitmapImage?)(object)new BitmapImage(new Uri(
                                this.App().CurrentDirectory + "PartnerPage" + Path.DirectorySeparatorChar + partner.Logo));
                            border.Background = new ImageBrush { ImageSource = logoImg };
                        }
                        catch { }


                        border.Width = 127;
                        border.Height = 127;

                        border.Child = new TextBlock
                        {
                            Height = 40,
                            FontSize = 14,
                            Text = partner.Name,
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 0, 0, -45),
                            FontFamily = new FontFamily("Montserrat"),
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        PartnersPanel.Children.Add(border);
                    }
                }
            }
        }

        private void MenuItemClick(object sender, PointerRoutedEventArgs e)
        {
            string? href = Convert.ToString(((FrameworkElement)sender).Tag);

            if (string.IsNullOrEmpty(href)) return;

            Process.Start(new ProcessStartInfo("cmd", $"/c start {href}"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.AppRaiseEvent("load-home");
        }
    }
}
