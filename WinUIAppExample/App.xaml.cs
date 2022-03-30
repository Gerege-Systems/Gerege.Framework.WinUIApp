using Microsoft.UI.Xaml;

namespace WinUIAppExample
{
    /// <author>
    /// codesaur - 2022.01.22
    /// </author>
    /// <project>
    /// Gerege Application Development Framework V5
    /// </project>

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
}
