using Microsoft.AspNetCore.Mvc;
using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Card_Collection_Tool.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;

namespace Card_Collection_Tool.Controllers
{
    public class CardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ScryfallService _scryfallService;

        public CardsController(ApplicationDbContext context, ScryfallService scryfallService)
        {
            _context = context;
            _scryfallService = scryfallService;
        }

        /*-- Old single card fetch method used before bulk db data import method --*/

        //[HttpPost]
        //[IgnoreAntiforgeryToken] // Temporarily disable CSRF protection for testing
        //public async Task<IActionResult> SearchCardsByName(string query, int page = 1, int pageSize = 20)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //    {
        //        return Json(new { success = false, message = "Please enter a search query." });
        //    }

        //    try
        //    {
        //        // Fetch and parse the JSON response 
        //        var cards = await _scryfallService.SearchCardsAsync(query);

        //        // Implement pagination on the backend
        //        var paginatedCards = cards
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        // Only send the necessary fields to the client
        //        var result = paginatedCards.Select(card => new
        //        {
        //            name = card.Name,
        //            oracle_text = card.OracleText,
        //            set_name = card.SetName,
        //            image_uris = card.ImageUris?.Normal
        //        });
        //        Console.WriteLine(Json(new { success = true, data = result }));
        //        return Json(new { success = true, data = result });

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error occurred: {ex.Message}");
        //        return Json(new { success = false, message = "An error occurred while searching for cards. Please try again later." });
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> AutocompleteSearch(string query)
        {
            
            var autocompleteResults = await _scryfallService.GetAutocompleteResultsAsync(query);

            
            var matchingCards = await _context.ScryfallCards
                .Where(card => autocompleteResults.Contains(card.Name))
                .GroupBy(card => card.Name)
                .Select(group => group.OrderByDescending(c => c.ReleaseDate).FirstOrDefault()) 
                .ToListAsync();

 
            var result = matchingCards.Select(card => new
            {
                id = card.Id,
                name = card.Name,
                imageUri = card.ImageUris.Png ?? card.ImageUris.Normal ?? card.ImageUris.Large// Display image URI
            });

            return Json(result);
        }
    }
}
