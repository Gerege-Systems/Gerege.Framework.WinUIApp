using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SampleApp;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace WinUIAppExample;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ClientLogin : Page
{
    public ClientLogin()
    {
        InitializeComponent();
    }

    private void LoginBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LoginBtn.IsEnabled = false;
            Username.IsEnabled = false;
            Password.IsEnabled = false;

            string username = Username.Text;
            string password = Password.Password;
            this.App().UserClient.Login(new { username, password});
        }
        catch (Exception ex)
        {
            errorTextBlock.Text = ex.Message;
        }
        finally
        {
            LoginBtn.IsEnabled = true;
            Username.IsEnabled = true;
            Password.IsEnabled = true;
        }
    }
}
