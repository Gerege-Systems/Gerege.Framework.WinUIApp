using System;
using System.Net.Http;
using Gerege.Framework.HttpClient;

/////// date: 2022.02.09 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace SampleApp;

/// <inheritdoc />
public class SampleUserClient : GeregeClient
{
    /// <inheritdoc />
    public class SampleToken : GeregeToken
    {
        /// <summary>
        /// Гэрэгэ токен авах мсж дугаар.
        /// </summary>
        public virtual int GeregeMessage() => 1;
    }

    /// <inheritdoc />
    public SampleUserClient(HttpMessageHandler handler) : base(handler)
    {
        BaseAddress = new Uri(this.AppRaiseEvent("get-server-address"));
    }

    /// <summary>
    /// Терминал нэвтрэх.
    /// </summary>
    /// <param name="payload">Нэвтрэх мэдээлэл.</param>
    public void Login(object payload)
    {
        GeregeToken? token = FetchToken(payload);

        if (string.IsNullOrEmpty(token?.Value))
            throw new Exception("Invalid token data!");

        this.AppRaiseEvent("client-login");
    }

    GeregeToken? currentToken = null;
    object? fetchTokenPayload = null;

    /// <inheritdoc />
    protected override GeregeToken? FetchToken(object? payload = null)
    {
        if (payload != null)
            fetchTokenPayload = payload;

        if (currentToken != null
            && currentToken.IsExpiring)
        {
            currentToken = null;
            payload = fetchTokenPayload;
        }

        if (payload != null)
            currentToken = Request<SampleToken>(payload);

        return currentToken;
    }
}
