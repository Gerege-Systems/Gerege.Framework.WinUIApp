﻿/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using SampleModule.PartnerPage;

/// <summary>
/// Модуль үйлчилгээний эх класс.
/// <para>
/// </para>
/// GeregeWinUIApp нь Модуль үйлчилгээг ажиллуулах зарчим нь бол
/// Module dll-ийг санах ойд ачаалаад Assembly дотроос ямар нэг namespace дотор агуулагдаагүй
/// ил байгаа Module классын Start функцыг дуудсанаар FrameworkElement аваад ашигладаг.
/// Тийм учраас Module классыг namespace-гүйгээр ил зарлаж байна.
/// </summary>
public class Module
{
    /// <summary>
    /// Модулийн эхлэл функц.
    /// Хост апп дээрээс дуудагдана.
    /// </summary>
    /// <param name="param">Хост апп-аас илгээсэн параметр.</param>
    /// <returns>
    /// Гэрэгийн томоохон харилцагчдын мэдээлэл агуулах хуудас.
    /// </returns>
    public FrameworkElement Start(dynamic param)
    {
        string log = Convert.ToString(param);
        Debug.WriteLine("Module.Start passed argument from Host application -> " + log);

        return new Partners();
    }
}
