using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [HttpGet("{id}")]
    public async Task<ActionResult<UserCardCollection>> GetUserCardCollection(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userCardCollection = await _context.UserCardCollections
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (userCardCollection == null)
        {
            return NotFound();
        }

        return userCardCollection;
    }

    // PUT: api/Collections/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserCardCollection(int id, UserCardCollection userCardCollection)
    {
        if (id != userCardCollection.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userCardCollection.UserId != userId)
        {
            return Unauthorized();
        }

        _context.Entry(userCardCollection).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserCardCollectionExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Collections
    [HttpPost]
    public async Task<ActionResult<UserCardCollection>> PostUserCardCollection([FromBody] UserCardCollection userCardCollection)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        userCardCollection.UserId = userId; // Ensure the collection is linked to the logged-in user

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
}
