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
using Microsoft.Data.SqlClient;
using System.Data;

namespace Card_Collection_Tool.Services
{
    public class ScryfallSyncService
    {
        private readonly ILogger<ScryfallSyncService> _logger;
        private readonly HttpClient _httpClient;
        private DateTime? _lastSync;
        private readonly string _connectionString;

        public ScryfallSyncService(ILogger<ScryfallSyncService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                    await ProcessCardAsync(card);
                    Interlocked.Increment(ref cardCount); // Safely increment the card count

                    // Log progress every 1000 cards
                    if (cardCount % 1000 == 0)
                    {
                        var elapsed = stopwatch.Elapsed;
                        _logger.LogInformation($"----------Processed {cardCount} cards so far. Time elapsed: {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds.-------------");
                    }
                });

                // Sync symbols table
                await FetchAndStoreSymbolsAsync();

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

        private async Task ProcessCardAsync(ScryfallCard card)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Convert lists to comma-separated strings
                string games = card.Games != null ? string.Join(",", card.Games) : null;
                string colorIdentity = card.ColorIdentity != null ? string.Join(",", card.ColorIdentity) : null;
                string colors = card.Colors != null ? string.Join(",", card.Colors) : null;
                string keywords = card.Keywords != null ? string.Join(",", card.Keywords) : null;

