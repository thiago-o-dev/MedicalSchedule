import '../models/pet/pet_model.dart';
import '../services/pet_service.dart';

class PetRepository {
  final PetService _service = PetService();

  Future<void> createPet(PetModel pet) async {
    await _service.createPet(pet);
  }

  Future<List<PetModel>> getPets({String? ownerId}) async {
    final response = await _service.getPets(ownerId: ownerId);
    final List data = response.data as List;
    return data
        .map((e) => PetModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
