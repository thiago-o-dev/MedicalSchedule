import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../core/storage/token_storage.dart';
import '../../state/pet_provider.dart';

class RelatedPetsScreen extends ConsumerWidget {
  const RelatedPetsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final petsAsync = ref.watch(petsProvider(null));

    return Scaffold(
      appBar: AppBar(
        title: const Text('Pets'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Logout',
            onPressed: () async {
              await TokenStorage.clear();
              if (context.mounted) context.go(Routes.login);
            },
          ),
        ],
      ),
      body: petsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text('Error: $e')),
        data: (pets) {
          if (pets.isEmpty) {
            return const Center(child: Text('No pets registered.'));
          }
          return ListView.builder(
            itemCount: pets.length,
            itemBuilder: (_, i) {
              final pet = pets[i];
              return Card(
                margin: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 6,
                ),
                child: ListTile(
                  leading: const Icon(Icons.pets),
                  title: Text(pet.name),
                  subtitle: Text(
                    '${pet.species.displayName} • ${pet.breed}',
                  ),
                  trailing: pet.isActive
                      ? null
                      : const Chip(label: Text('Inactive')),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
