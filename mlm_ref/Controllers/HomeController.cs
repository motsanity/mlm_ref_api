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
    public async Task<IActionResult> Register(
        [FromQuery] string userName,
        [FromQuery] string userRef, 
        [FromQuery] string activationCode,
        [FromQuery] string sponsorRef, 
        [FromQuery] string placementRef, 
        [FromQuery] char memberPosition)
    {
        // The service logic stays the same
        await _service.RegisterMemberAsync(new {
            username = userName,
            owner_id = userName,
            member_code_ref = userRef,
            activation_code = activationCode,
            sponsor_ref = sponsorRef,
            placement_ref = placementRef,
            position = memberPosition
        });

        return Ok(new { message = "Registration and Activation queued" });
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate(string userRef, string activationCode)
    {
        await _service.ActivateMemberAsync(new
        {
            user_ref = userRef,
            activation_code = activationCode
        });

        return Ok(new { message = "Activation queued" });
    }

    [HttpPost("process-system-queue")]
    public async Task<IActionResult> ProcessSystemQueue()
    {
        await _service.ProcessSystemQueue();
        return Ok(new { message = "System queue processing queued" });
    }

    [HttpPost("create-head")]
    public async Task<IActionResult> CreateHead([FromQuery] int headCount)
    {
        if (!new[] { 1, 3, 7, 15, 31 }.Contains(headCount))        {
            return BadRequest(new { message = "Invalid head count. Allowed values are 1, 3, 7, 15, 31." });
        }

        await _service.CreateHead(headCount);
        return Ok(new { message = "Head creation queued" });
    }


}
