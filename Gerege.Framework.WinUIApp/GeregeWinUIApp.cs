using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace Gerege.Framework.WinUIApp
{
    /// <author>
    /// codesaur - 2022.01.22
    /// </author>
    /// <project>
    /// Gerege Application Development Framework V5
    /// </project>

    /// <summary>
    /// Гэрэгэ логикоор ажиллах програм хангамжын үндсэн суурь апп хийсвэр класс.
    /// </summary>
    public abstract class GeregeWinUIApp : Application
    {
        /// <summary>Апп процесс идэвхтэй ажиллаж буй хавтас зам.</summary>
        public string CurrentDirectory;

        /// <summary>Гэрэгэ үзэгдэл хүлээн авагч төрөл зарлах.</summary>
        /// <param name="name">Гэрэгэ үзэгдэл нэр.</param>
        /// <param name="args">Үзэгдэлд дамжуулах өгөгдөл параметр.</param>
        /// <returns>Үзэгдэл хүлээн авагчаас үр дүн эсвэл null буцаана.</returns>
        public delegate dynamic? GeregeEventHandler(string name, dynamic? args = null);

        /// <summary>Gerege үзэгдэл хүлээн авагч.</summary>
        public event GeregeEventHandler? EventHandler;

        /// <summary>Гэрэгэ апп үүсгэгч.</summary>
        public GeregeWinUIApp()
        {
            // App идэвхитэй ажиллаж байгаа хавтас олоx
            DirectoryInfo? currentDir = null;
            if (Environment.ProcessPath != null)
                currentDir = Directory.GetParent(Environment.ProcessPath);
            if (currentDir == null)
                currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            CurrentDirectory = currentDir.FullName + Path.DirectorySeparatorChar;
        }

        /// <summary>Гэрэгэ үзэгдлийг идэвхжүүлэх.</summary>
        /// <param name="name">Идэвхжүүлэх үзэгдэл нэр.</param>
        /// <param name="args">Үзэгдэлд дамжуулагдах өгөгдөл.</param>
        /// <returns>
        /// Үзэгдэл хүлээн авагчаас үр дүн ирвэл dynamic төрлөөр буцаана.
        /// Ямар нэгэн алдаа гарч Exception үүссэн бол үзэгдлийн үр дүнд алдааны мэдээллийг олгоно.
        /// <para>Үзэгдэл хүлээн авагчаас үр дүн null байх боломжтой.</para>
        /// </returns>
        public virtual dynamic? RaiseEvent(string name, dynamic? args = null)
        {
            // боломжит үзэгдэл хүлээн авагчдийг цуглуулж байна
            Delegate[]? delegates = EventHandler?.GetInvocationList();

            // боломжит үзэгдэл хүлээн авагчид байхгүй тул null утга буцаая
            if (delegates == null) return null;

            // хүлээн авагчидаас боловсруулсан үр дүнг энэ жагсаалтад бүртгэе
            List<dynamic> results = new();

            // үзэгдэл хүлээн авагч бүрийг ажиллуулж байна
            foreach (Delegate d in delegates)
            {
                try
                {
                    results.Add(d.DynamicInvoke(name, args));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            // амжилттай гүйцэтгэсэн эхний үр дүн эсвэл null буцаая
            return results.FirstOrDefault(r => r != null);
        }

        /// <summary>
        /// Гэрэгэ клиент апп нь Гэрэгэ хэрэглэгч эрхийн дагуу нэвтэрч ажиллах ерөнхий зохион байгуулалттай.
        /// <para>
        /// Үүнд GeregeWinUIApp нь анх амжилттай ачаалагдах үедээ энэ хийсвэр функцыг дуудан
        /// HTTP клиент обьект болон лог бичих обьект гэх мэт шаардлагатай бүрэлдэхүүн хэсгээ үүсгэдэг байх ёстой.
        /// </para>
        /// <para>
        /// Хөгжүүлэгч нь заавал энэ функцийг удамшуулсан класс дээрээ override түлхүүр үгээр
        /// дахин функц болгон тодорхойлж тухайн aппд шаардлагатай бүрэлдэхүүн обьектуудыг үүсгэнэ.
        /// </para>
        /// </summary>
        protected abstract void CreateComponents();

        /// <summary>
        /// Гэрэгэ үйлчилгээний DLL ачаалж Module классын Start функцыг дуудсанаар FrameworkElement авах.
        /// </summary>
        /// <param name="filePath">Module DLL файлын зам/нэр.</param>
        /// <param name="param">Module.Start функцэд дамжуулах параметр.</param>
        /// <returns>
        /// Амжилттай уншигдаж үүссэн FrameworkElement.
        /// </returns>
        public FrameworkElement LoadModule(string filePath, dynamic param)
        {
            if (string.IsNullOrEmpty(filePath)
                    || !File.Exists(filePath))
                throw new Exception(filePath + ": Модул зам олдсонгүй!");

            string dllName = Path.GetFileName(filePath);

            Assembly assembly = Assembly.LoadFrom(filePath);
            Type? type = assembly.GetType("Module");
            if (type == null) throw new Exception(dllName + ": Module class олдсонгүй!");

            object? instanceOfMyType = Activator.CreateInstance(type);
            if (instanceOfMyType == null) throw new Exception(dllName + ": Module обьект үүсгэж чадсангүй!");

            MethodInfo? methodInfo = type.GetMethod("Start", new Type[] { typeof(object) });
            if (methodInfo == null) throw new Exception(dllName + ": Module.Start функц олдоогүй эсвэл буруу тодорхойлсон байна!");

            try
            {
                object[] parameters = new object[1] { param };
                object? result = methodInfo.Invoke(instanceOfMyType, parameters);
                return result is FrameworkElement element ? element : throw new Exception(dllName + ": Module.Start функц нь FrameworkElement буцаасангүй!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// Апп анх ачаалагдах үед биелэх үзэгдлийн виртуал функц.
        /// Анхны код нь аппыг цор ганц хувилбараар ажиллуулах зорилготой.
        /// <para>
        /// Хөгжүүлэгч нь энэ функцийг удамшуулсан класс дээрээ override түлхүүр үгээр
        /// дахин функц болгон тодорхойлж шаардлагатай үйлдэл логик оруулж/нэмж гүйцэтгэж болно.
        /// </para>
        /// </summary>
        /// <param name="args">Ачаалагдсан процессын утгууд.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // системд өмнө нь ачаалагдсан Гэрэгэ апп-ыг хайж байна
            AppActivationArguments appArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            AppInstance mainInstance = AppInstance.FindOrRegisterForKey("GeregeWinUIApplication");

            // системд ачаалагдсан Гэрэгэ апп олдохнуу?
            if (!mainInstance.IsCurrent)
            {
                // ачаалагдсан байгаа Гэрэгэ апп-ыг идэвхжүүллээ
                await mainInstance.RedirectActivationToAsync(appArgs);

                // шинээр ачаалж байгаа энэ Гэрэгэ апп-ыг устгалаа
                Process.GetCurrentProcess().Kill();

                return;
            }

            // системд өмнө нь ачаалагдсан Гэрэгэ апп олдоогүй тул
            // шинээр ачаалж байгаа энэ Гэрэгэ апп-ыг хэвийн үргэлжлүүлж ажиллуулъяа
            AppInstance.GetCurrent().Activated += App_Activated;

            // GeregeWinUIApp шинээр ачаалж байна гэж үзээд
            // шаардлагатай бүрэлдэхүүн хэсэг үүсгэх хийсвэр функцыг ажиллууллая
            CreateComponents();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private const int SW_SHOWNOACTIVATE = 4;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        // Апп идэвхжүүлэх хүсэлт ирвэл аппын үндсэн цонхыг олж сэргээгээд хамгийн дээр харуулна
        private void App_Activated(object? sender, AppActivationArguments e)
        {
            try
            {
                // Апп үндсэн процессыг авна
                var prc = Process.GetCurrentProcess();

                // Процесс нь үндсэн цонхгүй бол хийх зүйлгүй
                if (prc.MainWindowHandle == IntPtr.Zero) return;

                // Үндсэн цонх нь харагдахгүй далд байвал сэргээе
                if (IsIconic(prc.MainWindowHandle))
                    _ = ShowWindow(prc.MainWindowHandle, SW_SHOWNOACTIVATE);

                // Үндсэн цонхыг хамгийн дээр идэвхжүүлж харуулъяа
                SetForegroundWindow(prc.MainWindowHandle);
            }
            catch (Exception)
            {
            }
        }
    }
}
