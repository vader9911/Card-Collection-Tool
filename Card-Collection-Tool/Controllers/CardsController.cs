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
            // Base SQL query with necessary joins
            var sql = new StringBuilder(@"
    SELECT 
        c.*,
        p.Usd, p.UsdFoil, p.UsdEtched, p.Eur, p.EurFoil, p.Tix,
        i.Small, i.Normal, i.Large, i.Png, i.ArtCrop, i.BorderCrop,
        l.Standard, l.Future, l.Historic, l.Timeless, l.Gladiator, 
        l.Pioneer, l.Explorer, l.Modern, l.Legacy, l.Pauper, 
        l.Vintage, l.Penny, l.Commander, l.Oathbreaker, l.StandardBrawl, 
        l.Brawl, l.Alchemy, l.PauperCommander, l.Duel, l.OldSchool, 
        l.Premodern, l.Predh
    FROM ScryfallCards c
    LEFT JOIN Prices p ON c.Id = p.ScryfallCardId
    LEFT JOIN ImageUris i ON c.Id = i.ScryfallCardId
    LEFT JOIN Legalities l ON c.Id = l.ScryfallCardId
    WHERE 1=1");

            var parameters = new List<SqlParameter>();

            // Apply filters
            if (!string.IsNullOrEmpty(name))
            {
                sql.Append(" AND c.Name LIKE '%' + @name + '%'");
                parameters.Add(new SqlParameter("@name", name));
            }
            if (!string.IsNullOrEmpty(set))
            {
                sql.Append(" AND c.SetName LIKE '%' + @set + '%'");
                parameters.Add(new SqlParameter("@set", set));
            }
            if (!string.IsNullOrEmpty(oracleText))
            {
                sql.Append(" AND c.OracleText LIKE '%' + @oracleText + '%'");
                parameters.Add(new SqlParameter("@oracleText", oracleText));
            }
            if (!string.IsNullOrEmpty(type))
            {
                sql.Append(" AND c.TypeLine LIKE '%' + @type + '%'");
                parameters.Add(new SqlParameter("@type", type));
            }

            // Handle color and color identity filters
            if (!string.IsNullOrEmpty(colors))
            {
                var colorArray = colors.Split(',');

                if (colorCriteria == "exact")
                {
                    sql.Append(" AND c.Colors = @colors");
                    parameters.Add(new SqlParameter("@colors", colors));
                }
                else if (colorCriteria == "any")
                {
                    for (int i = 0; i < colorArray.Length; i++)
                    {
                        sql.Append($" AND c.Colors LIKE '%' + @color{i} + '%'");
                        parameters.Add(new SqlParameter($"@color{i}", colorArray[i].Trim()));
                    }
                }
            }

            if (!string.IsNullOrEmpty(colorIdentity))
            {
                var colorIdentityArray = colorIdentity.Split(',');

                if (colorIdentityCriteria == "exact")
                {
                    sql.Append(" AND c.ColorIdentity = @colorIdentity");
                    parameters.Add(new SqlParameter("@colorIdentity", colorIdentity));
                }
                else if (colorIdentityCriteria == "any")
                {
                    for (int i = 0; i < colorIdentityArray.Length; i++)
                    {
                        sql.Append($" AND c.ColorIdentity LIKE '%' + @ci{i} + '%'");
                        parameters.Add(new SqlParameter($"@ci{i}", colorIdentityArray[i].Trim()));
                    }
                }
            }



            // Handle numerical filters with comparators
            if (manaValue.HasValue)
            {
                var comparator = manaValueComparator == "greater" ? ">" : manaValueComparator == "less" ? "<" : "=";
                sql.Append($" AND c.Cmc {comparator} @manaValue");
                parameters.Add(new SqlParameter("@manaValue", manaValue));
            }
            if (!string.IsNullOrEmpty(power))
            {
                var comparator = powerComparator == "greater" ? ">" : powerComparator == "less" ? "<" : "=";
                sql.Append($" AND c.Power {comparator} @power");
                parameters.Add(new SqlParameter("@power", power));
            }
            if (!string.IsNullOrEmpty(toughness))
            {
                var comparator = toughnessComparator == "greater" ? ">" : toughnessComparator == "less" ? "<" : "=";
                sql.Append($" AND c.Toughness {comparator} @toughness");
                parameters.Add(new SqlParameter("@toughness", toughness));
            }
            if (!string.IsNullOrEmpty(loyalty))
            {
                var comparator = loyaltyComparator == "greater" ? ">" : loyaltyComparator == "less" ? "<" : "=";
                sql.Append($" AND c.Loyalty {comparator} @loyalty");
                parameters.Add(new SqlParameter("@loyalty", loyalty));
            }

            // Handle dynamic sorting
            var validSortColumns = new[] { "name", "cmc", "price", "toughness", "power" }; // Valid columns to sort by
            if (validSortColumns.Contains(sortOrder.ToLower()))
            {
                sql.Append($" ORDER BY c.{sortOrder} {sortDirection}");
            }

            // Execute the query 
            var results = new List<ScryfallCard>();
            using (var connection = new SqlConnection("Server=DESKTOP-O35BQH4\\SQLEXPRESS;Database=Card-Collecting-Tool;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"))
            {
                using (var command = new SqlCommand(sql.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            while (reader.Read())
                            {
                                // extract the Id from the reader
                                var cardId = reader["Id"].ToString();

                                
                                var card = new ScryfallCard
                                {
                                    Id = cardId,
                                    Name = reader["Name"].ToString(),
                                    Cmc = reader["Cmc"] != DBNull.Value ? Convert.ToSingle(reader["Cmc"]) : (float?)null,
                                    ManaCost = reader["ManaCost"].ToString(),
                                    TypeLine = reader["TypeLine"].ToString(),
                                    OracleText = reader["OracleText"].ToString(),
                                    Power = reader["Power"].ToString(),
                                    Toughness = reader["Toughness"].ToString(),
                                    Prices = new Prices
                                    {
                                        ScryfallCardId = cardId,
                                        Usd = reader["Usd"]?.ToString(),
                                        UsdFoil = reader["UsdFoil"]?.ToString(),
                                        UsdEtched = reader["UsdEtched"]?.ToString(),
                                        Eur = reader["Eur"]?.ToString(),
                                        EurFoil = reader["EurFoil"]?.ToString(),
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

                                results.Add(card);
                            }
                        }
                    }
                }

                return Ok(results);
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
        
    }
}
