using System;
using System.Diagnostics;
using SampleModule.PartnerPage;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

/// <summary>
/// Модуль үйлчилгээний эх класс.
/// <para>
/// </para>
/// Gerege application нь Модуль үйлчилгээг ажиллуулах зарчим нь бол
/// Module dll-ийг санах ойд ачаалаад Assembly дотроос ямар нэг namespace дотор агуулагдаагүй
/// ил байгаа Module классын Start функцыг дуудсанаар үр дүнд үүсэх обьектыг аваад ашигладаг.
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
    public object? Start(object param)
    {
        string? log = Convert.ToString(param);
        Debug.WriteLine("Param -> " + log);

        return new Partners();
    }
}
