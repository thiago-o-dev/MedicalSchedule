import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../repositories/pet_repository.dart';

final petRepositoryProvider =
    Provider<PetRepository>(
  (ref) => PetRepository(),
);