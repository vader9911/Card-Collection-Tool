using System.Data;
using Microsoft.AspNetCore.Mvc;
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
 
  )
        {
            var sql = new StringBuilder("SELECT * FROM ScryfallCards WHERE 1=1");
            var parameters = new List<SqlParameter>();


            // Execute the query using ADO.NET
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
                            // Safely retrieve and convert each column
                            var card = new ScryfallCard
                            {
                                Id = reader["Id"] != DBNull.Value ? reader["Id"].ToString() : string.Empty,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                                Cmc = reader["Cmc"] != DBNull.Value ? Convert.ToSingle(reader["Cmc"]) : (float?)null,
                                ColorIdentity = reader["ColorIdentity"] != DBNull.Value ? JsonConvert.DeserializeObject<List<string>>(reader["ColorIdentity"].ToString()) : new List<string>(),
                                Colors = reader["Colors"] != DBNull.Value ? JsonConvert.DeserializeObject<List<string>>(reader["Colors"].ToString()) : new List<string>(),
                                Keywords = reader["Keywords"] != DBNull.Value ? JsonConvert.DeserializeObject<List<string>>(reader["Keywords"].ToString()) : new List<string>(),
                                Legalities = reader["Legalities"] != DBNull.Value ? JsonConvert.DeserializeObject<Legalities>(reader["Legalities"].ToString()) : null,
                                ManaCost = reader["ManaCost"] != DBNull.Value ? reader["ManaCost"].ToString() : string.Empty,
                                Power = reader["Power"] != DBNull.Value ? reader["Power"].ToString() : string.Empty,
                                Toughness = reader["Toughness"] != DBNull.Value ? reader["Toughness"].ToString() : string.Empty,
                                TypeLine = reader["TypeLine"] != DBNull.Value ? reader["TypeLine"].ToString() : string.Empty,
                                Artist = reader["Artist"] != DBNull.Value ? reader["Artist"].ToString() : string.Empty,
                                CollectorNumber = reader["CollectorNumber"] != DBNull.Value ? reader["CollectorNumber"].ToString() : string.Empty,
                                Digital = reader["Digital"] != DBNull.Value && Convert.ToBoolean(reader["Digital"]),
                                FlavorText = reader["FlavorText"] != DBNull.Value ? reader["FlavorText"].ToString() : string.Empty,
                                FullArt = reader["FullArt"] != DBNull.Value && Convert.ToBoolean(reader["FullArt"]),
                                Games = reader["Games"] != DBNull.Value ? JsonConvert.DeserializeObject<List<string>>(reader["Games"].ToString()) : new List<string>(),
                                ImageUris = reader["ImageUris"] != DBNull.Value ? JsonConvert.DeserializeObject<ImageUris>(reader["ImageUris"].ToString()) : null,
                                Rarity = reader["Rarity"] != DBNull.Value ? reader["Rarity"].ToString() : string.Empty,
                                ReleaseDate = reader["ReleaseDate"] != DBNull.Value ? reader["ReleaseDate"].ToString() : string.Empty,
                                Reprint = reader["Reprint"] != DBNull.Value && Convert.ToBoolean(reader["Reprint"]),
                                SetName = reader["SetName"] != DBNull.Value ? reader["SetName"].ToString() : string.Empty,
                                Set = reader["Set"] != DBNull.Value ? reader["Set"].ToString() : string.Empty,
                                SetId = reader["SetId"] != DBNull.Value ? reader["SetId"].ToString() : string.Empty,
                                Variation = reader["Variation"] != DBNull.Value && Convert.ToBoolean(reader["Variation"]),
                                VariationOf = reader["VariationOf"] != DBNull.Value ? reader["VariationOf"].ToString() : string.Empty
                            };
                            results.Add(card);
                        }
                    }
                }
            }

            return Ok(results);
        }





        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetCardAutocomplete([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search cannot be empty.");
            }

            var matchingCards = await _context.ScryfallCards
                .Where(card => card.Name.Contains(query))
                .OrderBy(card => card.Name)
                .Select(card => new
                {
                    id = card.Id,
                    name = card.Name,
                    imageUri = card.ImageUris.Png ?? card.ImageUris.Large ?? card.ImageUris.Normal ?? "default-image-url.png"
                })
                .Take(20)
                .Distinct()
                .ToListAsync();

          

            return Ok(matchingCards);
        }


        // GET: api/cards/{cardId}
        [HttpGet("{cardId}/details")]
        public async Task<IActionResult> GetCardDetails(string cardId)
        {
            Console.WriteLine($"Received request for card ID: {cardId}");
            if (string.IsNullOrEmpty(cardId))
            {
                return BadRequest(new { message = "Card ID cannot be null or empty." });
            }

            var card = await _context.ScryfallCards
                .Include(c => c.ImageUris) // Include related data if necessary
                .Where(c => c.Id == cardId)
                .Select(card => new
                    {
                        card.Id,
                        card.Name,
                        ImageUri = card.ImageUris.Normal,
                        card.ManaCost,
                        card.TypeLine,
                        card.OracleText,
                        card.SetName,
                        card.Artist,
                        card.Rarity,
                        Prices = card.Prices.USD
                    })
                .FirstOrDefaultAsync();

            if (card == null)
            {
                return NotFound(new { message = "Card not found." });
            }

       
            return Ok(card);
        }
    }
}
