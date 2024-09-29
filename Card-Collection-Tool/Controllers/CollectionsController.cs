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
    private readonly ILogger<CollectionsController> _logger;

    public CollectionsController(IConfiguration configuration, ILogger<CollectionsController> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }


    // GET: api/Collections
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserCardCollection>>> GetUserCardCollections()
    {
        // Retrieve the logged-in user's ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Log the start of the request
        _logger.LogInformation("Fetching collections for user ID: {UserId}", userId);

        // Check if the user is authenticated
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt - user is not authenticated.");
            return Unauthorized("User is not authenticated.");
        }

        var collections = new List<UserCardCollection>();

        try
        {
            // Establish a database connection
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                _logger.LogInformation("Database connection opened successfully.");

                var query = @"
                        SELECT CollectionID, UserID, CollectionName, ImageUri, Notes, CreatedDate 
                        FROM UserCardCollection 
                        WHERE UserID = @UserID";

                using (var command = new SqlCommand(query, connection))
                {
                    // Add the user ID parameter to the SQL command
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Read the results of the query and map them to the UserCardCollection objects
                        while (await reader.ReadAsync())
                        {
                            collections.Add(new UserCardCollection
                            {
                                CollectionID = reader.GetInt32(reader.GetOrdinal("CollectionID")),
                                UserId = reader.GetString(reader.GetOrdinal("UserID")),
                                CollectionName = reader.GetString(reader.GetOrdinal("CollectionName")),
                                ImageUri = reader["ImageUri"] != DBNull.Value ? reader["ImageUri"] as string : null,
                                Notes = reader["Notes"] != DBNull.Value ? reader["Notes"] as string : null,
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Successfully retrieved {Count} collections for user ID: {UserId}", collections.Count, userId);
        }
        catch (Exception ex)
        {
            // Log the exception with an error level
            _logger.LogError(ex, "An error occurred while fetching collections for user ID: {UserId}", userId);
            return StatusCode(500, "An error occurred while fetching collections.");
        }

        // Return the list of collections as a response
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



    [HttpPost("upsert-card")]
    public async Task<IActionResult> AddCardToCollection([FromBody] AddCardToCollectionRequest request)
    {
        Console.WriteLine($"Received Request: CollectionID = {request.CollectionID}, CardID = {request.CardID}, Quantity = {request.Quantity}");

        if (request == null || string.IsNullOrEmpty(request.CardID) || request.CollectionID <= 0 || request.Quantity <= 0)
        {
            return BadRequest(new { message = "Invalid request payload. Please ensure all required fields are filled correctly." });
        }

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var validateQuery = "SELECT COUNT(1) FROM UserCardCollection WHERE collectionID = @collectionID";
                using (var validateCommand = new SqlCommand(validateQuery, connection))
                {
                    validateCommand.Parameters.AddWithValue("@collectionID", request.CollectionID);
                    var exists = (int)await validateCommand.ExecuteScalarAsync();

                    if (exists == 0)
                    {
                        return BadRequest(new { message = "The specified collection does not exist. Please select a valid collection." });
                    }
                }

                using (var command = new SqlCommand("UpsertCollectionCard", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@collectionID", request.CollectionID);
                    command.Parameters.AddWithValue("@cardID", request.CardID);
                    command.Parameters.AddWithValue("@quantity", request.Quantity);

                    await command.ExecuteNonQueryAsync();
                }
            }

            // Return a JSON response instead of plain text
            return Ok(new { message = "Card added or updated successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding card to collection: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while adding the card to the collection.", error = ex.Message });
        }
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
                "SELECT CollectionName, ImageUri, Notes, CreatedDate FROM UserCardCollection WHERE CollectionID = @CollectionID AND UserID = @UserID",
                connection
            );
            collectionCmd.Parameters.AddWithValue("@CollectionID", collectionId);
            collectionCmd.Parameters.AddWithValue("@UserID", userId);

            var collectionDetails = new
            {
                CollectionName = string.Empty,
                ImageUri = string.Empty,
                Notes = string.Empty,
                CreatedDate = DateTime.MinValue
            };

            using (var reader = await collectionCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    collectionDetails = new
                    {
                        CollectionName = reader["CollectionName"].ToString(),
                        ImageUri = reader["ImageUri"]?.ToString() ?? string.Empty,
                        Notes = reader["Notes"]?.ToString() ?? string.Empty,
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    };
                }
                else
                {
                    return NotFound(new { message = "Collection not found." });
                }
            }

            // Fetch Card IDs and Quantities from CollectionCards table
            var cardIdsCmd = new SqlCommand(
                "SELECT CardID, Quantity FROM CollectionCards WHERE CollectionID = @CollectionID",
                connection
            );
            cardIdsCmd.Parameters.AddWithValue("@CollectionID", collectionId);

            var cardDataList = new List<object>();

            using (var reader = await cardIdsCmd.ExecuteReaderAsync())
            {
                var cardIds = new List<string>();
                var cardQuantities = new Dictionary<string, int>();

                // Collect Card IDs and Quantities
                while (await reader.ReadAsync())
                {
                    var cardId = reader["CardID"].ToString();
                    var quantity = Convert.ToInt32(reader["Quantity"]);

                    cardIds.Add(cardId);
                    cardQuantities[cardId] = quantity;
                }

                // Fetch detailed card data using the Card IDs from v_CardData view
                foreach (var cardId in cardIds)
                {
                    var cardDataCmd = new SqlCommand(
                        @"SELECT 
                        Id, Name, ManaCost, TypeLine, OracleText, SetName, Artist, Rarity, Normal,
                        Power, Toughness, FlavorText, ReleaseDate, Variation, Colors, ColorIdentity
                      FROM v_CardData 
                      WHERE Id = @CardID",
                        connection
                    );
                    cardDataCmd.Parameters.AddWithValue("@CardID", cardId);

                    using (var cardReader = await cardDataCmd.ExecuteReaderAsync())
                    {
                        if (await cardReader.ReadAsync())
                        {
                            cardDataList.Add(new
                            {
                                CardID = cardReader["Id"].ToString(),
                                Name = cardReader["Name"].ToString(),
                                ManaCost = cardReader["ManaCost"].ToString(),
                                TypeLine = cardReader["TypeLine"].ToString(),
                                OracleText = cardReader["OracleText"].ToString(),
                                SetName = cardReader["SetName"].ToString(),
                                Artist = cardReader["Artist"].ToString(),
                                Rarity = cardReader["Rarity"].ToString(),
                                ImageUri = cardReader["Normal"].ToString(),
                                Power = cardReader["Power"]?.ToString(),
                                Toughness = cardReader["Toughness"]?.ToString(),
                                FlavorText = cardReader["FlavorText"]?.ToString(),
                                ReleaseDate = cardReader["ReleaseDate"]?.ToString(),
                                Variation = cardReader["Variation"] != DBNull.Value ? Convert.ToBoolean(cardReader["Variation"]) : false,
                                Colors = cardReader["Colors"]?.ToString()?.Split(',').ToList() ?? new List<string>(),
                                ColorIdentity = cardReader["ColorIdentity"]?.ToString()?.Split(',').ToList() ?? new List<string>(),
                                Quantity = cardQuantities[cardId] // Get the quantity from the previously populated dictionary
                            });
                        }
                    }
                }
            }

            // Return the combined collection and card details
            return Ok(new
            {
                collectionDetails.CollectionName,
                collectionDetails.ImageUri,
                collectionDetails.Notes,
                collectionDetails.CreatedDate,
                Cards = cardDataList
            });
        }
    }



    [HttpGet("{collectionId}/card-ids")]
    public async Task<ActionResult<IEnumerable<string>>> GetCardIdsByCollectionId(int collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Ensure the user is authenticated
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        var cardIds = new List<string>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT CardID
            FROM CollectionCards
            WHERE CollectionID = @CollectionID";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CollectionID", collectionId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cardIds.Add(reader.GetString(reader.GetOrdinal("CardID")));
                    }
                }
            }
        }

        return Ok(cardIds);
    }

}
