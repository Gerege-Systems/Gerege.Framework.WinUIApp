namespace WinUIAppExample;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SampleApp;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.App().EventHandler += GeregEventHandler;
        this.AppRaiseEvent("trigger-client-login");
    }

    /// <summary>
    /// Апп дээр идэвхижсэн үзэгдлүүдийг хүлээн авч ажиллуулах.
    /// </summary>
    /// <param name="event">Идэвхжсэн үзэгдэл.</param>
    /// <param name="param">Үзэгдэлд дамжуулагдсан өгөгдөл.</param>
    /// <returns>
    /// Үзэгдэл хүлээн авагчтай бол боловсруулсан үр дүнг dynamic төрлөөр буцаана, үгүй бол null утга буцна.
    /// </returns>
    public dynamic? GeregEventHandler(string @event, dynamic? param = null)
    {
        Debug.WriteLine("Gerege үзэгдэл дуудагдаж байна => " + @event);

        return @event switch
        {
            "trigger-client-login" => OnTriggerClientLogin(),
            "client-login" => OnClientLogin(),
            "load-home" => OnLoadHome(),
            "load-page" => OnLoadPage(param),

            _ => null,
        };
    }

    /// <summary>
    /// Апп дээр үндсэн хэрэглэгч нэвтрэхийг шаардах үед энэ функц ажиллана.
    /// </summary>
    public dynamic? OnTriggerClientLogin()
    {
        MainFrame.Navigate(typeof(ClientLogin));

        return null;
    }

    /// <summary>
    /// Апп дээр үндсэн хэрэглэгч амжилттай нэвтрэх үед энэ функц ажиллана.
    /// </summary>
    public dynamic? OnClientLogin()
    {
        return this.AppRaiseEvent("load-home");
    }

    /// <summary>
    /// Нүүр хуудасруу шилжихийг хүсэх үед энэ функц ажиллана.
    /// </summary>
    public dynamic? OnLoadHome()
    {
        MainGrid.Children.Clear();
        MainGrid.Children.Add(MainFrame);

        MainFrame.Navigate(typeof(HomePage));

        return null;
    }

    /// <summary>
    /// Модулиас уншсан Page рүү шилжих.
    /// </summary>
    /// <param name="param">Page обьект.</param>
    public dynamic? OnLoadPage(dynamic param)
    {
        try
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add((Page)param);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return null;
    }
}
