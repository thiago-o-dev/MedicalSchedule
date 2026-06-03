import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../repositories/owner_repository.dart';

final ownerRepositoryProvider =
    Provider<OwnerRepository>(
  (ref) => OwnerRepository(),
);