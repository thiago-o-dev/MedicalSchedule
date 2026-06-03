import '../models/owner/owner_model.dart';
import '../services/owner_service.dart';

class OwnerRepository {
  final OwnerService _service = OwnerService();

  Future<OwnerModel> createOwner(
    OwnerModel owner,
  ) async {
    final response = await _service.createOwner(owner);

    return OwnerModel.fromJson(response.data);
  }
}