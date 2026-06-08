import '../models/owner/owner_model.dart';
import '../services/owner_service.dart';

class OwnerRepository {
  final OwnerService _service = OwnerService();

  Future<OwnerModel> createOwner(OwnerModel owner) async {
    final response = await _service.createOwner(owner);
    return OwnerModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<List<OwnerModel>> getOwners() async {
    final response = await _service.getOwners();
    final List data = response.data as List;
    return data
        .map((e) => OwnerModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<OwnerModel> getOwnerById(String id) async {
    final response = await _service.getOwnerById(id);
    return OwnerModel.fromJson(response.data as Map<String, dynamic>);
  }
}
