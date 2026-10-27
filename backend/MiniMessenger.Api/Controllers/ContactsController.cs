using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Extensions;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private const long MaxFileSize = 10 * 1024 * 1024;   // 10 Mo

    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ContactDto>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _contactService.GetPagedAsync(User.GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpPost("import")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<ImportReportDto>> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Fichier manquant ou vide." });

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Le fichier doit être au format .csv." });

        try
        {
            await using var stream = file.OpenReadStream();
            var report = await _contactService.ImportCsvAsync(User.GetUserId(), stream);
            return Ok(report);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}