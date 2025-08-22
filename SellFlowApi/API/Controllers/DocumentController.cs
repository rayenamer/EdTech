using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.interfaces;
using API.Helpers;
using API.entities;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly GetUserId _getUserIdHelper;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly UploadHandler _uploadHandler;
        private readonly FileDeployer _fileDeployer;
        private readonly ILogger<DocumentController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public DocumentController(
            IDocumentRepository documentRepository,
            IUserDataRepository userDataRepository,
            ILogger<DocumentController> logger,
            UserManager<AppUser> userManager,
            GetUserId getUserIdHelper
            )
        {
            _getUserIdHelper = getUserIdHelper;
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
                // Find the User record
                var user = await _userDataRepository.GetByUserIdAsync(userId);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                // Convert file to bytes using UploadHandler
                byte[] fileBytes = _uploadHandler.Upload(file);

                // Create document entity with the User ID
                var document = new Document
                {
                    Bytes = fileBytes,
                    UserDataId = user.Id, // Use the User.Id
                    DocumentName = documentName
                };

                // Save to database
                var savedDocument = await _documentRepository.AddAsync(document);

                // Associate document with user
                await _userDataRepository.AddDocumentAsync(user.Id, savedDocument.Id);

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
            var result = await _getUserIdHelper.GetUserIdFromClaims(User);
            _logger.LogInformation("User ID from claims: {UserId}", result.userId);
            var userId = result.userId;

            var user = await _userDataRepository.GetByUserIdAsync(userId);
           
            if (user == null)
            {
                _logger.LogWarning("User not found for User ID: {UserId}", userId);
                return false;
            }

            var Id = user.Id;
            _logger.LogInformation("User ID: {UserId}", Id);

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
            var user = await _userDataRepository.GetByUserIdAsync(userId);
            if (user == null) return false;
            var Id = user.Id; // Get the User ID

            return await _documentRepository.DeleteDocByNameAndUserDataId(documentName, Id);
        }
    }
}
            