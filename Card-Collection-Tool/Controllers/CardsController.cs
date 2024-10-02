using System.Data;
using Microsoft.AspNetCore.Mvc;
using Card_Collection_Tool.Models;
using Card_Collection_Tool.Data;
using Card_Collection_Tool.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Card_Collection_Tool.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ScryfallService _scryfallService;

        public CardsController(ApplicationDbContext context, ScryfallService scryfallService)
        {
            _context = context;
            _scryfallService = scryfallService;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchCards(
     [FromQuery] string name = null,
     [FromQuery] string set = null,
     [FromQuery] string oracleText = null,
     [FromQuery] string type = null,
     [FromQuery] string colors = null,
     [FromQuery] string colorCriteria = "exact",
     [FromQuery] string colorIdentity = null,
     [FromQuery] string colorIdentityCriteria = "exact",
     [FromQuery] float? manaValue = null,
     [FromQuery] string manaValueComparator = "equals",
     [FromQuery] string manaCost = null,
     [FromQuery] string power = null,
     [FromQuery] string powerComparator = "equals",
     [FromQuery] string toughness = null,
     [FromQuery] string toughnessComparator = "equals",
     [FromQuery] string loyalty = null,
     [FromQuery] string loyaltyComparator = "equals",
     [FromQuery] string sortOrder = "name",
     [FromQuery] string sortDirection = "asc",
     [FromQuery] bool showAllVersions = false)
        {
            // Define the parameters to pass to the stored procedure
            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@name", name ?? (object)DBNull.Value),
        new SqlParameter("@set", set ?? (object)DBNull.Value),
        new SqlParameter("@oracleText", oracleText ?? (object)DBNull.Value),
        new SqlParameter("@type", type ?? (object)DBNull.Value),
        new SqlParameter("@colors", colors ?? (object)DBNull.Value),
        new SqlParameter("@colorCriteria", colorCriteria ?? (object)DBNull.Value),
        new SqlParameter("@colorIdentity", colorIdentity ?? (object)DBNull.Value),
        new SqlParameter("@colorIdentityCriteria", colorIdentityCriteria ?? (object)DBNull.Value),
        new SqlParameter("@manaValue", manaValue ?? (object)DBNull.Value),
        new SqlParameter("@manaValueComparator", manaValueComparator ?? (object)DBNull.Value),
        new SqlParameter("@power", power ?? (object)DBNull.Value),
        new SqlParameter("@powerComparator", powerComparator ?? (object)DBNull.Value),
        new SqlParameter("@toughness", toughness ?? (object)DBNull.Value),
        new SqlParameter("@toughnessComparator", toughnessComparator ?? (object)DBNull.Value),
        new SqlParameter("@loyalty", loyalty ?? (object)DBNull.Value),
        new SqlParameter("@loyaltyComparator", loyaltyComparator ?? (object)DBNull.Value),
        new SqlParameter("@sortOrder", sortOrder ?? (object)DBNull.Value),
        new SqlParameter("@sortDirection", sortDirection ?? (object)DBNull.Value)
    };

            // Execute the stored procedure
            var results = new List<ScryfallCard>();
            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (var command = new SqlCommand("SearchCards", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());
                    connection.Open();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Use the helper method to create a ScryfallCard object
                            results.Add(CreateScryfallCard(reader));
                        }
                    }
                }
            }

            // Optionally filter the most recent versions if applicable
            if (!showAllVersions)
            {
                results = FilterMostRecentNonDigitalVersion(results);
            }

            return Ok(results);
        }


        // Function to filter the most recent version of each card based on release date
        private List<ScryfallCard?> FilterMostRecentVersions(List<ScryfallCard> cards)
        {
            return cards
                .GroupBy(c => c.Name)
                .Select(g => g.OrderByDescending(c => c.ReleaseDate).FirstOrDefault())
                .ToList();
        }

        private List<ScryfallCard> FilterMostRecentNonDigitalVersion(List<ScryfallCard> cards)
        {
            var filteredCards = new List<ScryfallCard>();

            // Group cards by name to handle versions
            var groupedCards = cards.GroupBy(c => c.Name);

            foreach (var group in groupedCards)
            {
                // Try to find the first non-digital card in the group
                var nonDigitalCard = group.FirstOrDefault(c => !c.Digital);

                if (nonDigitalCard != null)
                {
                    filteredCards.Add(nonDigitalCard);
                }
                else
                {
                    // If no non-digital cards are found, fall back to the first card in the group
                    filteredCards.Add(group.First());
                }
            }

            return filteredCards;
        }


        [HttpGet("autocomplete/{field}")]
        public async Task<IActionResult> GetAutocompleteOptions(string field, [FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Query cannot be empty.");
            }

            List<string> options;
            try
            {
                options = await GetAutocompleteOptionsFromDatabaseAsync(field, searchTerm);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(options);
        }

        private async Task<List<string>> GetAutocompleteOptionsFromDatabaseAsync(string field, string searchTerm)
        {
            string sqlQuery;
            switch (field.ToLower())
            {
                case "type":
                    sqlQuery = "SELECT DISTINCT TypeLine FROM ScryfallCards WHERE TypeLine LIKE @Query ORDER BY TypeLine";
                    break;

                case "set":
                    sqlQuery = "SELECT DISTINCT SetName FROM ScryfallCards WHERE SetName LIKE @Query ORDER BY SetName";
                    break;

                case "oracletext":
                    sqlQuery = "SELECT DISTINCT OracleText FROM ScryfallCards WHERE OracleText LIKE @Query ORDER BY OracleText";
                    break;

                // Add more cases for other fields as needed...

                default:
                    throw new ArgumentException($"Field type '{field}' is not supported for autocomplete.");
            }

            List<string> results = new List<string>();

            // Use ADO.NET to execute the query
            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Query", $"%{searchTerm}%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return results.Take(10).ToList(); // Limit the number of results for performance reasons
        }


        [HttpGet("{selectedCardId}/details")]
        public async Task<IActionResult> GetCardById(string selectedCardId)
        {
            Console.WriteLine("API called with Card ID: " + selectedCardId);

            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Database connection opened successfully.");

                    var sqlQuery = @"
                        SELECT *
                        FROM v_CardData
                        WHERE Id IN (@CardId)";

                    Console.WriteLine("Executing SQL Query: " + sqlQuery);

                    ScryfallCard card = null;

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CardId", selectedCardId);
                        Console.WriteLine("Card ID Parameter Set: " + selectedCardId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("No rows returned from the query.");
                                return NotFound("No card details found for the provided ID.");
                            }

                            Console.WriteLine("Rows returned from the query, processing results...");

                            if (await reader.ReadAsync())
                            {
                                var cardId = reader["Id"].ToString();

                                card = new ScryfallCard
                                {
                                    Id = cardId,
                                    Name = reader["Name"].ToString(),
                                    Cmc = reader["Cmc"] != DBNull.Value ? Convert.ToSingle(reader["Cmc"]) : (float?)null,
                                    ManaCost = reader["ManaCost"].ToString(),
                                    TypeLine = reader["TypeLine"].ToString(),
                                    OracleText = reader["OracleText"].ToString(),
                                    Power = reader["Power"].ToString(),
                                    Toughness = reader["Toughness"].ToString(),
                                    Rarity = reader["Rarity"].ToString(),
                                    Digital = Convert.ToBoolean(reader["Digital"]),
                                    Artist = reader["Artist"].ToString(),
                                    CollectorNumber = reader["CollectorNumber"].ToString(),
                                    FlavorText = reader["FlavorText"].ToString(),
                                    FullArt = reader["FullArt"] != DBNull.Value ? Convert.ToBoolean(reader["FullArt"]) : (bool?)null,
                                    Games = reader["Games"] != DBNull.Value ? reader["Games"].ToString().Split(',').ToList() : new List<string?>(),
                                    ReleaseDate = reader["ReleaseDate"].ToString(),
                                    Reprint = reader["Reprint"] != DBNull.Value ? Convert.ToBoolean(reader["Reprint"]) : (bool?)null,
                                    SetName = reader["SetName"].ToString(),
                                    Set = reader["Set"].ToString(),
                                    SetId = reader["SetId"].ToString(),
                                    Variation = reader["Variation"] != DBNull.Value ? Convert.ToBoolean(reader["Variation"]) : false,
                                    VariationOf = reader["VariationOf"].ToString(),
                                    Colors = reader["Colors"] != DBNull.Value ? reader["Colors"].ToString().Split(',').ToList() : new List<string?>(),
                                    ColorIdentity = reader["ColorIdentity"] != DBNull.Value ? reader["ColorIdentity"].ToString().Split(',').ToList() : new List<string?>(),
                                    Keywords = reader["Keywords"] != DBNull.Value ? reader["Keywords"].ToString().Split(',').ToList() : new List<string?>(),

                                    Prices = new Prices
                                    {
                                        ScryfallCardId = cardId,
                                        Usd = reader["Usd"]?.ToString(),
                                        Usd_Foil = reader["UsdFoil"]?.ToString(),
                                        Usd_Etched = reader["UsdEtched"]?.ToString(),
                                        Eur = reader["Eur"]?.ToString(),
                                        Eur_Foil = reader["EurFoil"]?.ToString(),
                                        Tix = reader["Tix"]?.ToString()
                                    },
                                    ImageUris = new ImageUris
                                    {
                                        ScryfallCardId = cardId,
                                        Small = reader["Small"]?.ToString(),
                                        Normal = reader["Normal"]?.ToString(),
                                        Large = reader["Large"]?.ToString(),
                                        Png = reader["Png"]?.ToString(),
                                        ArtCrop = reader["ArtCrop"]?.ToString(),
                                        BorderCrop = reader["BorderCrop"]?.ToString()
                                    },
                                    Legalities = new Legalities
                                    {
                                        ScryfallCardId = cardId,
                                        Standard = reader["Standard"]?.ToString(),
                                        Future = reader["Future"]?.ToString(),
                                        Historic = reader["Historic"]?.ToString(),
                                        Timeless = reader["Timeless"]?.ToString(),
                                        Gladiator = reader["Gladiator"]?.ToString(),
                                        Pioneer = reader["Pioneer"]?.ToString(),
                                        Explorer = reader["Explorer"]?.ToString(),
                                        Modern = reader["Modern"]?.ToString(),
                                        Legacy = reader["Legacy"]?.ToString(),
                                        Pauper = reader["Pauper"]?.ToString(),
                                        Vintage = reader["Vintage"]?.ToString(),
                                        Penny = reader["Penny"]?.ToString(),
                                        Commander = reader["Commander"]?.ToString(),
                                        Oathbreaker = reader["Oathbreaker"]?.ToString(),
                                        StandardBrawl = reader["StandardBrawl"]?.ToString(),
                                        Brawl = reader["Brawl"]?.ToString(),
                                        Alchemy = reader["Alchemy"]?.ToString(),
                                        PauperCommander = reader["PauperCommander"]?.ToString(),
                                        Duel = reader["Duel"]?.ToString(),
                                        OldSchool = reader["OldSchool"]?.ToString(),
                                        Premodern = reader["Premodern"]?.ToString(),
                                        Predh = reader["Predh"]?.ToString()
                                    }
                                };

                                Console.WriteLine($"Processed Card: {card.Name} with ID: {card.Id}");
                            }
                        }
                    }

                    if (card == null)
                    {
                        Console.WriteLine("No card was found.");
                        return NotFound();
                    }

                    Console.WriteLine($"Returning card: {card.Name} with ID: {card.Id}");
                    return Ok(card);
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Error fetching card details: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{cardName}/variations")]
        public async Task<IActionResult> GetCardVariations(string cardName)
        {
            Console.WriteLine($"API called to fetch variations for card: {cardName}");

            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Database connection opened successfully.");

                    // SQL query to fetch cards with matching names
                    var sqlQuery = @"
                SELECT 
                    Id, Name, SetName, ReleaseDate, Digital, Usd, UsdFoil, UsdEtched
                FROM 
                    dbo.v_CardData
                WHERE 
                    Name = @CardName
                    AND Digital = 0" ;

                    Console.WriteLine("Executing SQL Query: " + sqlQuery);

                    var variations = new List<object>();

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CardName", cardName);
                        Console.WriteLine("Card Name Parameter Set: " + cardName);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("No variations found for the card.");
                                return NotFound("No variations found for the provided card name.");
                            }

                            Console.WriteLine("Rows returned from the query, processing results...");

                            while (await reader.ReadAsync())
                            {
                                var card = new
                                {
                                    Id = reader["Id"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    ReleaseDate = reader["ReleaseDate"].ToString(),
                                    SetName = reader["SetName"].ToString(),
                                    Digital = reader["Digital"].ToString(),
                                    Usd = reader["Usd"].ToString(),
                                    UsdFoil = reader["UsdFoil"].ToString(),
                                    UsdEtched = reader["UsdEtched"].ToString()
                                };

                                variations.Add(card);
                                Console.WriteLine($"Found Variation: {card.Name} with ID: {card.Id}, set name: {card.SetName}, release date: {card.ReleaseDate}");
                            }
                        }
                    }

                    if (variations.Count == 0)
                    {
                        Console.WriteLine("No card variations were added to the results list.");
                        return NotFound();
                    }

                    Console.WriteLine($"Returning {variations.Count} card variation(s).");
                    return Ok(variations);
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Error fetching card variations: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("details")]
        public async Task<IActionResult> GetCardsByIds([FromBody] List<string>? cardIds)
        {
            Console.WriteLine("API called with Card IDs: " + string.Join(", ", cardIds));

            if (cardIds == null || cardIds.Count == 0)
            {
                return BadRequest("No card IDs provided.");
            }

            try
            {
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Database connection opened successfully.");

                    // SQL query to select card data using the view v_CardData
                    var sqlQuery = @"
                SELECT * 
                FROM v_CardData 
                WHERE Id IN (" + string.Join(", ", cardIds.Select((_, index) => $"@CardId{index}")) + ")";

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        // Add each card ID as a parameter to the SQL command
                        for (int i = 0; i < cardIds.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@CardId{i}", cardIds[i]);
                        }

                        var cards = new List<ScryfallCard>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cards.Add(CreateScryfallCard(reader));
                            }
                        }

                        Console.WriteLine($"Processed {cards.Count} cards.");
                        return Ok(cards);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Error fetching card details: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }









        //[HttpGet("autocomplete")]
        //public async Task<IActionResult> GetCardAutocomplete([FromQuery] string query)
        //{
        //    if (string.IsNullOrEmpty(query))
        //    {
        //        return BadRequest("Search cannot be empty.");
        //    }

        //    var matchingCards = await _context.ScryfallCards
        //        .Where(card => card.Name.Contains(query))
        //        .OrderBy(card => card.Name)
        //        .Select(card => new
        //        {
        //            id = card.Id,
        //            name = card.Name,
        //            imageUri = card.ImageUris.Png ?? card.ImageUris.Large ?? card.ImageUris.Normal ?? "default-image-url.png"
        //        })
        //        .Take(20)
        //        .Distinct()
        //        .ToListAsync();



        //    return Ok(matchingCards);
        //}


        //        // GET: api/cards/{cardId}
        //        [HttpGet("{cardId}/details")]
        //        public async Task<IActionResult> GetCardDetails(string cardId)
        //        {
        //            Console.WriteLine($"Received request for card ID: {cardId}");
        //            if (string.IsNullOrEmpty(cardId))
        //            {
        //                return BadRequest(new { message = "Card ID cannot be null or empty." });
        //            }

        //            var card = await _context.ScryfallCards
        //                .Include(c => c.ImageUris) // Include related data if necessary
        //                .Where(c => c.Id == cardId)
        //                .Select(card => new
        //                    {
        //                        card.Id,
        //                        card.Name,
        //                        ImageUri = card.ImageUris.Normal,
        //                        card.ManaCost,
        //                        card.TypeLine,
        //                        card.OracleText,
        //                        card.SetName,
        //                        card.Artist,
        //                        card.Rarity,
        //                        Prices = card.Prices.Usd
        //                    })
        //                .FirstOrDefaultAsync();

        //            if (card == null)
        //            {
        //                return NotFound(new { message = "Card not found." });
        //            }


        //            return Ok(card);
        //        }
        //    }



        private ScryfallCard CreateScryfallCard(SqlDataReader reader)
        {
            var cardId = reader["Id"].ToString();

            return new ScryfallCard
            {
                Id = cardId,
                Name = reader["Name"].ToString(),
                Cmc = reader["Cmc"] != DBNull.Value ? Convert.ToSingle(reader["Cmc"]) : (float?)null,
                ManaCost = reader["ManaCost"].ToString(),
                TypeLine = reader["TypeLine"].ToString(),
                OracleText = reader["OracleText"].ToString(),
                Power = reader["Power"].ToString(),
                Toughness = reader["Toughness"].ToString(),
                Rarity = reader["Rarity"].ToString(),
                Digital = Convert.ToBoolean(reader["Digital"]),
                Artist = reader["Artist"].ToString(),
                CollectorNumber = reader["CollectorNumber"].ToString(),
                FlavorText = reader["FlavorText"].ToString(),
                FullArt = reader["FullArt"] != DBNull.Value ? Convert.ToBoolean(reader["FullArt"]) : (bool?)null,
                Games = reader["Games"] != DBNull.Value ? reader["Games"].ToString().Split(',').ToList() : new List<string?>(),
                ReleaseDate = reader["ReleaseDate"].ToString(),
                Reprint = reader["Reprint"] != DBNull.Value ? Convert.ToBoolean(reader["Reprint"]) : (bool?)null,
                SetName = reader["SetName"].ToString(),
                Set = reader["Set"].ToString(),
                SetId = reader["SetId"].ToString(),
                Variation = reader["Variation"] != DBNull.Value ? Convert.ToBoolean(reader["Variation"]) : false,
                VariationOf = reader["VariationOf"].ToString(),
                Colors = reader["Colors"] != DBNull.Value ? reader["Colors"].ToString().Split(',').ToList() : new List<string?>(),
                ColorIdentity = reader["ColorIdentity"] != DBNull.Value ? reader["ColorIdentity"].ToString().Split(',').ToList() : new List<string?>(),
                Keywords = reader["Keywords"] != DBNull.Value ? reader["Keywords"].ToString().Split(',').ToList() : new List<string?>(),

                Prices = new Prices
                {
                    ScryfallCardId = cardId,
                    Usd = reader["Usd"]?.ToString(),
                    Usd_Foil = reader["UsdFoil"]?.ToString(),
                    Usd_Etched = reader["UsdEtched"]?.ToString(),
                    Eur = reader["Eur"]?.ToString(),
                    Eur_Foil = reader["EurFoil"]?.ToString(),
                    Tix = reader["Tix"]?.ToString()
                },
                ImageUris = new ImageUris
                {
                    ScryfallCardId = cardId,
                    Small = reader["Small"]?.ToString(),
                    Normal = reader["Normal"]?.ToString(),
                    Large = reader["Large"]?.ToString(),
                    Png = reader["Png"]?.ToString(),
                    ArtCrop = reader["ArtCrop"]?.ToString(),
                    BorderCrop = reader["BorderCrop"]?.ToString()
                },
                Legalities = new Legalities
                {
                    ScryfallCardId = cardId,
                    Standard = reader["Standard"]?.ToString(),
                    Future = reader["Future"]?.ToString(),
                    Historic = reader["Historic"]?.ToString(),
                    Timeless = reader["Timeless"]?.ToString(),
                    Gladiator = reader["Gladiator"]?.ToString(),
                    Pioneer = reader["Pioneer"]?.ToString(),
                    Explorer = reader["Explorer"]?.ToString(),
                    Modern = reader["Modern"]?.ToString(),
                    Legacy = reader["Legacy"]?.ToString(),
                    Pauper = reader["Pauper"]?.ToString(),
                    Vintage = reader["Vintage"]?.ToString(),
                    Penny = reader["Penny"]?.ToString(),
                    Commander = reader["Commander"]?.ToString(),
                    Oathbreaker = reader["Oathbreaker"]?.ToString(),
                    StandardBrawl = reader["StandardBrawl"]?.ToString(),
                    Brawl = reader["Brawl"]?.ToString(),
                    Alchemy = reader["Alchemy"]?.ToString(),
                    PauperCommander = reader["PauperCommander"]?.ToString(),
                    Duel = reader["Duel"]?.ToString(),
                    OldSchool = reader["OldSchool"]?.ToString(),
                    Premodern = reader["Premodern"]?.ToString(),
                    Predh = reader["Predh"]?.ToString()
                }
            };
        }

    }
}
