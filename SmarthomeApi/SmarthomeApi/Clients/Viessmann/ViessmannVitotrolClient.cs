﻿using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmarthomeApi.Clients.Viessmann
{
    public class ViessmannVitotrolClient
    {
        private const string _username = "***REMOVED***";
        private const string _password = "***REMOVED***";
        private const string _deviceId = "***REMOVED***";
        private const string _installationId = "***REMOVED***";

        private static readonly HttpClient _client = new HttpClient();

        private const string _soapUri = @"https://api.viessmann.io/vitotrol/soap/v1.0/iPhoneWebService.asmx";
        private const string _soapPrefix = @"<?xml version=""1.0"" encoding=""UTF-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns=""http://www.e-controlnet.de/services/vii/""><soap:Body>";
        private const string _soapSuffix = @"</soap:Body></soap:Envelope>";
        private const string _soapAction = @"http://www.e-controlnet.de/services/vii/";

        private TokenStore _tokenStore;

        public ViessmannVitotrolClient(PersistenceContext databaseContext)
        {
            _tokenStore = new TokenStore(databaseContext, "viessmann_vitotrol");
        }

        public async Task<List<KeyValuePair<string, string>>> GetTypeInfo()
        {
            await Authenticate();

            var body = $"<GeraetId>{_deviceId}</GeraetId><AnlageId>{_installationId}</AnlageId>";
            return await ParseTypeInfo(await SendSoap("GetTypeInfo", body, _tokenStore.AccessToken));
        }

        public async Task<List<Tuple<string, string, DateTime>>> GetData(IEnumerable<int> datapoints)
        {
            await Authenticate();

            var datapointList = string.Join("</int><int>", datapoints.Select(x => x.ToString()));
            var body = $"<UseCache>false</UseCache><GeraetId>{_deviceId}</GeraetId><AnlageId>{_installationId}</AnlageId><DatenpunktIds><int>{datapointList}</int></DatenpunktIds>";
            return await ParseData(await SendSoap("GetData", body, _tokenStore.AccessToken));
        }

        private async Task Authenticate()
        {
            if (_tokenStore.IsAccessTokenValid())
                return;

            var body = $"<Betriebssystem>Android</Betriebssystem><AppId>prod</AppId><Benutzer>{_username}</Benutzer><AppVersion>93</AppVersion><Passwort>{_password}</Passwort>";
            var response = await SendSoap("Login", body, null);

            var cookies = response.Headers.GetValues("Set-Cookie").Select(x => x.Replace("path=/; HttpOnly", "").Trim().TrimEnd(';')).ToList();
            var sessiontoken = string.Join(";", cookies);

            await _tokenStore.UpdateToken(sessiontoken, DateTime.Now.AddHours(1), string.Empty);
        }

        private async Task<HttpResponseMessage> SendSoap(string action, string body, string token)
        {
            var soapBody = $"{_soapPrefix}<{action}>{body}</{action}>{_soapSuffix}";
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_soapUri));
            request.Headers.Add("SOAPAction", $"{_soapAction}{action}");
            request.Headers.Add("Connection", "Keep-Alive");
            request.Headers.TryAddWithoutValidation("User-Agent", "Dalvik/2.1.0 (Linux; U; Android 9; Nokia 8 Sirocco Build/PPR1.180610.011)");
            request.Headers.Host = "api.viessmann.io";
            if (token != null)
                request.Headers.Add("Cookie", token);
            request.Content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

            return await _client.SendAsync(request);
        }

        private async Task<List<KeyValuePair<string, string>>> ParseTypeInfo(HttpResponseMessage response)
        {
            var elem = await XElement.LoadAsync(await response.Content.ReadAsStreamAsync(), LoadOptions.None, new CancellationToken());
            return elem.Descendants().First(x=>x.Name.LocalName.Equals("TypeInfoListe", StringComparison.InvariantCultureIgnoreCase))
                .Descendants().Where(x => x.Name.LocalName.Equals("DatenpunktTypInfo", StringComparison.InvariantCultureIgnoreCase)).Select(d =>
            {
                return new KeyValuePair<string, string>(
                    d.Descendants().First(x => x.Name.LocalName.Equals("DatenpunktId", StringComparison.InvariantCultureIgnoreCase)).Value,
                    d.Descendants().First(x => x.Name.LocalName.Equals("DatenpunktName", StringComparison.InvariantCultureIgnoreCase)).Value
                );
            }).ToList();
        }

        private async Task<List<Tuple<string, string, DateTime>>> ParseData(HttpResponseMessage response)
        {
            var elem = await XElement.LoadAsync(await response.Content.ReadAsStreamAsync(), LoadOptions.None, new CancellationToken());
            return elem.Descendants().First(x => x.Name.LocalName.Equals("DatenwerteListe", StringComparison.InvariantCultureIgnoreCase))
                .Descendants().Where(x => x.Name.LocalName.Equals("WerteListe", StringComparison.InvariantCultureIgnoreCase)).Select(d =>
                {
                    return new Tuple<string, string, DateTime>(
                        d.Descendants().First(x => x.Name.LocalName.Equals("DatenpunktId", StringComparison.InvariantCultureIgnoreCase)).Value,
                        d.Descendants().First(x => x.Name.LocalName.Equals("Wert", StringComparison.InvariantCultureIgnoreCase)).Value,
                        DateTime.ParseExact(
                            d.Descendants().First(x => x.Name.LocalName.Equals("Zeitstempel", StringComparison.InvariantCultureIgnoreCase)).Value,
                            "yyyy'-'MM'-'dd HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToLocalTime()
                    );
                }).ToList();
        }
    }
}