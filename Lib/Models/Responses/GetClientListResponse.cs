using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PixelByProxy.Asus.Router.Models.Responses
{
    internal class GetClientListResponse
    {
        public string? ClientApiLevel { get; set; }

        public List<string>? MacList { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken>? ClientsJson { get; set; }
    }
}
