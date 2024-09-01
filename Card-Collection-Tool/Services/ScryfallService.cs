using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Card_Collection_Tool.Services
{
    public class ScryfallService
    {
        private readonly HttpClient _httpClient;

        public ScryfallService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ScryfallCard>> SearchCardsAsync(string query)
        {
            try
            {
                var encodedQuery = query.Replace(" ", "+"); // Less aggressive encoding for search queries
                Console.WriteLine($"Original Query: {query}");
                Console.WriteLine($"Encoded Query: {encodedQuery}");

                var response = await _httpClient.GetAsync($"https://api.scryfall.com/cards/search?q={encodedQuery}");

                response.EnsureSuccessStatusCode();

                // Log the raw JSON response to check the structure
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                Console.WriteLine($"Raw Response Content: {responseContent}");

                var apiResponse = await response.Content.ReadFromJsonAsync<ScryfallApiResponse>();

                if (apiResponse != null && apiResponse.Data != null)
                {
                    Console.WriteLine($"Number of cards found: {apiResponse.Data.Count}");
                    return apiResponse.Data;
                }
                else
                {
                    Console.WriteLine("No data returned from the Scryfall API.");
                    return new List<ScryfallCard>();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching data from Scryfall API: {ex.Message}");
                return new List<ScryfallCard>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                return new List<ScryfallCard>();
            }
        }
    }

        public class ScryfallApiResponse
{
    [JsonPropertyName("data")]
    public List<ScryfallCard> Data { get; set; }
}

public class ScryfallCard
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("oracle_text")]
    public string OracleText { get; set; }

    [JsonPropertyName("set_name")]
    public string SetName { get; set; }

    [JsonPropertyName("image_uris")]
    public ImageUris ImageUris { get; set; }
}

public class ImageUris
{
    [JsonPropertyName("small")]
    public string Small { get; set; }

    [JsonPropertyName("normal")]
    public string Normal { get; set; }

    [JsonPropertyName("large")]
    public string Large { get; set; }

    [JsonPropertyName("png")]
    public string Png { get; set; }
    }

}
