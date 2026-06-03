import '../models/pet/pet_model.dart';
import '../services/pet_service.dart';

class PetRepository {
  final PetService _service = PetService();

  Future<PetModel> createPet(
    PetModel pet,
  ) async {
    final response = await _service.createPet(pet);

    return PetModel.fromJson(response.data);
  }

  Future<List<PetModel>> getPets(
    String ownerId,
  ) async {
    final response = await _service.getPets(ownerId);

    final List data = response.data;

    return data
        .map(
          (e) => PetModel.fromJson(e),
        )
        .toList();
  }
}