using Newtonsoft.Json;

namespace ITMSales.Shared.Responses
{
	public class CityResponse
	{
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}

