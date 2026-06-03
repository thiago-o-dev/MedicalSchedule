import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../repositories/vet_repository.dart';

final vetRepositoryProvider =
    Provider<VetRepository>(
  (ref) => VetRepository(),
);