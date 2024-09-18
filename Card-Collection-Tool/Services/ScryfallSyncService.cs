using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using System.Reflection;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Card_Collection_Tool.Services
{
    public class ScryfallSyncService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ScryfallSyncService> _logger;
        private readonly HttpClient _httpClient;
        private DateTime? _lastSync;

        public ScryfallSyncService(ApplicationDbContext context, ILogger<ScryfallSyncService> logger, HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
            _lastSync = null;
        }

        public async Task SyncScryfallDataAsync()
        {
            _logger.LogInformation("Running data sync");

            try
            {
                if (_lastSync.HasValue && _lastSync.Value.AddHours(24) > DateTime.UtcNow)
                {
                    _logger.LogInformation("Sync already completed within the last 24 hours. Skipping this run.");
                    return;
                }

                _logger.LogInformation("Starting Scryfall data synchronization.");

                // Fetch the bulk data URI for card data
                string downloadUri = await FetchBulkDataUriAsync();
                if (string.IsNullOrEmpty(downloadUri))
                {
                    _logger.LogError("Failed to fetch card data URI from Scryfall.");
                    return;
                }

                using var response = await _httpClient.GetAsync(downloadUri, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to download card data from Scryfall API. Status Code: {StatusCode}, Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return;
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);

                _logger.LogInformation("Processing card data stream from Scryfall API.");

                var serializer = new JsonSerializer();
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Server=DESKTOP-O35BQH4\\SQLEXPRESS;Database=Card-Collecting-Tool;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True") // Replace with your actual connection string
                    .Options;

                var cards = new ConcurrentBag<ScryfallCard>(); // Use a thread-safe collection to store cards

                int cardCount = 0;
                var stopwatch = Stopwatch.StartNew();

                // Read and deserialize the JSON data asynchronously
                while (await jsonReader.ReadAsync())
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        var card = serializer.Deserialize<ScryfallCard>(jsonReader);
                        if (card != null)
                        {
                            cards.Add(card); // Add the card to the concurrent bag
                        }
                    }
                }

                // Process cards in parallel
                await Parallel.ForEachAsync(cards, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (card, token) =>
                {
                    using var context = new ApplicationDbContext(options); // Each task needs its own DbContext instance
                    await ProcessCardAsync(context, card);
                    Interlocked.Increment(ref cardCount); // Safely increment the card count

                    // Log progress every 1000 cards
                    if (cardCount % 1000 == 0)
                    {
                        var elapsed = stopwatch.Elapsed;
                        _logger.LogInformation($"----------Processed {cardCount} cards so far. Time elapsed: {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds.-------------");
                    }
                });

                _lastSync = DateTime.UtcNow;
                _logger.LogInformation("Scryfall data synchronization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during Scryfall data synchronization: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private async Task<string> FetchBulkDataUriAsync()
        {
            try
            {
                _logger.LogInformation("Fetching bulk data metadata from Scryfall API...");

                var response = await _httpClient.GetAsync("https://api.scryfall.com/bulk-data");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch bulk data metadata from Scryfall API. Status Code: {StatusCode}, Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received bulk data metadata from Scryfall API.");
                _logger.LogInformation(content);
                var bulkDataResponse = JsonConvert.DeserializeObject<ScryfallBulkDataResponse>(content);

                // Find the entry for the "default_cards" type
                var defaultCardsEntry = bulkDataResponse?.Data?.FirstOrDefault(entry => entry.Type == "default_cards");
                //var defaultCardsEntryUri = defaultCardsEntry.DownloadUri

                if (defaultCardsEntry == null)
                {
                    _logger.LogError("No 'default_cards' entry found in Scryfall bulk data.");
                    return null;
                }

                _logger.LogInformation("Download URI for default cards: {DownloadUri}", defaultCardsEntry.DownloadUri);
                return defaultCardsEntry.DownloadUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching bulk data URI from Scryfall API.");
                return null;
            }
        }

        private async Task ProcessCardAsync(ApplicationDbContext context, ScryfallCard card)
        {
            //_logger.LogInformation("Processing card data...");

            var existingCard = await context.ScryfallCards
                                            .Include(c => c.Prices)
                                            .Include(c => c.ImageUris)
                                            .Include(c => c.Legalities)
                                            .FirstOrDefaultAsync(c => c.Id == card.Id);

            if (existingCard == null)
            {
                if (card.Legalities != null)
                {
                    card.Legalities.ScryfallCardId = card.Id;
                }

                if (card.Prices != null)
                {
                    card.Prices.ScryfallCardId = card.Id;
                }

                if (card.ImageUris != null)
                {
                    card.ImageUris.ScryfallCardId = card.Id;
                }

                await context.ScryfallCards.AddAsync(card);
            }
            else
            {
                // Existing card - update its data

                // Update the card's properties
                existingCard.Name = card.Name;
                existingCard.Cmc = card.Cmc;
                existingCard.ColorIdentity = card.ColorIdentity;
                existingCard.Colors = card.Colors;
                existingCard.Keywords = card.Keywords;
                existingCard.ManaCost = card.ManaCost;
                existingCard.Power = card.Power;
                existingCard.Toughness = card.Toughness;
                existingCard.TypeLine = card.TypeLine;
                existingCard.Artist = card.Artist;
                existingCard.CollectorNumber = card.CollectorNumber;
                existingCard.Digital = card.Digital;
                existingCard.FlavorText = card.FlavorText;
                existingCard.OracleText = card.OracleText;
                existingCard.FullArt = card.FullArt;
                existingCard.Games = card.Games;
                existingCard.Rarity = card.Rarity;
                existingCard.ReleaseDate = card.ReleaseDate;
                existingCard.Reprint = card.Reprint;
                existingCard.SetName = card.SetName;
                existingCard.Set = card.Set;
                existingCard.SetId = card.SetId;
                existingCard.Variation = card.Variation;
                existingCard.VariationOf = card.VariationOf;
                existingCard.Legalities = card.Legalities;
                existingCard.Prices = card.Prices;
                existingCard.ImageUris = card.ImageUris;


                // Update Prices
                if (card.Prices != null)
                {
                    existingCard.Prices = card.Prices;
                    existingCard.Prices.ScryfallCardId = card.Id;
                }

                // Update Image URIs
                if (card.ImageUris != null)
                {
                    existingCard.ImageUris = card.ImageUris;
                    existingCard.ImageUris.ScryfallCardId = card.Id;
                }

                // Update Legalities
                if (card.Legalities != null)
                {
                    existingCard.Legalities = card.Legalities;
                    existingCard.Legalities.ScryfallCardId = card.Id;
                }

                // Update the existing card in the context
                context.ScryfallCards.Update(existingCard);
            }

            await context.SaveChangesAsync();
        }
    }
}


        public class ScryfallBulkDataResponse
{
    public string Object { get; set; }
    public bool HasMore { get; set; }
    public List<ScryfallBulkDataEntry> Data { get; set; }
}

public class ScryfallBulkDataEntry
{
    public string Object { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("download_uri")]
    public string DownloadUri { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public long Size { get; set; }

    //[JsonProperty("uri")]
    //public string Uri { get; set; }
    public string ContentType { get; set; }
    public string ContentEncoding { get; set; }
}

public class DefualtCards
{

}