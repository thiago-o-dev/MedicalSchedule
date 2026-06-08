import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/storage/token_storage.dart';
import '../models/owner/owner_model.dart';
import '../repositories/owner_repository.dart';

final ownerRepositoryProvider = Provider<OwnerRepository>(
  (ref) => OwnerRepository(),
);

final ownersProvider = FutureProvider<List<OwnerModel>>((ref) {
  return ref.read(ownerRepositoryProvider).getOwners();
});

final currentOwnerProvider = FutureProvider<OwnerModel?>((ref) async {
  final email = await TokenStorage.getEmail();
  if (email == null) return null;
  final owners = await ref.read(ownerRepositoryProvider).getOwners();
  try {
    return owners.firstWhere((o) => o.email == email);
  } catch (_) {
    return null;
  }
});
