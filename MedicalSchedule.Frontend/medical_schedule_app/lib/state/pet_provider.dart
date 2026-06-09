import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/owner/owner_contact_model.dart';
import '../models/pet/pet_model.dart';
import '../repositories/pet_repository.dart';

final petRepositoryProvider = Provider<PetRepository>(
  (ref) => PetRepository(),
);

final petsProvider =
    FutureProvider.family<List<PetModel>, String?>((ref, ownerId) {
  return ref.read(petRepositoryProvider).getPets(ownerId: ownerId);
});

final petByIdProvider =
    FutureProvider.family<PetModel, String>((ref, id) {
  return ref.read(petRepositoryProvider).getPetById(id);
});

final petOwnerProvider =
    FutureProvider.family<OwnerContactModel?, String>((ref, petId) {
  return ref.read(petRepositoryProvider).getPetOwner(petId);
});
