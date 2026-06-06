using Microsoft.AspNetCore.Mvc;

namespace Registry.Api.Controllers;

[Route("api/pets")]
public sealed class PetsController : Controller
{
    private readonly PetService _service;

    public PetsController(PetService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        RegisterPetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.RegisterAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{petId:guid}/owners")]
    public async Task<IActionResult> AddOwner(
        Guid petId,
        AddPetOwnerRequest request,
        CancellationToken cancellationToken)
    {
        await _service.AddOwnerAsync(
            petId,
            request,
            cancellationToken);

        return NoContent();
    }
}