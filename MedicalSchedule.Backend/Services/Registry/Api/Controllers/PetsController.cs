using Microsoft.AspNetCore.Mvc;
using Registry.Api.Requests;
using Registry.Features.Pets;
using SharedKernel.Abstractions;

namespace Registry.Api.Controllers;

[ApiController]
[Route("api/pets")]
public sealed class PetsController(
    ICommandHandler<RegisterPetCommand, Guid> registerPet,
    ICommandHandler<AddPetOwnerCommand> addPetOwner,
    ICommandHandler<RemovePetOwnerCommand> removePetOwner,
    IQueryHandler<GetPetOwnerQuery, OwnerContactResponse?> getPetOwner) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterPetRequest request,
        CancellationToken cancellationToken)
    {
        var id = await registerPet.HandleAsync(
            new RegisterPetCommand(request.Name, request.Species, request.Breed, request.BirthDate, request.PrimaryOwnerId),
            cancellationToken);

        return CreatedAtAction(nameof(GetOwner), new { petId = id }, new { id });
    }

    [HttpPost("{petId:guid}/owners")]
    public async Task<IActionResult> AddOwner(
        Guid petId,
        [FromBody] AddPetOwnerRequest request,
        CancellationToken cancellationToken)
    {
        await addPetOwner.HandleAsync(
            new AddPetOwnerCommand(petId, request.OwnerId, request.IsPrimaryOwner),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{petId:guid}/owners/{ownerId:guid}")]
    public async Task<IActionResult> RemoveOwner(
        Guid petId,
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        await removePetOwner.HandleAsync(
            new RemovePetOwnerCommand(petId, ownerId),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{petId:guid}/owner")]
    public async Task<IActionResult> GetOwner(
        Guid petId,
        CancellationToken cancellationToken)
    {
        var owner = await getPetOwner.HandleAsync(new GetPetOwnerQuery(petId), cancellationToken);

        return owner is null ? NotFound() : Ok(owner);
    }
}
