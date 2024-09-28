using Card_Collection_Tool.Data;
using Card_Collection_Tool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly string _connectionString;

    public CollectionsController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }


    // GET: api/Collections
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserCardCollection>>> GetUserCardCollections()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        var collections = new List<UserCardCollection>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = @"
                    SELECT CollectionID, UserID, CollectionName, ImageUri, Notes, CreatedDate 
                    FROM UserCardCollection 
                    WHERE UserID = @UserID";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        collections.Add(new UserCardCollection
                        {
                            CollectionID = reader.GetInt32(reader.GetOrdinal("CollectionID")),
                            UserId = reader.GetString(reader.GetOrdinal("UserID")),
                            CollectionName = reader.GetString(reader.GetOrdinal("CollectionName")),
                            ImageUri = reader["ImageUri"] as string,
                            Notes = reader["Notes"] as string,
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                        });
                    }
                }
            }
        }

        return Ok(collections);
    }


    // GET: api/Collections/5
    [HttpGet("{collectionId}")]
    public async Task<ActionResult<UserCardCollection>> GetUserCardCollection(int collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Ensure the user is authenticated
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        UserCardCollection userCardCollection = null;

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // SQL query to select the collection based on the collection ID and user ID
            var query = @"
            SELECT CollectionID, UserID, CollectionName, ImageUri, Notes, CreatedDate
            FROM UserCardCollection
            WHERE CollectionID = @CollectionID AND UserID = @UserID";

            using (var command = new SqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@CollectionID", collectionId);
                command.Parameters.AddWithValue("@UserID", userId);

                // Execute the command and read the results
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        userCardCollection = new UserCardCollection
                        {
                            CollectionID = reader.GetInt32(reader.GetOrdinal("CollectionID")),
                            UserId = reader.GetString(reader.GetOrdinal("UserID")),
                            CollectionName = reader.GetString(reader.GetOrdinal("CollectionName")),
                            ImageUri = reader["ImageUri"] as string,
                            Notes = reader["Notes"] as string,
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                        };
                    }
                }
            }
        }

        // Check if the collection was found
        if (userCardCollection == null)
        {
            return NotFound();
        }

        return Ok(userCardCollection);
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
    // CollectionsController.cs

    [HttpPost("create")]
    public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        if (string.IsNullOrEmpty(request.CollectionName))
        {
            return BadRequest("Collection name is required.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Execute the stored procedure to create the collection
            var command = new SqlCommand("dbo.UpsertCollection", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@CollectionID", DBNull.Value);  // NULL for creation
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@CollectionName", request.CollectionName);
            command.Parameters.AddWithValue("@ImageUri", (object)request.ImageUri ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object)request.Notes ?? DBNull.Value);

            // Execute and get the new CollectionID
            var newCollectionId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return Ok(new { CollectionID = newCollectionId, message = "Collection created successfully." });
        }
    }


    // DELETE: api/Collections/5
    [HttpDelete("{collectionId}/delete")]
    public async Task<IActionResult> DeleteCollection(int collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Start a transaction to ensure all deletions occur safely
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Delete all cards in the collection
                    var deleteCardsCmd = new SqlCommand(
                        "DELETE FROM CollectionCards WHERE CollectionID = @CollectionID",
                        connection, transaction
                    );
                    deleteCardsCmd.Parameters.AddWithValue("@CollectionID", collectionId);
                    await deleteCardsCmd.ExecuteNonQueryAsync();

                    // Delete the collection itself
                    var deleteCollectionCmd = new SqlCommand(
                        "DELETE FROM UserCardCollection WHERE CollectionID = @CollectionID AND UserID = @UserID",
                        connection, transaction
                    );
                    deleteCollectionCmd.Parameters.AddWithValue("@CollectionID", collectionId);
                    deleteCollectionCmd.Parameters.AddWithValue("@UserID", userId);
                    var rowsAffected = await deleteCollectionCmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        // Roll back if the collection does not exist or does not belong to the user
                        transaction.Rollback();
                        return NotFound(new { message = "Collection not found or you do not have permission to delete it." });
                    }

                    // Commit the transaction if all operations were successful
                    transaction.Commit();
                    return Ok(new { message = "Collection deleted successfully." });
                }
                catch (Exception ex)
                {
                    // Roll back the transaction on error
                    transaction.Rollback();
                    return StatusCode(500, new { message = "An error occurred while deleting the collection.", error = ex.Message });
                }
            }
        }
    }



    [HttpPut("{collectionId}/update")]
    public async Task<IActionResult> UpdateCollection(int collectionId, [FromBody] UpdateCollectionRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        if (string.IsNullOrEmpty(request.CollectionName))
        {
            return BadRequest("Collection name is required.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Execute the stored procedure to update the collection
            var command = new SqlCommand("dbo.UpsertCollection", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@CollectionID", collectionId);  // Pass the existing CollectionID
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@CollectionName", request.CollectionName);
            command.Parameters.AddWithValue("@ImageUri", (object)request.ImageUri ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object)request.Notes ?? DBNull.Value);

            // Execute and confirm the CollectionID
            var updatedCollectionId = Convert.ToInt32(await command.ExecuteScalarAsync());

            if (updatedCollectionId != collectionId)
            {
                return NotFound(new { message = "Collection not found or you do not have permission to update it." });
            }

            return Ok(new { message = "Collection updated successfully." });
        }
    }



    [HttpPost("{collectionId}/addCard")]
    public async Task<IActionResult> AddCardToCollection(int collectionId, [FromBody] AddCardRequest request)
    {
        // Validate input
        if (request == null || string.IsNullOrEmpty(request.CardId) || request.Quantity <= 0)
        {
            return BadRequest("Invalid card or quantity.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Check if the collection exists and belongs to the user
            var checkCollectionCmd = new SqlCommand(
                "SELECT COUNT(*) FROM UserCardCollection WHERE CollectionID = @CollectionID AND UserID = @UserID",
                connection
            );
            checkCollectionCmd.Parameters.AddWithValue("@CollectionID", collectionId);
            checkCollectionCmd.Parameters.AddWithValue("@UserID", userId);

            var exists = (int)await checkCollectionCmd.ExecuteScalarAsync() > 0;
            if (!exists)
            {
                return NotFound(new { message = "Collection not found." });
            }

            // Use the stored procedure to add or update the card in the collection
            var command = new SqlCommand("dbo.UpsertCollectionCard", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@CollectionID", collectionId);
            command.Parameters.AddWithValue("@CardID", request.CardId);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);

            await command.ExecuteNonQueryAsync();
        }

        return Ok(new { message = "Card added to the collection successfully." });
    }

    [HttpGet("{collectionId}/details")]
    public async Task<IActionResult> GetCollectionDetails(int collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Fetch collection details
            var collectionCmd = new SqlCommand(
                "SELECT CollectionName FROM UserCardCollection WHERE CollectionID = @CollectionID AND UserID = @UserID",
                connection
            );
            collectionCmd.Parameters.AddWithValue("@CollectionID", collectionId);
            collectionCmd.Parameters.AddWithValue("@UserID", userId);

            var collectionName = await collectionCmd.ExecuteScalarAsync() as string;
            if (collectionName == null)
            {
                return NotFound(new { message = "Collection not found." });
            }

            // Fetch card details
            var cardCmd = new SqlCommand(
                @"SELECT c.CardID, sc.Name, sc.ManaCost, sc.TypeLine, sc.OracleText, sc.SetName, sc.Artist, sc.Rarity, sc.ImageUris_Normal, cc.Quantity 
              FROM CollectionCards cc
              JOIN ScryfallCards sc ON cc.CardID = sc.Id
              WHERE cc.CollectionID = @CollectionID",
                connection
            );
            cardCmd.Parameters.AddWithValue("@CollectionID", collectionId);

            var cards = new List<object>();
            using (var reader = await cardCmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    cards.Add(new
                    {
                        CardID = reader["CardID"].ToString(),
                        Name = reader["Name"].ToString(),
                        ManaCost = reader["ManaCost"].ToString(),
                        TypeLine = reader["TypeLine"].ToString(),
                        OracleText = reader["OracleText"].ToString(),
                        SetName = reader["SetName"].ToString(),
                        Artist = reader["Artist"].ToString(),
                        Rarity = reader["Rarity"].ToString(),
                        ImageUri = reader["ImageUris_Normal"].ToString(),
                        Quantity = reader["Quantity"]
                    });
                }
            }

            return Ok(new { collectionName, cards });
        }
    }
    }
