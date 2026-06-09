import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../state/pet_provider.dart';
import '../../widgets/pet_card.dart';

class RelatedPetsScreen extends ConsumerWidget {
  RelatedPetsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final petsAsync = ref.watch(petsProvider(null));

    return petsAsync.when(
      loading: () => Center(child: CircularProgressIndicator()),
      error: (e, _) => Center(child: Text(e.toString())),
      data: (pets) {
        if (pets.isEmpty) {
          return Center(child: Text('No pets registered.'));
        }
        return RefreshIndicator(
          onRefresh: () async => ref.invalidate(petsProvider(null)),
          child: ListView.builder(
            itemCount: pets.length,
            itemBuilder: (_, i) {
              final pet = pets[i];
              return PetCard(
                pet: pet,
                showActions: false,
                onTap: () => context.push('${Routes.petDetail}/${pet.id}'),
              );
            },
          ),
        );
      },
    );
  }
}
