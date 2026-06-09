import '../models/vet/vet_model.dart';
import '../services/vet_service.dart';

class VetRepository {
  final VetService _service = VetService();

  Future<VetModel> createVet(VetModel vet) async {
    final response = await _service.createVet(vet);
    return VetModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<List<VetModel>> getVets() async {
    final response = await _service.getVets();
    final data = response.data as List;
    return data
        .map((e) => VetModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<VetModel> getVetById(String id) async {
    final response = await _service.getVetById(id);
    return VetModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> deactivateVet(String id) async {
    await _service.deactivateVet(id);
  }

  Future<VetModel?> getCurrentVet() async {
    try {
      final response = await _service.getMe();
      return VetModel.fromJson(response.data as Map<String, dynamic>);
    } catch (_) {
      return null;
    }
  }
}
