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
var asusService = new AsusService(config, httpClient);
var firewallService = new FirewallService(config, httpClient);

var clients = await asusService.GetClientsAsync();
Console.WriteLine($"Clients: {clients.Count}");

var firewallSettings = await firewallService.GetFirewallSettingsAsync();
Console.WriteLine($"Firewall Enabled: {firewallSettings.Enabled}");
```

### Write Access

Note that the write methods will throw an exception unless you set EnableWrite on the AsusConfiguration to true.
Write access is currently experiemental, use at your own risk.

## AsusService Methods
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
GetWebHistory            - Gets the web history for the clients, note this requires QoS to be enabled.
```

## FirewallService Methods
```
GetFirewallSettingsAsync - Gets the current firewall settings.
SetFirewallSettingsAsync - Set the firewall settings to the defined values.
```

## Testing

Last tested on an ASUS RT-AX86U with firmware version 3.0.0.4.388_22068.

## Credits

This work is based on the [AsusRouterMonitor](https://github.com/lmeulen/AsusRouterMonitor) python library by [Leo van der Meulen](https://github.com/lmeulen) re-written in C#.