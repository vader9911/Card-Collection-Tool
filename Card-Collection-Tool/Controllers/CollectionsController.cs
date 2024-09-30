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
    public async Task<IActionResult> GetUserCardCollections()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT CollectionID, CollectionName, ImageUri, TotalCards, TotalValue
            FROM UserCardCollection 
            WHERE UserID = @UserID";

            var collections = new List<UserCardCollection>();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        collections.Add(new UserCardCollection
                        {
                            CollectionID = reader.IsDBNull(reader.GetOrdinal("CollectionID"))
                                ? 0
                                : reader.GetInt32(reader.GetOrdinal("CollectionID")),

                            CollectionName = reader.IsDBNull(reader.GetOrdinal("CollectionName"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("CollectionName")),

                            ImageUri = reader.IsDBNull(reader.GetOrdinal("ImageUri"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("ImageUri")),

                            TotalCards = reader.IsDBNull(reader.GetOrdinal("TotalCards"))
                                ? 0
                                : reader.GetInt32(reader.GetOrdinal("TotalCards")),

                            TotalValue = reader.IsDBNull(reader.GetOrdinal("TotalValue"))
                                ? 0
                                : reader.GetDecimal(reader.GetOrdinal("TotalValue"))
                        });
                    }
                }
            }

            return Ok(collections);
        }
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
        try
        {
            Console.WriteLine($"Received Request: CollectionID = {request.CollectionID}, CardID = {request.CardID}, Quantity = {request.Quantity}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Database connection opened successfully.");

                // Add the card to the collection
                var insertCardQuery = @"
            INSERT INTO CollectionCards (CollectionID, CardID, Quantity)
            VALUES (@CollectionID, @CardID, @Quantity)";

                using (var command = new SqlCommand(insertCardQuery, connection))
                {
                    command.Parameters.AddWithValue("@CollectionID", request.CollectionID);
                    command.Parameters.AddWithValue("@CardID", request.CardID);
                    command.Parameters.AddWithValue("@Quantity", request.Quantity);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"{rowsAffected} row(s) inserted into CollectionCards.");
                }

                // Call the stored procedure to update the collection summary
                using (var command = new SqlCommand("UpsertCollectionSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CollectionID", request.CollectionID);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Stored procedure executed. {rowsAffected} row(s) affected.");
                }
            }

            // Return success message
            Console.WriteLine("Card added or updated successfully.");
            return Ok(new { message = "Card added or updated successfully." });
        }
        catch (Exception ex)
        {
            // Log the error and return internal server error
            Console.WriteLine($"Error in AddCardToCollection: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        }
    }



    [HttpPost("delete-card")]
    public async Task RemoveCardFromCollection(int collectionId, string cardId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Remove the card from the collection
            var deleteCardQuery = @"
            DELETE FROM CollectionCards 
            WHERE CollectionID = @CollectionID AND CardID = @CardID";

            using (var command = new SqlCommand(deleteCardQuery, connection))
            {
                command.Parameters.AddWithValue("@CollectionID", collectionId);
                command.Parameters.AddWithValue("@CardID", cardId);
                await command.ExecuteNonQueryAsync();
            }

            // Call the stored procedure to update the collection summary
            using (var command = new SqlCommand("UpsertCollectionSummary", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CollectionID", collectionId);
                await command.ExecuteNonQueryAsync();
            }
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
                CreatedDate = DateTime.MinValue,

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
                        Power, Toughness, FlavorText, ReleaseDate, Variation, Colors, ColorIdentity, Usd
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
                                Usd = cardReader["Usd"]?.ToString(),
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
