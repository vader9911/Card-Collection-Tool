using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CollectionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Collections
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserCardCollection>>> GetUserCardCollections()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
        return await _context.UserCardCollections
            .Where(c => c.UserId == userId) // Filter by UserId
            .ToListAsync();
    }

    // GET: api/Collections/5
    [HttpGet("{collectionId}")]
    public async Task<ActionResult<UserCardCollection>> GetUserCardCollection(int collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userCardCollection = await _context.UserCardCollections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);


        if (userCardCollection == null)
        {
            return NotFound();
        }

        return userCardCollection;
    }

    //// PUT: api/Collections/5
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutUserCardCollection(int id, UserCardCollection userCardCollection)
    //{
    //    if (id != userCardCollection.Id)
    //    {
    //        return BadRequest();
    //    }

    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (userCardCollection.UserId != userId)
    //    {
    //        return Unauthorized();
    //    }

    //    _context.Entry(userCardCollection).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!UserCardCollectionExists(id))
    //        {
    //            return NotFound();
    //        }
    //        else
    //        {
    //            throw;
    //        }
    //    }

    //    return NoContent();
    //}

    // POST: api/Collections
    [HttpPost]
    public async Task<ActionResult<UserCardCollection>> PostUserCardCollection([FromBody] UserCardCollection userCardCollection)
    {
        // Retrieve the authenticated user's ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Ensure the user is authenticated
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        // Set the UserId from the authenticated user's claims
        userCardCollection.UserId = userId;

        // Validate that the collection name is provided
        if (string.IsNullOrEmpty(userCardCollection.CollectionName))
        {
            return BadRequest("Collection name cannot be empty.");
        }

        // Check if a collection with the same name already exists for this user
        var existingCollection = await _context.UserCardCollections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CollectionName == userCardCollection.CollectionName);

        if (existingCollection != null)
        {
            return BadRequest("A collection with this name already exists.");
        }

        _context.UserCardCollections.Add(userCardCollection);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUserCardCollection", new { id = userCardCollection.Id }, userCardCollection);
    }

    // DELETE: api/Collections/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserCardCollection(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userCardCollection = await _context.UserCardCollections
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (userCardCollection == null)
        {
            return NotFound();
        }

        _context.UserCardCollections.Remove(userCardCollection);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserCardCollectionExists(int id)
    {
        return _context.UserCardCollections.Any(e => e.Id == id);
    }

    [HttpPost("{collectionId}/addCard")]
public async Task<IActionResult> AddCardToCollection(int collectionId, [FromBody] AddCardRequest request)
    {
        Console.WriteLine($"Received collectionId: {collectionId}");
        Console.WriteLine($"Received CardId: {request.CardId}");
        Console.WriteLine($"Received Quantity: {request.Quantity}");

        // Validate input
        if (request == null || string.IsNullOrEmpty(request.CardId) || request.Quantity <= 0)
        {
            return BadRequest("Invalid card or quantity.");
        }

        // Find the collection by ID
        var collection = await _context.UserCardCollections.FirstOrDefaultAsync(c => c.Id == collectionId);
        if (collection == null)
        {
            return NotFound(new { message = "Collection not found." });
        }

        // Check if the card already exists in the collection
        var existingCardEntry = collection.CardIds.FirstOrDefault(c => c.CardId == request.CardId);

        if (existingCardEntry != null)
        {
            // If the card already exists, update the quantity
            existingCardEntry.Quantity += request.Quantity;
        }
        else
        {
            // If the card does not exist, add it to the collection
            var newCardEntry = new CardEntry
            {
                CardId = request.CardId,
                Quantity = request.Quantity
            };
            collection.CardIds.Add(newCardEntry);
        }

        // Update the collection in the database
        _context.UserCardCollections.Update(collection);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Card added to the collection successfully." });
    }

    [HttpGet("{collectionId}/details")]
    public async Task<IActionResult> GetCollectionDetails(int collectionId)
    {
        // Find the collection by its ID
        var collection = await _context.UserCardCollections
            .FirstOrDefaultAsync(c => c.Id == collectionId);

        if (collection == null)
        {
            return NotFound(new { message = "Collection not found." });
        }

        // Extract the card IDs from the list of CardEntry objects
        var cardIds = collection.CardIds.Select(entry => entry.CardId).ToList();

        if (cardIds == null || !cardIds.Any())
        {
            return Ok(new { collectionName = collection.CollectionName, cards = new List<ScryfallCard>() });
        }

        // Fetch the full card details from the ScryfallCard table using the card IDs
        var cardDetails = await _context.ScryfallCards
            .Where(card => cardIds.Contains(card.Id))
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
            .ToListAsync();

        // Create a response object with the collection and the card details
        var response = new
        {
            collectionName = collection.CollectionName,
            cards = cardDetails
        };

        return Ok(response);
    }

}
