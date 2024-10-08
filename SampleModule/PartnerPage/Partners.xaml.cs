﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using SampleApp;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace SampleModule.PartnerPage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

public class Partner
{
    [JsonPropertyName("name")]
    [JsonRequired]
    public string Name { get; set; }

    [JsonPropertyName("logo")]
    [JsonRequired]
    public string Logo { get; set; }

    [JsonPropertyName("href")]
    [JsonRequired]
    public string WebAddress { get; set; }
}

public class PartnerList
{
    [JsonPropertyName("partners")]
    [JsonRequired]
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
        try
        {
            PartnersPanel.Children.Clear();

            PartnerList list = this.AppUserGet<PartnerList>("http://mock-server/get/partners");
            TitleBox.Text = "Successfully retrieved partners list.";

            foreach (Partner partner in list.Data)
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
        catch (Exception ex)
        {
            TitleBox.Text = ex.Message;
            Debug.WriteLine($"Error on fetching data -> {ex.Message}");
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
