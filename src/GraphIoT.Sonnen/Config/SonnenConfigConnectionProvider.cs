﻿using Microsoft.Extensions.Options;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenClient.Network;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Config
{
    public class SonnenHttpClient
    {
        public SonnenHttpClient(HttpClient client) => Client = client;
        public HttpClient Client { get; private set; }
    }

    public class SonnenConfigConnectionProvider : SonnenConnectionProvider
    {
        public SonnenConfigConnectionProvider(TokenStore<SonnenPortalClient> tokenStore, IOptions<SonnenConfig> config, SonnenHttpClient client)
        {
            ClientId = config.Value.ClientId;

            AuthData = new SonnenHostAuth(tokenStore, config.Value.Username, config.Value.Password);

            Client = client.Client;
        }
    }
}