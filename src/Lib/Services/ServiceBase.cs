using System.Net;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Messages;

namespace PixelByProxy.Asus.Router.Services;

/// <summary>
/// Base class for the router services.
/// </summary>
public class ServiceBase
{
    #region Constants

    private const string UserAgent = "asusrouter-Android-DUTUtil-1.0.0.245";

    #endregion

    #region Fields

    private readonly AsusConfiguration _asusConfig;
    private readonly HttpClient _httpClient;

    private static string? _authToken;
    private static string? _authCreds;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of the service.
    /// </summary>
    /// <param name="asusConfig">The Asus router configuration settings.</param>
    /// <param name="httpClient">Http Client for communication with the Asus APIs.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ServiceBase(AsusConfiguration asusConfig, HttpClient httpClient)
    {
        _asusConfig = asusConfig;
        _httpClient = httpClient;
    }

    #endregion

    #region Protected Methods

    protected void EnsureWriteAccess()
    {
        if (!_asusConfig.EnableWrite)
            throw new InvalidOperationException(ErrorStrings.EnableWriteAccess);
    }

    protected async Task<string> GetAuthTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var creds = $"{_asusConfig.UserName}:{_asusConfig.Password}";
            var login = Convert.ToBase64String(Encoding.ASCII.GetBytes(creds));
            var content = $"login_authorization={login}";

            var json = await GetResponseJsonAsync("/login.cgi", HttpMethod.Post, content, false, cancellationToken).ConfigureAwait(false);
            var token = json["asus_token"]!.ToString();
            return token;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new HttpRequestException("Authentication Failed", ex, HttpStatusCode.Unauthorized);
        }
    }

    protected async Task<string> AuthenticateAsync(CancellationToken cancellationToken)
    {
        var currentCreds = $"{_asusConfig.UserName}:{_asusConfig.Password}";

        if (string.IsNullOrEmpty(_authToken) || !string.Equals(_authCreds, currentCreds, StringComparison.Ordinal))
        {
            _authToken = await GetAuthTokenAsync(cancellationToken).ConfigureAwait(false);
            _authCreds = currentCreds;
        }

        return _authToken;
    }

    protected async Task<JObject> GetHookResponseJsonAsync(string hook, CancellationToken cancellationToken)
    {
        var response = await GetHookResponseAsync(hook, cancellationToken).ConfigureAwait(false);
        var obj = JsonConvert.DeserializeObject<JObject>(response);
        return obj!;
    }

    protected async Task<string> GetHookResponseAsync(string hook, CancellationToken cancellationToken)
    {
        var response = await GetResponseAsync($"/appGet.cgi?hook={hook}()", HttpMethod.Get, null, null, true, cancellationToken).ConfigureAwait(false);
        return response;
    }

    protected async Task<JObject> GetResponseJsonAsync(string path, HttpMethod method, string? jsonContent, bool authenticate, CancellationToken cancellationToken)
    {
        var response = await GetResponseAsync(path, method, jsonContent, null, authenticate, cancellationToken).ConfigureAwait(false);
        var obj = JsonConvert.DeserializeObject<JObject>(response);
        return obj!;
    }

    protected async Task<string> GetResponseAsync(string path, HttpMethod method, string? jsonContent, Dictionary<string, string>? formContent, bool authenticate, CancellationToken cancellationToken)
    {
        var protocol = _asusConfig.UseHttps ? "https" : "http";
        var requestUri = new Uri($"{protocol}://{_asusConfig.HostName}{path}");

        var request = new HttpRequestMessage(method, requestUri);

        request.Headers.UserAgent.TryParseAdd(UserAgent);

        if (authenticate)
        {
            var token = await AuthenticateAsync(cancellationToken).ConfigureAwait(false);
            request.Headers.Add("Cookie", $"asus_token={token}");
        }

        if (!string.IsNullOrEmpty(jsonContent))
            request.Content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        if (formContent != null && formContent.Any())
            request.Content = new FormUrlEncodedContent(formContent);

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (content.IndexOf("error_status", StringComparison.OrdinalIgnoreCase) > -1)
        {
            var errorJson = JsonConvert.DeserializeObject<JObject>(content)!;
            throw new HttpRequestException($"Error {errorJson["error_status"]}", null, HttpStatusCode.BadRequest);
        }

        return content;
    }

    protected static DateTime ParseUnixTime(string unixTimeSeconds)
    {
        if (!long.TryParse(unixTimeSeconds, out var unixTime))
            return DateTime.MinValue;

        var dt = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
        dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        return dt;
    }

    #endregion
}