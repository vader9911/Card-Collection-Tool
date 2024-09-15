using Microsoft.AspNetCore.Mvc;
using Card_Collection_Tool.Data;
using Card_Collection_Tool.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
     [FromQuery] string query,
     [FromQuery] bool showAllVersions,
     [FromQuery] string type = null,
     [FromQuery] string oracleText = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }

            // Build base query for cards
            var cardQuery = _context.ScryfallCards
                .Where(c => c.Name.Contains(query));

            // Apply filters
            if (!string.IsNullOrEmpty(type))
            {
                cardQuery = cardQuery.Where(c => c.TypeLine.Contains(type));
            }
            if (!string.IsNullOrEmpty(oracleText))
            {
                cardQuery = cardQuery.Where(c => c.OracleText.Contains(oracleText));
            }

            if (!showAllVersions)
            {
                // Filter to only the most recent versions if showAllVersions is false
                cardQuery = cardQuery
                    .GroupBy(c => c.Name)
                    .Select(g => g.OrderByDescending(c => c.ReleaseDate).FirstOrDefault());
            }

            var filteredCards = await cardQuery.ToListAsync();
            // Select relevant card data including the image URL
            var results = filteredCards
        .AsEnumerable() // Move to client-side evaluation
        .Where(c => !showAllVersions || c == filteredCards
            .Where(x => x.Name == c.Name)
            .OrderByDescending(x => x.ReleaseDate)
            .FirstOrDefault()) // Get only the most recent versions if showAllVersions is false
        .Select(c => new
        {
            c.Name,
            c.TypeLine,
            c.OracleText,
            imageUri = c.ImageUris.Png ?? c.ImageUris.Large ?? c.ImageUris.Normal ?? "default-image-url.png",
            c.SetName,
            c.Rarity,
            c.Prices
        })
        .ToList();

            return Ok(results);
        }


        // Endpoint for autocomplete search
        //[HttpGet("autocomplete")]
        //public async Task<IActionResult> AutocompleteSearch(string query)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //        return BadRequest("Query cannot be empty.");

        //    var autocompleteResults = await _scryfallService.GetAutocompleteResultsAsync(query);

        //    var matchingCards = await _context.ScryfallCards
        //        .Where(card => autocompleteResults.Contains(card.Name))
        //        .GroupBy(card => card.Name)
        //        .Select(group => group.OrderByDescending(c => c.ReleaseDate).FirstOrDefault())
        //        .ToListAsync();

        //    var result = matchingCards
        //        .Where(card => card != null && card.ImageUris != null)
        //        .Select(card => new
        //        {
        //            id = card.Id,
        //            name = card.Name,
        //            imageUri = card.ImageUris.Png ?? card.ImageUris.Large ?? card.ImageUris.Normal ?? "default-image-url.png"
        //        })
        //        .ToList();

        //    return Ok(result); // Return as JSON
        //}

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
