import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/pet/pet_model.dart';
import '../repositories/pet_repository.dart';

final petRepositoryProvider = Provider<PetRepository>(
  (ref) => PetRepository(),
);

final petsProvider =
    FutureProvider.family<List<PetModel>, String?>((ref, ownerId) {
  return ref.read(petRepositoryProvider).getPets(ownerId: ownerId);
});
