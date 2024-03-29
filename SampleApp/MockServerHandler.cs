﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace SampleApp;

/// <summary>
/// Туршилтын зорилгоор ашиглах хуурамч сервер хандалт.
/// </summary>
public sealed class MockServerHandler : HttpMessageHandler
{
    private readonly string tokenValue = "GEREGE_SUPER_SECRET_TOKEN";

    /// <inheritdoc />
    protected sealed override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {            
        try
        {
            Thread.Sleep(500); // Интернетээр хандаж буй мэт сэтгэгдэл төрүүлэх үүднээс хором хүлээлгэе

            string? requestTarget = request.RequestUri?.ToString();
            if (requestTarget != this.AppRaiseEvent("get-server-address"))
                throw new Exception("Unknown route pattern [" + requestTarget + "]");

            string message_code_header = string.Join("", request.Headers.GetValues("message_code"));
            if (string.IsNullOrEmpty(message_code_header)
                || request.Method != HttpMethod.Post) throw new Exception("Invalid request");

            int message_code = Convert.ToInt32(message_code_header);
            string? token = request.Headers.Authorization?.ToString();

            Task<string>? input = request.Content?.ReadAsStringAsync(cancellationToken);
            if (input is null)
                throw new("Invalid input!");

            return HandleMessages(message_code, JsonConvert.DeserializeObject(input.Result), token);
        }
        catch (Exception ex)
        {
            return Respond(new
            {
                code = 404,
                status = "Error 404",
                message = ex.Message,
                result = new { }
            },
            HttpStatusCode.NotFound);
        }
    }

    private Task<HttpResponseMessage> Respond(object content, HttpStatusCode StatusCode = HttpStatusCode.OK)
    {
        return Task.FromResult(new HttpResponseMessage()
        {
            StatusCode = StatusCode,
            Content = new StringContent(JsonConvert.SerializeObject(content))
        });
    }

    private Task<HttpResponseMessage> HandleMessages(int message_code, object? payload, string? token = null)
    {
        if (message_code == 1)
            return UserLogin(payload);

        if (token != "Bearer " + tokenValue)
            throw new Exception("Unauthorized access!");

        return message_code switch
        {
            101 => Respond(new
            {
                code = 200,
                status = "success",
                message = "",
                result = new
                {
                    title = "Welcome to Gerege Systems"
                }
            }),
            102 => Respond(new
            {
                code = 200,
                status = "success",
                message = "",
                result = LoadEmbeddedJson("gerege_partners.json")
            }),
            _ => throw new Exception(message_code + ": Message code not found"),
        };
    }

    private Task<HttpResponseMessage> UserLogin(dynamic? payload)
    {
        string username = Convert.ToString(payload?.username ?? "");
        string password = Convert.ToString(payload?.password ?? "");

        if (string.IsNullOrEmpty(username)
            || string.IsNullOrEmpty(password))
            throw new Exception("Please provide login information!");

        if (username != "Gerege" || password != "mongol")
            throw new Exception("Invalid user login!");

        return Respond(new {
            code = 200,
            status = "success",
            message = "fetching user token",
            result = new {
                token = tokenValue,
                expires_in = 300,
                expires = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd H:mm:ss")
            }
        });
    }

    private dynamic? LoadEmbeddedJson(string szFilePath)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(typeof(MockServerHandler).Namespace + "." + szFilePath);
            if (stream == null)
                throw new FileNotFoundException(assembly.GetName().Name + " дотор " + szFilePath + " гэсэн файл байхгүй л байна даа!");

            using StreamReader reader = new StreamReader(stream);
            return JsonConvert.DeserializeObject(reader.ReadToEnd());
        }
        catch (Exception)
        {
            throw;
        }
    }
}
