using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Card_Collection_Tool.Services
{
    public class ScryfallSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private const string LastSyncKey = "LastScryfallSync";

        public ScryfallSyncService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task SyncScryfallDataAsync()
        {
            // Get the last sync time from the database
            var lastSyncSetting = await _context.AppSettings
                .FirstOrDefaultAsync(s => s.Key == LastSyncKey);

            DateTime lastSyncTime;
            if (lastSyncSetting != null)
            {
                lastSyncTime = DateTime.Parse(lastSyncSetting.Value);

                // Check if 24 hours have passed since the last sync
                if ((DateTime.UtcNow - lastSyncTime).TotalHours < 0)
                {
                    Console.WriteLine("Sync skipped. Last sync was less than 24 hours ago.");
                    return; // Exit if it's not time to sync yet
                }
            }

            // Corrected absolute URL for the bulk data endpoint
            var bulkDataUrl = "https://api.scryfall.com/bulk-data";
            
            // Fetch the bulk data endpoint to get the download URI
            var bulkDataResponse = await _httpClient.GetFromJsonAsync<ScryfallBulkDataResponse>(bulkDataUrl);
            
            if (bulkDataResponse != null)
            {
                // Find the "Default Cards" entry and get its download URI
                var defaultCardsData = bulkDataResponse.data.Find(b => b.type == "default_cards");
                
                if (defaultCardsData != null)
                {
                    var downloadUri = defaultCardsData.download_uri;

                    // Ensure the downloadUri is absolute
                    var cardDataResponse = await _httpClient.GetAsync(downloadUri);

                    if (cardDataResponse.IsSuccessStatusCode)
                    {
                        var cardDataJson = await cardDataResponse.Content.ReadAsStringAsync();

                        // Deserialize the JSON data to a list of ScryfallCard objects
                        var cardData = JsonSerializer.Deserialize<List<ScryfallCard>>(cardDataJson);

                        if (cardData != null)
                        {
                            // Clear existing data and replace with new data
                            _context.ScryfallCards.RemoveRange(_context.ScryfallCards);
                            await _context.ScryfallCards.AddRangeAsync(cardData);
                            await _context.SaveChangesAsync();

                            // Update the last sync time
                            if (lastSyncSetting == null)
                            {
                                lastSyncSetting = new AppSettings { Key = LastSyncKey };
                                _context.AppSettings.Add(lastSyncSetting);
                            }

                            lastSyncSetting.Value = DateTime.UtcNow.ToString("o"); // ISO 8601 format
                            await _context.SaveChangesAsync();

                            Console.WriteLine("Data synced successfully.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to download bulk data from {downloadUri}. Status Code: {cardDataResponse.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine("Could not find the 'Default Cards' data in the bulk data response.");
                }
            }
            else
            {
                Console.WriteLine("Failed to fetch bulk data endpoint.");
            }
        }
    }

    // Model for the bulk data response
    public class ScryfallBulkDataResponse
    {
        public List<ScryfallBulkData> data { get; set; }
    }

    public class ScryfallBulkData
    {
        public string type { get; set; }
        public string download_uri { get; set; }
    }
}