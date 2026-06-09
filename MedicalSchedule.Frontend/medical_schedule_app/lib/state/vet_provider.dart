import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/vet/vet_model.dart';
import '../repositories/vet_repository.dart';

final vetRepositoryProvider = Provider<VetRepository>(
  (ref) => VetRepository(),
);

final vetsProvider = FutureProvider.autoDispose<List<VetModel>>((ref) {
  return ref.read(vetRepositoryProvider).getVets();
});

final vetByIdProvider =
    FutureProvider.autoDispose.family<VetModel, String>((ref, id) {
  return ref.read(vetRepositoryProvider).getVetById(id);
});

final currentVetProvider = FutureProvider<VetModel?>((ref) {
  return ref.read(vetRepositoryProvider).getCurrentVet();
});
