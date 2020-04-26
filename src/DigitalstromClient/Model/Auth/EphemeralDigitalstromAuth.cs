﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Model.Auth
{
    public class EphemeralDigitalstromAuth : IDigitalstromAuth, IEquatable<EphemeralDigitalstromAuth>
    {
        public virtual string? ApplicationToken { get; private set; }
        public virtual string? SessionToken { get; private set; }
        public virtual DateTime SessionExpiration { get; private set; }

        private string? _appId = null;
        private string? _username = null;
        private string? _userPassword = null;
        private readonly Semaphore _loginSemaphore = new Semaphore(1, 1);
        private readonly Func<IDigitalstromAuth>? _credentialsCallback;

        public string AppId { get { CheckCallback(); return _appId!; } }
        public string Username { get { CheckCallback(); return _username!; } }
        public string UserPassword { get { CheckCallback(); return _userPassword!; } }

        public EphemeralDigitalstromAuth(Func<IDigitalstromAuth> credentialsCallback)
        {
            _credentialsCallback = credentialsCallback;
        }

        public EphemeralDigitalstromAuth(string appId, string username, string password)
        {
            _credentialsCallback = null;
            _appId = appId;
            _username = username;
            _userPassword = password;
        }

        public bool MustFetchApplicationToken()
        {
            try
            {
                _loginSemaphore.WaitOne();
                return string.IsNullOrEmpty(ApplicationToken);
            }
            finally { _loginSemaphore.Release(); }
        }

        public bool MustFetchSessionToken()
        {
            return string.IsNullOrEmpty(SessionToken) || SessionExpiration.ToUniversalTime().CompareTo(DateTime.UtcNow) < 0;
        }

        public async Task TouchSessionTokenAsync()
        {
            if (ApplicationToken is null)
                return;

            await UpdateTokenAsync(SessionToken, DateTime.UtcNow.AddSeconds(60), ApplicationToken);
        }

        public virtual async Task UpdateTokenAsync(string? sessionToken, DateTime sessionExpiration, string applicationToken)
        {
            ApplicationToken = applicationToken;
            SessionToken = sessionToken;
            SessionExpiration = sessionExpiration;
            await Task.CompletedTask;
            return;
        }

        public IDigitalstromAuth DeepClone()
        {
            return new EphemeralDigitalstromAuth(AppId, Username, UserPassword)
            {
                ApplicationToken = ApplicationToken,
                SessionToken = SessionToken,
                SessionExpiration = SessionExpiration
            };
        }

        public bool Equals(EphemeralDigitalstromAuth other)
        {
            return ApplicationToken == other.ApplicationToken && SessionToken == other.SessionToken
                && SessionExpiration == other.SessionExpiration && AppId == other.AppId
                && Username == other.Username && UserPassword == other.UserPassword;
        }

        /// <summary>
        /// The three private fields _appId, _username and _userPassword are guaranteed to be
        /// non-null after this method returns, or an exception is thrown.
        /// </summary>
        private void CheckCallback()
        {
            try
            {
                _loginSemaphore.WaitOne();

                bool anyValueIsNull() => _appId is null || _username is null || _userPassword is null;
                if (anyValueIsNull() && !(_credentialsCallback is null))
                {
                    var credentials = _credentialsCallback();
                    _appId = credentials.AppId;
                    _username = credentials.Username;
                    _userPassword = credentials.UserPassword;
                }

                if (anyValueIsNull())
                    throw new Exception("Credentials could not be retrieved by callback.");
            }
            finally { _loginSemaphore.Release(); }
        }
    }
}