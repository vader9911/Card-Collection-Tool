using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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
                if ((DateTime.UtcNow - lastSyncTime).TotalHours < 24)
                {
                    Console.WriteLine("Sync skipped. Last sync was less than 24 hours ago.");
                    return;
                }
            }

            // Perform the sync operation
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

                    // Update the last sync time
                    if (lastSyncSetting == null)
                    {
                        lastSyncSetting = new AppSettings { Key = LastSyncKey };
                        _context.AppSettings.Add(lastSyncSetting);
                    }

                    lastSyncSetting.Value = DateTime.UtcNow.ToString("o"); 
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Data synced successfully.");
                }
            }
        }
    }

    public class ScryfallBulkData
    {
        public string DataUri { get; set; }
    }
}
