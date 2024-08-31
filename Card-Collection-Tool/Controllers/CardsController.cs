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
        public async Task<IActionResult> SearchCards(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Please enter a search query." });
            }

            try
            {
                var cards = await _scryfallService.SearchCardsAsync(query);

                // Debugging: Log the query and the number of cards returned
                Console.WriteLine($"Search Query: {query}");
                Console.WriteLine($"Cards Returned: {cards.Count}");

                // Return the card data as JSON
                return Json(new { success = true, data = cards });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while searching for cards. Please try again later." });
            }
        }
    }
}
