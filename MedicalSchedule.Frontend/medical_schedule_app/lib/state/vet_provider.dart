import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/storage/token_storage.dart';
import '../models/vet/vet_model.dart';
import '../repositories/vet_repository.dart';

final vetRepositoryProvider = Provider<VetRepository>(
  (ref) => VetRepository(),
);

final vetsProvider = FutureProvider<List<VetModel>>((ref) {
  return ref.read(vetRepositoryProvider).getVets();
});

final currentVetProvider = FutureProvider<VetModel?>((ref) async {
  final email = await TokenStorage.getEmail();
  if (email == null) return null;
  final vets = await ref.read(vetRepositoryProvider).getVets();
  try {
    return vets.firstWhere((v) => v.email == email);
  } catch (_) {
    return null;
  }
});
