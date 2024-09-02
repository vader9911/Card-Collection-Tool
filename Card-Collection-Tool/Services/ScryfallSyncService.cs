using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Card_Collection_Tool.Services
{
    public class ScryfallSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public ScryfallSyncService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task SyncScryfallDataAsync()
        {
            // URL to the Scryfall bulk data
            var bulkDataUrl = "https://api.scryfall.com/bulk-data/default-cards";

            var response = await _httpClient.GetFromJsonAsync<ScryfallBulkData>(bulkDataUrl);

            if (response != null && response.DataUri != null)
            {
                var cardData = await _httpClient.GetFromJsonAsync<List<ScryfallCard>>(response.DataUri);

                if (cardData != null)
                {
                    // Clear existing data and replace with new data
                    _context.ScryfallCards.RemoveRange(_context.ScryfallCards);
                    await _context.ScryfallCards.AddRangeAsync(cardData);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    public class ScryfallBulkData
    {
        public string DataUri { get; set; }
    }
}
