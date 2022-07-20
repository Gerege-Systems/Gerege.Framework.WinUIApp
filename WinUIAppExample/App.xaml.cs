using Microsoft.UI.Xaml;

/////// date: 2022.01.22 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace WinUIAppExample;

public partial class App : SampleApp.SampleApp
{
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Апп анх ачаалагдах үед үндсэн цонхыг үүсгэн идэвхжүүлж байна.
    /// </summary>
    /// <param name="args">Ачаалагдсан процессын утгууд.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        Window window = new MainWindow();
        window.Activate();
    }
}
