using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

            string? token = request.Headers.Authorization?.ToString();
            if (token != $"Bearer {tokenValue}")
                throw new Exception("Unauthorized access!");

            string? path = request.RequestUri?.AbsolutePath;

            if (request.Method == HttpMethod.Post
                && path == "/user/login")
            {
                Task<string>? input = request.Content?.ReadAsStringAsync(cancellationToken);
                var payload = JsonSerializer.Deserialize<JsonElement>(input?.Result ?? "{}");
                if (!payload.TryGetProperty("username", out JsonElement username)
                    || payload.TryGetProperty("password", out JsonElement password))
                    throw new Exception("Please provide login information!");

                if (username.ToString() != "Gerege"
                    || password.ToString() != "mongol")
                    throw new Exception("Invalid user login!");

                return Respond(new
                {
                    code = 200,
                    status = "success",
                    message = "fetching user token",
                    result = new
                    {
                        token = tokenValue,
                        expires_in = 300,
                        expires = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd H:mm:ss")
                    }
                });
            }
            else if(request.Method == HttpMethod.Get)
                return path switch
                {
                    "/get/title" => Respond(new
                    {
                        code = 200,
                        status = "success",
                        message = "",
                        result = new
                        {
                            title = "Welcome to Gerege Systems"
                        }
                    }),
                    "/get/partners" => Respond(new
                    {
                        code = 200,
                        status = "success",
                        message = "",
                        result = LoadEmbeddedJson("gerege_partners.json")
                    }),
                    _ => throw new Exception("Unknown route pattern")
                };

            throw new Exception("Not Found");
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
            Content = new StringContent(JsonSerializer.Serialize(content))
        });
    }

    private object? LoadEmbeddedJson(string szFilePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{typeof(MockServerHandler).Namespace}.{szFilePath}")
            ?? throw new FileNotFoundException($"{assembly.GetName().Name} дотор {szFilePath} гэсэн файл байхгүй л байна даа!");
        using StreamReader reader = new StreamReader(stream);
        return JsonSerializer.Deserialize<object>(reader.ReadToEnd());
    }
}
