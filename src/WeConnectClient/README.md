﻿[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.WeConnectClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.WeConnectClient/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# WeConnectClient

This class library provides a way to call the Volkswagen WeConnect interfaces. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.WeConnectClient
```

## Usage

You have to implement the interfaces `IWeConnectAuth` and `IWeConnectConnectionProvider` to provide the WeConnect client with all information necessary to authenticate and establish a connection.

The minimal viable example for playing around with the client would be to create a default connection provider as shown here:

```csharp
var auth = new WeConnectAuth("<username>", "<password>");
using var connectionProvider = new WeConnectConnectionProvider(auth);
```

> **Caution:** in a productive use you may want to implement your own `IWeConnectAuth`, let user credentials be entered by the user in some way and immediatelly discard them again. The `IWeConnectAuth` object will contain session information that is used to stay logged in and can be persisted by your custom subclass.

If you have the connection providers in place, you can create and use the WeConnect client like this:

```csharp
var weclient = new WeConnectPortalClient(connectionProvider);

// Get the first vehicle of the logged in user
var vehicle = (await weclient.GetVehicleList()).FirstOrDefault() ?? new();
Console.WriteLine($"VIN: {vehicle.Vin} license plate: {vehicle.LicensePlate}");

// Get statistics about trips
var trips = await weclient.GetLongtermTripStatistics(vehicle.Vin);
foreach (var trip in trips)
    Console.WriteLine($"Trip {trip.TripEndTimestamp}: avg. speed: {trip.AverageSpeedKmph}");
```

For more usage examples you can also have a look at the [unit tests](../../test/WeConnectClient.Tests).

## Platform Support

WeConnectClient is targeted for .NET 9.0 or higher.

## License

The MIT License (MIT)

Copyright (c) 2019-2024 Philip Daubmeier

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
