# AsusRouterTools

A class to access functionality from your Asus router.

## Usage

```csharp
var config = new AsusConfiguration
{
    HostName = "router.asus.com",
    UserName = userName,
    Password = password
};

var httpClient = new HttpClient();
var service = new AsusService(config, httpClient);
var clients = await service.GetClientsAsync();

Console.WriteLine($"Clients: {clients.Count}");
```

## Available Methods
```
GetTokenAsync            - Gets the authentication token.
GetUptimeAsync           - Gets the uptime and time of last boot.
GetMemoryUsageAsync      - Gets memory usage statistics.
GetCpuUsageAsync         - Gets CPU usage statistics.
GetClientsAsync          - Gets info for all of the connected clients.
GetTrafficAsync          - Gets the current and total network usage.
GetWanStatusAsync        - Gets WAN status information.
GetRouterSettingsAsync   - Gets various router settings.
GetLanStatusAsync        - Gets LAN status information.
GetDhcpLeaseMacListAsync - Gets a list of DHCP leases given out.
```

## Credits

This work is based on the [AsusRouterMonitor](https://github.com/lmeulen/AsusRouterMonitor) python library by [Leo van der Meulen](https://github.com/lmeulen) re-written in C#.