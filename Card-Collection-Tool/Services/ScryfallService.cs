using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using Card_Collection_Tool.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Card_Collection_Tool.Data;


namespace Card_Collection_Tool.Services
{
    public class ScryfallService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ScryfallService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<ScryfallCard> GetCardByIdAsync(string cardId)
        {
            var card = new ScryfallCard();

            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "SELECT * FROM ScryFallCards WHERE CardId = @CardId";

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@CardId", cardId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Map fields from the database to the Card object
                            card.Id = reader.GetString(reader.GetOrdinal("CardId"));
                            // Map other fields as needed
                        }
                    }
                }
            }

            return card;
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

}