                // Upsert the main card data using the UpsertScryfallCard stored procedure
                var upsertCardCmd = new SqlCommand("dbo.UpsertScryfallCard", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                upsertCardCmd.Parameters.AddWithValue("@Id", card.Id);
                upsertCardCmd.Parameters.AddWithValue("@Name", (object)card.Name ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Cmc", (object)card.Cmc ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@ColorIdentity", (object)colorIdentity ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Colors", (object)colors ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Keywords", (object)keywords ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@ManaCost", (object)card.ManaCost ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Power", (object)card.Power ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Toughness", (object)card.Toughness ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@TypeLine", (object)card.TypeLine ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Artist", (object)card.Artist ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@CollectorNumber", (object)card.CollectorNumber ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Digital", (object)card.Digital ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@FlavorText", (object)card.FlavorText ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@OracleText", (object)card.OracleText ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@FullArt", (object)card.FullArt ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Games", (object)games ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Rarity", (object)card.Rarity ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@ReleaseDate", (object)card.ReleaseDate ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Reprint", (object)card.Reprint ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@SetName", (object)card.SetName ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Set", (object)card.Set ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@SetId", (object)card.SetId ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@Variation", (object)card.Variation ?? DBNull.Value);
                upsertCardCmd.Parameters.AddWithValue("@VariationOf", (object)card.VariationOf ?? DBNull.Value);
                await upsertCardCmd.ExecuteNonQueryAsync();

                // Upsert the price data using the UpsertPrice stored procedure
                var upsertPriceCmd = new SqlCommand("dbo.UpsertPrice", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                upsertPriceCmd.Parameters.AddWithValue("@ScryfallCardId", card.Id);
                upsertPriceCmd.Parameters.AddWithValue("@Usd", (object)card.Prices?.Usd ?? DBNull.Value);
                upsertPriceCmd.Parameters.AddWithValue("@UsdFoil", (object)card.Prices?.Usd_Foil ?? DBNull.Value);
                upsertPriceCmd.Parameters.AddWithValue("@UsdEtched", (object)card.Prices?.Usd_Etched ?? DBNull.Value);
                upsertPriceCmd.Parameters.AddWithValue("@Eur", (object)card.Prices?.Eur ?? DBNull.Value);
                upsertPriceCmd.Parameters.AddWithValue("@EurFoil", (object)card.Prices?.Eur_Foil ?? DBNull.Value);
                upsertPriceCmd.Parameters.AddWithValue("@Tix", (object)card.Prices?.Tix ?? DBNull.Value);
                await upsertPriceCmd.ExecuteNonQueryAsync();

                // Upsert the image URIs using the UpsertImageUri stored procedure
                var upsertImageCmd = new SqlCommand("dbo.UpsertImageUri", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                upsertImageCmd.Parameters.AddWithValue("@ScryfallCardId", card.Id);
                upsertImageCmd.Parameters.AddWithValue("@Small", (object)card.ImageUris?.Small ?? DBNull.Value);
                upsertImageCmd.Parameters.AddWithValue("@Normal", (object)card.ImageUris?.Normal ?? DBNull.Value);
                upsertImageCmd.Parameters.AddWithValue("@Large", (object)card.ImageUris?.Large ?? DBNull.Value);
                upsertImageCmd.Parameters.AddWithValue("@Png", (object)card.ImageUris?.Png ?? DBNull.Value);
                upsertImageCmd.Parameters.AddWithValue("@ArtCrop", (object)card.ImageUris?.ArtCrop ?? DBNull.Value);
                upsertImageCmd.Parameters.AddWithValue("@BorderCrop", (object)card.ImageUris?.BorderCrop ?? DBNull.Value);
                await upsertImageCmd.ExecuteNonQueryAsync();

                // Upsert the legalities using the UpsertLegalities stored procedure
                var upsertLegalitiesCmd = new SqlCommand("dbo.UpsertLegalities", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                upsertLegalitiesCmd.Parameters.AddWithValue("@ScryfallCardId", card.Id);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Standard", (object)card.Legalities?.Standard ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Future", (object)card.Legalities?.Future ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Historic", (object)card.Legalities?.Historic ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Timeless", (object)card.Legalities?.Timeless ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Gladiator", (object)card.Legalities?.Gladiator ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Pioneer", (object)card.Legalities?.Pioneer ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Explorer", (object)card.Legalities?.Explorer ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Modern", (object)card.Legalities?.Modern ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Legacy", (object)card.Legalities?.Legacy ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Pauper", (object)card.Legalities?.Pauper ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Vintage", (object)card.Legalities?.Vintage ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Penny", (object)card.Legalities?.Penny ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Commander", (object)card.Legalities?.Commander ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Oathbreaker", (object)card.Legalities?.Oathbreaker ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@StandardBrawl", (object)card.Legalities?.StandardBrawl ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Brawl", (object)card.Legalities?.Brawl ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Alchemy", (object)card.Legalities?.Alchemy ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@PauperCommander", (object)card.Legalities?.PauperCommander ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Duel", (object)card.Legalities?.Duel ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@OldSchool", (object)card.Legalities?.OldSchool ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Premodern", (object)card.Legalities?.Premodern ?? DBNull.Value);
                upsertLegalitiesCmd.Parameters.AddWithValue("@Predh", (object)card.Legalities?.Predh ?? DBNull.Value);
                await upsertLegalitiesCmd.ExecuteNonQueryAsync();
            }
        }

        public async Task FetchAndStoreSymbolsAsync()
        {
            var response = await _httpClient.GetAsync("https://api.scryfall.com/symbology");
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var symbolsData = JsonConvert.DeserializeObject<ScryfallSymbolsResponse>(content);

                    if (symbolsData != null && symbolsData.Data.Any())
                    {
                        foreach (var symbol in symbolsData.Data)
                        {
                            await SaveSymbolToDatabase(symbol);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch symbols: {response.StatusCode}");
                }
            }
        }

        private async Task SaveSymbolToDatabase(ScryfallSymbol symbol)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
            IF NOT EXISTS (SELECT 1 FROM CardSymbols WHERE Symbol = @Symbol)
            BEGIN
                INSERT INTO CardSymbols (Symbol, EnglishDescription, SvgUri)
                VALUES (@Symbol, @English, @SvgUri)
            END
            ELSE
            BEGIN
                UPDATE CardSymbols
                SET EnglishDescription = @English, SvgUri = @SvgUri
                WHERE Symbol = @Symbol
            END";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Symbol", symbol.Symbol);
                    command.Parameters.AddWithValue("@English", symbol.English);
                    command.Parameters.AddWithValue("@SvgUri", symbol.Svg_Uri);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }



        public class ScryfallSymbolsResponse
        {
            public List<ScryfallSymbol> Data { get; set; }
        }

        public class ScryfallSymbol
        {
            public string Symbol { get; set; }
            public string English { get; set; }
            public string Svg_Uri { get; set; }
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

    }
}
    