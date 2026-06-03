import '../models/vet/vet_model.dart';
import '../services/vet_service.dart';

class VetRepository {
  final VetService _service = VetService();

  Future<VetModel> createVet(
    VetModel vet,
  ) async {
    final response = await _service.createVet(vet);

    return VetModel.fromJson(response.data);
  }

  Future<List<VetModel>> getVets() async {
    final response = await _service.getVets();

    final List data = response.data;

    return data
        .map(
          (e) => VetModel.fromJson(e),
        )
        .toList();
  }
}