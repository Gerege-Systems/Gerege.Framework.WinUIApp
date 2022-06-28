﻿namespace SampleApp;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Gerege.Framework.WinUIApp;
using Gerege.Framework.HttpClient;

/// <summary>
/// GeregeWinUIApp-аас удамшсан SampleApp апп объект.
/// </summary>
public class SampleApp : GeregeWinUIApp
{
    /// <summary>
    /// Хэрэглэгчийн HTTP клиент обьект.
    /// </summary>
    public SampleUserClient UserClient;

    /// <inheritdoc />
    protected override void CreateComponents()
    {
        // Gerege үзэгдэл хүлээн авагч тохируулъя
        EventHandler += BaseEventHandler;

        // Хэрэглэгчийн HTTP клиентэд зориулж удирдлагууд үүсгэж байна
        var pipeline = new LoggingHandler(new ConsoleLogger())
        {
            InnerHandler = new RetryHandler()
            {
                // Бодит серверлүү хандах бол энэ удирдлага ашиглаарай
                //InnerHandler = new System.Net.Http.HttpClientHandler()

                // Туршилтын зорилгоор хуурамч сервер хандалтын удирдлага ашиглаж байна
                InnerHandler = new MockServerHandler()
            }
        };

        // Хэрэглэгчийн клиентийг үүсгэж байна
        UserClient = new(pipeline);
    }

    /// <inheritdoc />
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // аппыг цор ганц хувилбараар ажиллуулах гол зорилготой эх ангийн үзэгдлийг дуудаж байна
        base.OnLaunched(args);

        // Апп процесс амжилттай ачаалсан тул өмнө нь Гэрэгэ HTTP клиентийн
        // хүсэлт боловсруулах үед үүссэн байж болох Cache хавтас байвал цэвэрлэе
        ClearCacheFolder();
    }

    /// <summary>
    /// Gerege үзэгдэл хүлээн авагч.
    /// </summary>
    public dynamic? BaseEventHandler(string @event, dynamic? param = null)
    {
        return @event switch
        {
            "get-server-address" => "http://mock-server/api",

            _ => null,
        };
    }

    /// <summary>
    /// Гэрэгэ үйлчилгээний DLL ачаалж Module классын Start функцыг дуудсанаар үр дүнг авах.
    /// </summary>
    /// <param name="filePath">Module DLL файлын зам/нэр.</param>
    /// <param name="param">Module.Start функцэд дамжуулах параметр.</param>
    /// <returns>
    /// Амжилттай үр дүн.
    /// </returns>
    public object? ModuleStart(string filePath, dynamic param)
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
            return methodInfo.Invoke(instanceOfMyType, parameters);                
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            throw ex.InnerException ?? ex;
        }
    }

    /// <summary>
    /// Гэрэгэ HTTP клиент хүсэлт боловсруулах үед үүссэн Cache хавтас байвал цэвэрлэх.
    /// <para>
    /// Гэрэгэ HTTP клиент нь хүсэлт боловсруулахдаа Cache үүсгэх боломжтой.
    /// Тийм учраас Апп шинээр ачаалах үед өмнөх процессийн үүсгэсэн Cache хавтас байгаа бол цэвэрлэх хэрэгтэй.
    /// </para>
    /// </summary>
    private void ClearCacheFolder()
    {
        try
        {
            GeregeCache tempCache = new(0, new { tmp = 0 });
            if (string.IsNullOrEmpty(tempCache.FilePath)) return;

            FileInfo cacheFI = new(tempCache.FilePath);
            if (cacheFI.Directory == null
                || !cacheFI.Directory.Exists
                || cacheFI.DirectoryName == null) return;

            DirectoryInfo dir = new(cacheFI.DirectoryName);
            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                subdir.Delete(true);
            }
        }
        catch { }
    }
}
