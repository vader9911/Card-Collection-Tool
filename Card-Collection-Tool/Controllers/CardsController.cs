using Microsoft.AspNetCore.Mvc;
using Card_Collection_Tool.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Card_Collection_Tool.Controllers
{
    public class CardsController : Controller
    {
        private readonly ScryfallService _scryfallService;

        public CardsController(ScryfallService scryfallService)
        {
            _scryfallService = scryfallService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Temporarily disable CSRF protection for testing
        public async Task<IActionResult> SearchCardsByName(string query, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Please enter a search query." });
            }

            try
            {
                // Fetch and parse the JSON response 
                var cards = await _scryfallService.SearchCardsAsync(query);

                // Implement pagination on the backend
                var paginatedCards = cards
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Only send the necessary fields to the client
                var result = paginatedCards.Select(card => new
                {
                    name = card.Name,
                    oracle_text = card.OracleText,
                    set_name = card.SetName,
                    image_uris = card.ImageUris?.Normal
                });
                Console.WriteLine(Json(new { success = true, data = result }));
                return Json(new { success = true, data = result });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while searching for cards. Please try again later." });
            }
        }
    }
}
