using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace Card_Collection_Tool.Services
{
    public class ScryfallService
    {
        private readonly HttpClient _httpClient;

        public ScryfallService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetAutocompleteResultsAsync(string query)
        {

            var response = await _httpClient.GetAsync($"https://api.scryfall.com/cards/autocomplete?q={query}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var autocompleteResponse = JsonSerializer.Deserialize<AutocompleteResponse>(jsonResponse);

            return autocompleteResponse?.data ?? new List<string>();
        }

        public class AutocompleteResponse
        {
            public List<string>? data { get; set; }
        }
    }




    public class ScryfallCardData
    { 
        public string? Id { get; set; }
    
        public string? Name { get; set; }


        public string? OracleText { get; set; }


        public string? SetName { get; set; }


        public ImageUris ImageUris { get; set; }
    }

    public class ImageUris
    {

        public string? Small { get; set; }


        public string? Normal { get; set; }

        public string? Large { get; set; }

        public string? Png { get; set; }


    }


    public class AutocompleteResponse
    {
        public List<string> Data { get; set; }
    }

}