﻿using Newtonsoft.Json;
using PhilipDaubmeier.ViessmannClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient
{
    public class ViessmannPlatformClient
    {
        private readonly IViessmannConnectionProvider<ViessmannPlatformClient> _connectionProvider;

        private readonly HttpClient _client;
        private readonly HttpClient _authClient;

        private const string _authUri = "https://iam.viessmann.com/idp/v1/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v1/token";
        private const string _redirectUri = "vicare://oauth-callback/everest";

        public enum Circuit
        {
            Circuit0,
            Circuit1
        }

        public ViessmannPlatformClient(IViessmannConnectionProvider<ViessmannPlatformClient> connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _client = connectionProvider.Client;
            _authClient = connectionProvider.AuthClient;
        }

        public async Task<string> GetInstallations()
        {
            var uri = new Uri("https://api.viessmann-platform.io/general-management/v1/installations?expanded=true");
            return await (await CallApi(uri)).Content.ReadAsStringAsync();
        }

        public async Task<(string status, double temperature)> GetOutsideTemperature()
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature("heating.sensors.temperature.outside"), "status", "value");
            return (res.Item1 ?? string.Empty, ((double?)res.Item2) ?? 0d);
        }

        public async Task<double> GetBoilerTemperature()
        {
            var res = await ParseFeatureResponse<ClassNullable<double>>(await GetFeature("heating.boiler.temperature"), "value");
            return ((double?)res) ?? 0d;
        }

        public async Task<bool> GetBurnerActiveStatus()
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>>(await GetFeature("heating.burner"), "active");
            return ((bool?)res) ?? false;
        }

        public async Task<(double hours, int starts)> GetBurnerStatistics()
        {
            var res = await ParseFeatureResponse<ClassNullable<double>, ClassNullable<double>>(await GetFeature("heating.burner.statistics"), "hours", "starts");
            return (((double?)res.Item1) ?? 0d, (int)(((double?)res.Item2) ?? 0d));
        }

        public async Task<string> GetCircuitOperatingMode(Circuit circuit)
        {
            return await ParseFeatureResponse<string>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.modes.active"), "value") ?? string.Empty;
        }

        public async Task<string> GetCircuitActiveProgram(Circuit circuit)
        {
            return await ParseFeatureResponse<string>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.active"), "value") ?? string.Empty;
        }

        public async Task<(bool active, double temperature)> GetCircuitProgramNormal(Circuit circuit)
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.normal"), "active", "temperature");
            return (((bool?)res.Item1) ?? false, ((double?)res.Item2) ?? 0d);
        }

        public async Task<(bool active, double temperature)> GetCircuitProgramReduced(Circuit circuit)
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.reduced"), "active", "temperature");
            return (((bool?)res.Item1) ?? false, ((double?)res.Item2) ?? 0d);
        }

        public async Task<(bool active, double temperature)> GetCircuitProgramComfort(Circuit circuit)
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.comfort"), "active", "temperature");
            return (((bool?)res.Item1) ?? false, ((double?)res.Item2) ?? 0d);
        }

        public async Task<(string status, double temperature)> GetCircuitTemperature(Circuit circuit)
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.sensors.temperature.supply"), "status", "value");
            return (res.Item1 ?? string.Empty, ((double?)res.Item2) ?? 0d);
        }

        public async Task<bool> GetCircuitCirculationPump(Circuit circuit)
        {
            return (await ParseFeatureResponse<string>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.circulation.pump"), "status"))?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<(string status, double temperature)> GetDhwStorageTemperature()
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature("heating.dhw.sensors.temperature.hotWaterStorage"), "status", "value");
            return (res.Item1 ?? string.Empty, ((double?)res.Item2) ?? 0d);
        }

        public async Task<bool> GetDhwPrimaryPump()
        {
            return (await ParseFeatureResponse<string>(await GetFeature("heating.dhw.pumps.primary"), "status"))?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<bool> GetDhwCirculationPump()
        {
            return (await ParseFeatureResponse<string>(await GetFeature("heating.dhw.pumps.circulation"), "status"))?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<(string status, double temperature)> GetBoilerTemperatureMain()
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature("heating.boiler.sensors.temperature.main"), "status", "value");
            return (res.Item1 ?? string.Empty, ((double?)res.Item2) ?? 0d);
        }

        public async Task<int> GetBurnerModulation()
        {
            var res = await ParseFeatureResponse<ClassNullable<double>>(await GetFeature("heating.burner.modulation"), "value");
            return (int)(((double?)res) ?? 0d);
        }

        private string CircuitNumber(Circuit circuit)
        {
            return circuit == Circuit.Circuit0 ? "0" : "1";
        }

        private async Task<HttpResponseMessage> GetFeature(string featureName)
        {
            var uri = $"https://api.viessmann-platform.io/operational-data/v1/installations/{_connectionProvider.PlattformInstallationId}/gateways/{_connectionProvider.PlattformGatewayId}/devices/0/features/{featureName}";
            return await CallApi(new Uri(uri));
        }

        private async Task<HttpResponseMessage> CallApi(Uri uri)
        {
            await Authenticate();

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _connectionProvider.AuthData.AccessToken);

            return await _client.SendAsync(request);
        }

        private async Task Authenticate()
        {
            if (_connectionProvider.AuthData.IsAccessTokenValid())
                return;

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_authUri}?type=web_server&client_id={_connectionProvider.PlattformApiClientId}&redirect_uri={_redirectUri}&response_type=code"),
                Method = HttpMethod.Get,
            };

            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_connectionProvider.AuthData.Username}:{_connectionProvider.AuthData.UserPassword}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            var response = await _authClient.SendAsync(request);
            var location = response.Headers.Location.AbsoluteUri;

            var prefix = $"{_redirectUri}?code=";
            if (!location.StartsWith(prefix) || location.Length <= prefix.Length)
                throw new Exception("could not retrieve auth code");

            var authorization_code = location.Substring(prefix.Length);

            Tuple<string, DateTime> loadedToken = await ParseTokenResponse(await _client.PostAsync(
                new Uri(_tokenUri), new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _connectionProvider.PlattformApiClientId),
                    new KeyValuePair<string, string>("client_secret", _connectionProvider.PlattformApiClientSecret),
                    new KeyValuePair<string, string>("code", authorization_code),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri)
                })));

            await _connectionProvider.AuthData.UpdateTokenAsync(loadedToken.Item1, loadedToken.Item2, string.Empty);
        }

        private async Task<Tuple<string, DateTime>> ParseTokenResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                access_token = "",
                expires_in = "",
                token_type = ""
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var authRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return new Tuple<string, DateTime>(authRaw.access_token, DateTime.Now.AddSeconds(int.Parse(authRaw.expires_in)));
        }

        private class ClassNullable<T> where T : struct
        {
            private readonly T? _val;
            public ClassNullable(T? value) { _val = value; }
            public override string ToString() { return _val?.ToString() ?? string.Empty; }
            public static implicit operator ClassNullable<T>(T? value) { return new ClassNullable<T>(value); }
            public static implicit operator T?(ClassNullable<T>? nullable) { return nullable is null ? default : nullable._val; }
        }

        private async Task<T?> ParseFeatureResponse<T>(HttpResponseMessage response, string attrName) where T : class
        {
            return (await ParseFeatureResponse<T, string>(response, attrName)).Item1;
        }

        private async Task<Tuple<T1?, T2?>> ParseFeatureResponse<T1, T2>(HttpResponseMessage response, params string[] attrNames) where T1 : class where T2 : class
        {
            var subDefLinks = new[] { new
                {
                    rel = new List<string>(),
                    href = ""
                } };
            var subDefEntities = new[] { new
                {
                    rel = new List<string>(),
                    properties = new {
                        apiVersion = 0,
                        isEnabled = true,
                        isReady = true,
                        gatewayId = "",
                        feature = "",
                        uri = "",
                        deviceId = "",
                        timestamp = ""
                    }
                } };
            var definition = new
            {
                links = subDefLinks,
                @class = new List<string>(),
                properties = new
                {
                    value = new
                    {
                        type = "",
                        value = 0d
                    },
                    active = new
                    {
                        type = "",
                        value = false
                    },
                    status = new
                    {
                        type = "",
                        value = ""
                    },
                    temperature = new
                    {
                        type = "",
                        value = 0d
                    },
                    hours = new
                    {
                        type = "",
                        value = 0d
                    },
                    starts = new
                    {
                        type = "",
                        value = 0d
                    }
                },
                entities = subDefEntities,
                actions = new List<object>()
            };
            var definitionStrVal = new
            {
                links = subDefLinks,
                @class = new List<string>(),
                properties = new
                {
                    value = new
                    {
                        type = "",
                        value = ""
                    }
                },
                entities = subDefEntities,
                actions = new List<object>()
            };

            var responseStr = await response.Content.ReadAsStringAsync();

            var typeStringRaw = JsonConvert.DeserializeAnonymousType(responseStr, new { properties = new { value = new { type = "" } } });
            if (typeStringRaw.properties?.value?.type?.Trim()?.Equals("string", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                var featureStrRaw = JsonConvert.DeserializeAnonymousType(responseStr, definitionStrVal);
                return new Tuple<T1?, T2?>(featureStrRaw.properties?.value?.value as T1, default);
            }

            var featureRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return (attrNames.FirstOrDefault()?.Trim()?.ToLowerInvariant() ?? string.Empty) switch
            {
                "active" => new Tuple<T1?, T2?>(((ClassNullable<bool>)featureRaw.properties?.active?.value) as T1, ((ClassNullable<double>)featureRaw.properties?.temperature?.value) as T2),
                "hours" => new Tuple<T1?, T2?>(((ClassNullable<double>)featureRaw.properties?.hours?.value) as T1, ((ClassNullable<double>)featureRaw.properties?.starts?.value) as T2),
                "status" => new Tuple<T1?, T2?>(featureRaw.properties?.status?.value as T1, ((ClassNullable<double>)featureRaw.properties?.value?.value) as T2),
                "value" => new Tuple<T1?, T2?>(((ClassNullable<double>)featureRaw.properties?.value?.value) as T1, default),
                _ => new Tuple<T1?, T2?>(default, default),
            };
        }
    }
}