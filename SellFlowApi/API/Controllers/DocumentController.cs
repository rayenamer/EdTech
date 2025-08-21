using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.DATA;
using API.Helpers;
using API.entities;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly UploadHandler _uploadHandler;
        private readonly FileDeployer _fileDeployer;
        private readonly ILogger<DocumentController> _logger;
         private readonly UserManager<AppUser> _userManager;

        public DocumentController(IDocumentRepository documentRepository, IUserDataRepository userDataRepository, ILogger<DocumentController> logger, UserManager<AppUser> userManager)
        {
            _documentRepository = documentRepository;
            _userDataRepository = userDataRepository;
            _logger = logger;
            _userManager = userManager;
            _uploadHandler = new UploadHandler();
            _fileDeployer = new FileDeployer();
        }

        /// <summary>
        /// Uploads a document and associates it with the user's profile
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <returns>Information about the uploaded document</returns>
        [HttpPost("add-document/{documentName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddDocument(IFormFile file, string documentName)
        {
            // Validate input
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided or file is empty");
            }

            // Get user ID from claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            try
            {
                // Find the UserData record that belongs to this User
                var userDataCollection = await _userDataRepository.GetByUserIdAsync(userId);
                var userData = userDataCollection.FirstOrDefault();

                if (userData == null)
                {
                    return BadRequest("User profile not found. Please create your profile first.");
                }

                // Convert file to bytes using UploadHandler
                byte[] fileBytes = _uploadHandler.Upload(file);

                // Create document entity with the correct UserData ID
                var document = new Document
                {
                    Bytes = fileBytes,
                    UserDataId = userData.Id, // Use the actual UserData.Id instead of userId
                    DocumentName = documentName
                };

                // Save to database
                var savedDocument = await _documentRepository.AddAsync(document);

                // Associate document with user data
                await _userDataRepository.AddDocumentAsync(userData.Id, savedDocument.Id);

                return Ok(new
                {
                    Id = savedDocument.Id,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Size = fileBytes.Length,
                    UserDataId = savedDocument.UserDataId,
                    Message = "Document uploaded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error in AddDocument: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                // Get user ID from claims to verify ownership (optional security check)
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized("Invalid user authentication.");

                // Optional: Verify the document belongs to this user
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                    return NotFound($"Document with ID {id} not found");

                // Delete the document
                var success = await _documentRepository.DeleteAsync(id);
                if (!success)
                    return NotFound($"Document with ID {id} not found");

                return Ok(new { Message = "Document deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("getdocument/{id}")]
        public async Task<IActionResult> GetDocument(int id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);

                if (document == null)
                {
                    return NotFound($"Document with ID {id} not found");
                }

                return Ok(new
                {
                    Id = document.Id,
                    Size = document.Bytes.Length,
                    Bytes = Convert.ToBase64String(document.Bytes) // Return as base64 for JSON
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();

                var result = documents.Select(d => new
                {
                    Id = d.Id,
                    Size = d.Bytes.Length,
                    DownloadUrl = Url.Action("DownloadDocument", "Document", new { id = d.Id }, Request.Scheme)
                }).ToList();


                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id, [FromQuery] string fileName = "document")
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);

                if (document == null)
                {
                    return NotFound($"Document with ID {id} not found");
                }

                // Detect file type from bytes and get appropriate extension
                string extension = _fileDeployer.DetectFileExtension(document.Bytes);

                // Use FileDeployer to create download file
                var fileResult = _fileDeployer.Deploy(document.Bytes, fileName, extension);

                return fileResult;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("check-document-name-AND-user-id/{documentName}")]
        public async Task<bool> CheckDocumentNameAndUserId(string documentName)
        {
            var result = await GetUserIdFromClaims();
            _logger.LogInformation("User ID from claims: {UserId}", result.userId);
            var userId = result.userId;

            var userDataCollection = await _userDataRepository.GetByUserIdAsync(userId);
           

            var userData = userDataCollection.First(); // Get the first UserData
            var Id = userData.Id;
            _logger.LogInformation("UserData ID: {UserDataId} for User ID: {UserId}", Id, userId);

            var documentExists = await _documentRepository.GetDocByNameAndUserDataId(documentName, Id);
            _logger.LogInformation("Document exists: {DocumentExists} for User ID: {UserId}", documentExists, userId);
            return documentExists;
        }

        [HttpDelete("DeleteDocByName/{documentName}")]
        public async Task<bool> DeleteDocByName(string documentName)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return false;

            if (!int.TryParse(userIdClaim, out int userId))
                return false;
            var userDataCollection = _userDataRepository.GetByUserIdAsync(userId);
            var Id = userDataCollection.Result.First().Id; // Get the first UserData ID

            return await _documentRepository.DeleteDocByNameAndUserDataId(documentName, Id);
        }

        //helper

        private async Task<(int userId, IActionResult error)> GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return (0, Unauthorized("User identifier claim not found."));

            // Try to parse the claim as an integer (works for classic JWT auth)
            if (int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogInformation("Found numeric user ID: {UserId}", userId);
                return (userId, null);
            }

            // For Google OAuth users, find the user by email
            _logger.LogInformation("Non-integer user ID found: {UserIdClaim}. This might be a Google OAuth user.", userIdClaim);

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email claim found for OAuth user");
                return (0, Unauthorized("User email claim not found."));
            }

            // Use the injected UserManager instead of getting it from HttpContext
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null)
            {
                _logger.LogWarning("No user found with email: {Email}", email);
                return (0, NotFound($"User with email {email} not found."));
            }

            userId = appUser.Id;
            _logger.LogInformation("Found user ID {UserId} for email {Email}", userId, email);
            return (userId, null);
        }
    }
}
            