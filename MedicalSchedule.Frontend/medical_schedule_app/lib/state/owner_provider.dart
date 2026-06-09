import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/owner/owner_model.dart';
import '../repositories/owner_repository.dart';

final ownerRepositoryProvider = Provider<OwnerRepository>(
  (ref) => OwnerRepository(),
);

final ownersProvider = FutureProvider.autoDispose<List<OwnerModel>>((ref) {
  return ref.read(ownerRepositoryProvider).getOwners();
});

final ownerByIdProvider =
    FutureProvider.autoDispose.family<OwnerModel, String>((ref, id) {
  return ref.read(ownerRepositoryProvider).getOwnerById(id);
});

final currentOwnerProvider = FutureProvider<OwnerModel?>((ref) {
  return ref.read(ownerRepositoryProvider).getCurrentOwner();
});
