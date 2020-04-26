﻿using PhilipDaubmeier.TokenStore;
using PhilipDaubmeier.ViessmannClient.Network;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannAuth<T> : IViessmannAuth
    {
        private readonly TokenStore<T> _tokenStore;

        public string? AccessToken => _tokenStore.AccessToken;
        public DateTime AccessTokenExpiry => _tokenStore.AccessTokenExpiry;

        public string Username { get; }
        public string UserPassword { get; }

        public ViessmannAuth(TokenStore<T> tokenStore, string username, string password)
        {
            _tokenStore = tokenStore;
            Username = username;
            UserPassword = password;
        }

        public bool IsAccessTokenValid() => _tokenStore.IsAccessTokenValid();

        public async Task UpdateTokenAsync(string? sessionToken, DateTime sessionExpiration, string? applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken ?? string.Empty, sessionExpiration, applicationToken ?? string.Empty);
        }
    }
}