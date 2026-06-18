# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`PixelByProxy.Asus.Router` (AsusRouterTools) is a .NET library that talks to the web UI/API of ASUS routers (the same `appGet.cgi` / `*.asp` endpoints the router's own web admin console uses) to read status info and, experimentally, write firewall settings. It is a port of the Python [AsusRouterMonitor](https://github.com/lmeulen/AsusRouterMonitor) project to C#.

## Commands

```
dotnet build PixelByProxy.Asus.Router.sln
dotnet test src/Tests/PixelByProxy.Asus.Router.FunctionalTests/PixelByProxy.Asus.Router.FunctionalTests.csproj
dotnet test --filter "FullyQualifiedName~FirewallServiceTests.SetFirewallSettingsAsync_WithIpV6Rules_UpdatesIpV6FirewallRules"
```

There is no unit test project — `PixelByProxy.Asus.Router.FunctionalTests` is the only test project, and despite the xUnit `[Fact]`/`[Theory]` structure, the tests are **functional/integration tests that hit a real, live ASUS router** (last verified against an RT-AX86U on firmware 3.0.0.4.388_22068). They are not safe to run without a configured router and will mutate router state (e.g. firewall enabled/disabled, firewall rule lists) as part of the test run — `ConfigurationFixture` snapshots firewall settings in `InitializeAsync` and restores them in `DisposeAsync`, but a crash mid-test can leave the router in a different state than before.

Router connection details are supplied via .NET User Secrets (`UserSecretsId` in the test csproj), bound from a config section named `AsusConfiguration` into the `AsusConfigurationSecrets` class (which is just an `AsusConfiguration` subclass) — see `src/Tests/.../Configuration/ConfigurationFixture.cs`. There is no secrets.json checked into the repo; a developer running these tests needs their own router credentials set via `dotnet user-secrets set` against the test project's `UserSecretsId`.

## Architecture

- **`src/Lib`** — the actual NuGet package (`PixelByProxy.Asus.Router`, targets net6.0). Only dependency is Newtonsoft.Json.
- **`src/Tests/PixelByProxy.Asus.Router.FunctionalTests`** — functional tests as described above.

### Service layer

- `ServiceBase` (`src/Lib/Services/ServiceBase.cs`) holds the shared HTTP plumbing all services build on:
  - `AuthenticateAsync`/`GetAuthTokenAsync` — logs into the router via `/login.cgi` with base64'd `user:pass`, caching the resulting `asus_token` in **static** fields (`_authToken`/`_authCreds`) shared across all instances/services for the process lifetime, re-authenticating only if credentials change.
  - `GetResponseAsync` builds the request (adds the router's expected `User-Agent`, attaches the `asus_token` cookie when `authenticate` is true), and throws `HttpRequestException` if the response body contains `error_status`.
  - `GetHookResponseAsync`/`GetHookResponseJsonAsync` call the router's `/appGet.cgi?hook=<name>()` endpoint, which is how most read-only status info is fetched.
  - `EnsureWriteAccess()` — guards any mutating call; throws unless `AsusConfiguration.EnableWrite` is true. Write support is explicitly experimental (per README).
- `AsusService`/`IAsusService` — read-only status/info methods (uptime, memory/CPU usage, connected clients, traffic, WAN status, router/LAN settings, DHCP leases, web history). Many of these endpoints return near-JSON or JS-snippet text rather than valid JSON, so methods patch the string (`response.Replace(...)`) or use regex (`UptimeRegex`, `WanStatusRegex`) to coerce it into something `JsonConvert` can parse. `GetRouterSettingsAsync`/`GetLanStatusAsync` fan out parallel `nvram_get(key)` hook calls per settings key and merge the resulting JSON objects.
- `FirewallService`/`IFirewallService` — reads firewall settings by scraping `/Advanced_BasicFirewall_Content.asp` HTML with `FirewallRegex` (pulls `firewall_enable`, `ipv6_fw_rulelist_array`, `ipv4_fw_rulelist_array` out of inline `<script>`), then URL-decodes and splits the rule list strings (`<` between rules, `>` between fields) into `FirewallRuleIpV6`/`FirewallRuleIpV4`. Writes (`SetFirewallSettingsAsync`) require `EnableWrite`, validate each rule via `FirewallRuleExtensions.IsRuleValid` (IP/CIDR format, port range, protocol, max 128 rules, no `< > ' %` in service names), then re-serialize and POST to `/start_apply.htm`.

Both services are stateless wrappers around `ServiceBase` and are constructed directly with `new XService(asusConfiguration, httpClient)` — there's no DI container or factory; callers own the `HttpClient` and `AsusConfiguration` lifetime.

### Other notable pieces

- `src/Lib/Extensions/FirewallRuleExtensions.cs` — internal-only validation/serialization logic for firewall rules (regex-based IPv6/CIDR validation, port range parsing, invalid-character checks). This is where new firewall rule validation rules belong.
- `src/Lib/Json/BoolConverter.cs` — custom Newtonsoft converter because the router's APIs represent booleans as `"0"`/`"1"` strings in spots.
- `src/Lib/Messages/ErrorStrings.cs` — centralized error message templates; tests assert on these exact messages, so changing wording here is a breaking change for the functional test suite.
- Models (`src/Lib/Models`) are mostly flat POCOs deserialized straight from router JSON via `[JsonProperty]` attributes — when adding a new field, check the router's actual JSON key naming (it's an inconsistent mix of snake_case/abbreviations from the underlying firmware).
