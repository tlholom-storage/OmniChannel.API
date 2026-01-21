using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using OmniChannel.API.Repositories;
using OmniChannel.API.Services;

namespace OmniChannel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SasController : ControllerBase
    {
        private readonly IActivityUploadRepository _repository;
        private readonly ILogService _logger;

        public SasController(IActivityUploadRepository repository, ILogService logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("token")]
        public async Task<IActionResult> GetUploadLinkAsync([FromQuery] string fileName)
        {
            await _logger.LogAsync($"SAS token requested for {fileName}", "WARN");

            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("Filename is required");

            var uploadUrl = _repository.GenerateUploadSasUrl(fileName);

            await _logger.LogAsync($"SAS token issued for {fileName}");
            return Ok(new { uploadUrl });
        }
    }
}
