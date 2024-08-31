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
            // Use the Scryfall API to search for cards based on the query
            var response = await _httpClient.GetFromJsonAsync<ScryfallApiResponse>($"https://api.scryfall.com/cards/search?q={query}");

            return response?.Data ?? new List<ScryfallCard>();
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
