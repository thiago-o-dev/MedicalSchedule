import '../models/owner/owner_contact_model.dart';
import '../models/pet/pet_model.dart';
import '../services/pet_service.dart';

class PetRepository {
  final PetService _service = PetService();

  Future<void> createPet(PetModel pet) async {
    await _service.createPet(pet);
  }

  Future<List<PetModel>> getPets({String? ownerId}) async {
    final response = await _service.getPets(ownerId: ownerId);
    final data = response.data as List;
    return data
        .map((e) => PetModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<PetModel> getPetById(String id) async {
    final response = await _service.getPetById(id);
    return PetModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<OwnerContactModel?> getPetOwner(String petId) async {
    try {
      final response = await _service.getPetOwner(petId);
      if (response.data == null) return null;
      return OwnerContactModel.fromJson(response.data as Map<String, dynamic>);
    } catch (_) {
      return null;
    }
  }

  Future<void> requestPetDeletion(String petId) async {
    await _service.requestPetDeletion(petId);
  }

  Future<void> addPetOwner(
    String petId, {
    required String ownerId,
    bool isPrimaryOwner = false,
  }) async {
    await _service.addPetOwner(petId, ownerId: ownerId, isPrimaryOwner: isPrimaryOwner);
  }

  Future<void> removePetOwner(String petId, String ownerId) async {
    await _service.removePetOwner(petId, ownerId);
  }
}
