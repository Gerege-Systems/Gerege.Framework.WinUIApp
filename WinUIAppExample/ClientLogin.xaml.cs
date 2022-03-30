using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SampleApp;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIAppExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClientLogin : Page
    {
        public ClientLogin()
        {
            this.InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string user = Username.Text;
            string pass = Password.Password;

            LoginBtn.IsEnabled = false;

            new Thread(() => ProceedUserLogin(user, pass)).Start();
        }

        private void ProceedUserLogin(string username, string password)
        {
            string? status = null;
            try
            {
                this.App().UserClient.Login(new { username, password });
                status = "Login success!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                status = ex.Message;
            }
            finally
            {
                errorTextBlock.Text = status;
                LoginBtn.IsEnabled = true;
            }
        }

    }
}
