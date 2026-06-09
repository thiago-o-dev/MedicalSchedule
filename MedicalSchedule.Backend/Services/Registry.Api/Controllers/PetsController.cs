using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registry.Api.Requests;
using Registry.Features.Pets;
using SharedKernel.Abstractions;

namespace Registry.Api.Controllers;

[ApiController]
[Route("api/pets")]
[Authorize]
public sealed class PetsController(
    ICommandHandler<RegisterPetCommand, Guid> registerPet,
    ICommandHandler<AddPetOwnerCommand> addPetOwner,
    ICommandHandler<RemovePetOwnerCommand> removePetOwner,
    ICommandHandler<RequestPetDeletionCommand> requestPetDeletion,
    IQueryHandler<GetAllPetsQuery, IReadOnlyList<PetResponse>> getAllPets,
    IQueryHandler<GetPetByIdQuery, PetResponse> getPetById,
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

        return CreatedAtAction(nameof(GetById), new { petId = id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? ownerId,
        CancellationToken cancellationToken)
    {
        var result = await getAllPets.HandleAsync(new GetAllPetsQuery(ownerId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{petId:guid}")]
    public async Task<IActionResult> GetById(
        Guid petId,
        CancellationToken cancellationToken)
    {
        var pet = await getPetById.HandleAsync(new GetPetByIdQuery(petId), cancellationToken);
        return Ok(pet);
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

    [HttpDelete("{petId:guid}")]
    public async Task<IActionResult> RequestDeletion(
        Guid petId,
        CancellationToken cancellationToken)
    {
        await requestPetDeletion.HandleAsync(
            new RequestPetDeletionCommand(petId),
            cancellationToken);

        return Accepted(new
        {
            petId,
            status = "PendingDeletion",
            message = "Pet deletion submitted. The Scheduling service is verifying future appointments."
        });
    }

    // Internal endpoint consumed by the Notifications background service,
    // which has no user JWT to forward. Not exposed via the Gateway.
    [HttpGet("{petId:guid}/owner")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOwner(
        Guid petId,
        CancellationToken cancellationToken)
    {
        var owner = await getPetOwner.HandleAsync(new GetPetOwnerQuery(petId), cancellationToken);

        return owner is null ? NotFound() : Ok(owner);
    }
}
