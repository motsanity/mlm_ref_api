using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mlm_ref.Models;
using mlm_ref.Services;
using mlm_ref.Features.User;

namespace mlm_ref.Controllers;

public class HomeController : Controller
{
    private readonly PlacementService _service;

    public HomeController(PlacementService service)
    {
        _service = service;
    }

    [HttpPost("register-and-activate")]
public async Task<IActionResult> Register(RegisterDto dto)
{
    await _service.RegisterMemberAsync(new
    {
        username = dto.Username,
        owner_id = dto.OwnerId,
        member_code_ref = dto.UserRef, // The 'ref' from member_codes table
        activation_code = dto.ActivationCode, // The actual binary/hex code
        sponsor_ref = dto.SponsorRef,
        placement_ref = dto.PlacementRef,
        position = dto.Position
    });

    return Ok(new { message = "Registration and Activation queued" });
}

    [HttpPost("activate")]
    public async Task<IActionResult> Activate(ActivateDto dto)
    {
        await _service.ActivateMemberAsync(new
        {
            user_ref = dto.UserRef,
            activation_code = dto.ActivationCode
        });

        return Ok(new { message = "Activation queued" });
    }

    [HttpPost("process-system-queue")]
    public async Task<IActionResult> ProcessSystemQueue()
    {
        await _service.ProcessSystemQueue();
        return Ok(new { message = "System queue processing queued" });
    }
}
