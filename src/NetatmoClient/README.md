﻿[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.NetatmoClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.NetatmoClient/)

# NetatmoClient

NetatmoClient is a .NET Core class library with a wrapper for the Netatmo Cloud RESTful JSON web service. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface of the Netatmo API.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.NetatmoClient
```

## Usage

You have to implement the interface `INetatmoConnectionProvider` to provide the `NetatmoWebClient` with all information necessary to authenticate at the API and establish a connection.

The minimal viable example for playing around with the client would be as follows:

```csharp
public class NetatmoConnProvider : INetatmoConnectionProvider
{
    public INetatmoAuth AuthData => new NetatmoAuth("<username>", "<password>");
    public HttpMessageHandler Handler => null;

    public string AppId => "<your_netatmo_connect_app_id>";
    public string AppSecret => "<your_netatmo_connect_app_secret>";
    public string Scope => "read_station read_presence access_presence";
}
```

> **Caution:** in a productive use you may want to load your app id and secret from a suitable vault and the user credentials should be entered by the user in some way and immediatelly discarded again. The `INetatmoAuth` object will contain a refresh token that can be used to re-authenticate at any time, which can be persisted by implementing a custom `INetatmoAuth` class. You can have a look at the respective classes in [`GraphIoT.Netatmo`](../GraphIoT.Netatmo/Config) as an example.

If you have the connection provider class in place, you can create a `NetatmoWebClient` and query station data and measurements:

```csharp
var netatmoClient = new NetatmoWebClient(new NetatmoConnProvider());

// Find the id of the first base station of the logged in user
var weatherStation = await netatmoClient.GetWeatherStationData();
var baseStationId = weatherStation.Devices.First().Id;

// Get temperature values of the past 24 hours of the base station
var start = DateTime.Now.AddDays(-1);
var end = DateTime.Now;
var measureTypes = new Measure[] { MeasureType.Temperature };
var measures = await netatmoClient.GetMeasure(baseStationId, baseStationId,
                     measureTypes, MeasureScale.Scale1Hour, start, end);

// Print out all values
foreach (var measure in measures.First().Value)
    Console.WriteLine($"timestamp: {measure.Key} temperature: {measure.Value} °C");
```

## Platform Support

CompactTimeSeries is compiled for .NET Core 2.2.

## License

The MIT License (MIT)

Copyright (c) 2019 Philip Daubmeier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.