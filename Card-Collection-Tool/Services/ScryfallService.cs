using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;


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
                var encodedQuery = Uri.EscapeDataString(query);
                var response = await _httpClient.GetAsync($"https://api.scryfall.com/cards/search?q={encodedQuery}");

                // Check if the response indicates success
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ScryfallApiResponse>();

                // Debugging: Log the response status and the count of returned cards
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                Console.WriteLine($"Number of cards found: {apiResponse?.Data.Count ?? 0}");

                return apiResponse?.Data ?? new List<ScryfallCard>();
            }
            catch (HttpRequestException ex)
            {
                // Log the error to the console for debugging purposes
                Console.WriteLine($"Error fetching data from Scryfall API: {ex.Message}");
                return new List<ScryfallCard>(); // Return an empty list if there's an error
            }
        }
    }

    public class ScryfallApiResponse
    {
        public List<ScryfallCard> Data { get; set; }
    }

    //Define ScryfallCard class model
    public class ScryfallCard
    {
        public string Name { get; set; } // Card name
        public string SetName { get; set; } // Card set name
        public string ImageUri { get; set; } // URL to the card image
        public string ManaCost { get; set; } // Mana cost of the card
        public string TypeLine { get; set; } // Card type
        public string OracleText { get; set; } // Card description or rules text
    }

}
