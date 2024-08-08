using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

/////// date: 2022.01.22 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace Gerege.Framework.WinUIApp;

/// <summary>
/// Гэрэгэ логикоор ажиллах програм хангамжын үндсэн суурь апп хийсвэр класс.
/// </summary>
public abstract partial class GeregeWinUIApp : Application
{
    /// <summary>Апп процесс идэвхтэй ажиллаж буй хавтас зам.</summary>
    public string CurrentDirectory;

    /// <summary>Гэрэгэ үзэгдэл хүлээн авагч төрөл зарлах.</summary>
    /// <param name="name">Гэрэгэ үзэгдэл нэр.</param>
    /// <param name="args">Үзэгдэлд дамжуулах өгөгдөл параметр.</param>
    /// <returns>Үзэгдэл хүлээн авагчаас үр дүн эсвэл null буцаана.</returns>
    public delegate object? GeregeEventHandler(string name, object? args = null);

    /// <summary>Gerege үзэгдэл хүлээн авагч.</summary>
    public event GeregeEventHandler? EventHandler;

    /// <summary>Гэрэгэ апп үүсгэгч.</summary>
    public GeregeWinUIApp()
    {
        // App идэвхитэй ажиллаж байгаа хавтас олоx
        DirectoryInfo? currentDir = null;
        if (Environment.ProcessPath != null)
            currentDir = Directory.GetParent(Environment.ProcessPath);
        currentDir ??= new DirectoryInfo(Environment.CurrentDirectory);
        CurrentDirectory = currentDir.FullName + Path.DirectorySeparatorChar;
    }

    /// <summary>Гэрэгэ үзэгдлийг идэвхжүүлэх.</summary>
    /// <param name="name">Идэвхжүүлэх үзэгдэл нэр.</param>
    /// <param name="args">Үзэгдэлд дамжуулагдах өгөгдөл.</param>
    /// <returns>
    /// Үзэгдэл хүлээн авагчаас үр дүн ирвэл object төрлөөр буцаана.
    /// Ямар нэгэн алдаа гарч Exception үүссэн бол үзэгдлийн үр дүнд алдааны мэдээллийг олгоно.
    /// <para>Үзэгдэл хүлээн авагчаас үр дүн null байх боломжтой.</para>
    /// </returns>
    public virtual object? RaiseEvent(string name, object? args = null)
    {
        // боломжит үзэгдэл хүлээн авагчдийг цуглуулж байна
        Delegate[]? delegates = EventHandler?.GetInvocationList();

        // боломжит үзэгдэл хүлээн авагчид байхгүй бол null утга буцаая
        if (delegates == null) return null;

        // хүлээн авагчидаас боловсруулсан үр дүнг энэ жагсаалтад бүртгэе
        List<object?> results = [];

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

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsIconic(IntPtr hWnd);
    
    [LibraryImport("user32.dll")]
    private static partial int ShowWindow(IntPtr hWnd, uint Msg);

    private const int SW_SHOWNOACTIVATE = 4;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr hWnd);

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
