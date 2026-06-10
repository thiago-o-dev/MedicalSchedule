import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../models/owner/owner_model.dart';
import '../../models/pet/pet_deletion_status_enum.dart';
import '../../models/pet/pet_model.dart';
import '../../state/owner_provider.dart';
import '../../state/pet_provider.dart';
import '../../widgets/api_error_snackbar.dart';
import '../../widgets/confirm_dialog.dart';
import '../../widgets/owner_badge.dart';
import '../../widgets/pet_deletion_dialog.dart';
import '../../widgets/saga_status_chip.dart';

class PetDetailScreen extends ConsumerWidget {
  final String petId;

  PetDetailScreen({super.key, required this.petId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final petAsync = ref.watch(petByIdProvider(petId));
    final primaryOwnerAsync = ref.watch(petOwnerProvider(petId));
    final allOwnersAsync = ref.watch(ownersProvider);

    return Scaffold(
      appBar: AppBar(title: Text('Pet Details')),
      body: petAsync.when(
        loading: () => Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text(e.toString())),
        data: (pet) => RefreshIndicator(
          onRefresh: () async => ref.invalidate(petByIdProvider(petId)),
          child: ListView(
            padding: EdgeInsets.all(16),
            children: [
              _PetHeader(pet: pet),
              SizedBox(height: 16),
              _DetailsCard(pet: pet),
              SizedBox(height: 16),
              Text(
                'Primary owner',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              SizedBox(height: 8),
              primaryOwnerAsync.when(
                loading: () => LinearProgressIndicator(),
                error: (e, _) => Text(e.toString()),
                data: (owner) => owner == null
                    ? Text('No primary owner assigned.')
                    : OwnerBadge(
                        name: owner.name,
                        email: owner.email,
                        isPrimary: true,
                      ),
              ),
              SizedBox(height: 16),
              Row(
                children: [
                  Text(
                    'Co-owners',
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  Spacer(),
                  TextButton.icon(
                    onPressed: pet.isActive
                        ? () => _showAddCoOwner(context, ref, pet, allOwnersAsync)
                        : null,
                    icon: Icon(Icons.add, size: 16),
                    label: Text('Add'),
                  ),
                ],
              ),
              _CoOwnersList(
                pet: pet,
                allOwnersAsync: allOwnersAsync,
                onRemove: (ownerId) async {
                  final ok = await showConfirmDialog(
                    context,
                    title: 'Remove co-owner?',
                    message: 'This will detach the owner from this pet.',
                    destructive: true,
                    confirmLabel: 'Remove',
                  );
                  if (!ok || !context.mounted) return;
                  try {
                    await ref
                        .read(petRepositoryProvider)
                        .removePetOwner(petId, ownerId);
                    ref.invalidate(petByIdProvider(petId));
                    if (context.mounted) {
                      showSuccessSnackBar(context, 'Co-owner removed.');
                    }
                  } catch (e) {
                    if (context.mounted) showApiErrorSnackBar(context, e);
                  }
                },
              ),
              SizedBox(height: 24),
              if (pet.isActive &&
                  pet.deletionStatus != PetDeletionStatus.pendingDeletion)
                ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.red.shade600,
                    foregroundColor: Colors.white,
                  ),
                  onPressed: () => _requestDeletion(context, ref, pet),
                  icon: Icon(Icons.delete_forever),
                  label: Text('Request pet deletion'),
                ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _requestDeletion(
    BuildContext context,
    WidgetRef ref,
    PetModel pet,
  ) async {
    final deleted = await showPetDeletionDialog(context, pet);
    if (!context.mounted) return;

    ref.invalidate(petByIdProvider(petId));
    ref.invalidate(petsProvider(pet.primaryOwnerId));

    if (deleted) {
      showSuccessSnackBar(context, 'Pet deleted.');
      if (context.mounted) Navigator.of(context).pop();
    }
  }

  Future<void> _showAddCoOwner(
    BuildContext context,
    WidgetRef ref,
    PetModel pet,
    AsyncValue<List<OwnerModel>> allOwnersAsync,
  ) async {
    final owners = allOwnersAsync.valueOrNull ?? <OwnerModel>[];
    final existingIds = pet.ownerships.map((o) => o.ownerId).toSet();
    final candidates =
        owners.where((o) => o.id != null && !existingIds.contains(o.id)).toList();

    if (candidates.isEmpty) {
      showApiErrorSnackBar(context, 'No available owners to add.');
      return;
    }

    final selected = await showDialog<OwnerModel>(
      context: context,
      builder: (ctx) => SimpleDialog(
        title: Text('Add co-owner'),
        children: candidates
            .map(
              (o) => SimpleDialogOption(
                onPressed: () => Navigator.pop(ctx, o),
                child: Text('${o.name} (${o.email})'),
              ),
            )
            .toList(),
      ),
    );

    if (selected == null || !context.mounted) return;

    try {
      await ref
          .read(petRepositoryProvider)
          .addPetOwner(pet.id!, ownerId: selected.id!);
      ref.invalidate(petByIdProvider(petId));
      if (context.mounted) showSuccessSnackBar(context, 'Co-owner added.');
    } catch (e) {
      if (context.mounted) showApiErrorSnackBar(context, e);
    }
  }
}

class _PetHeader extends StatelessWidget {
  final PetModel pet;
  _PetHeader({required this.pet});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Hero(
          tag: 'pet-${pet.id ?? pet.name}',
          child: CircleAvatar(
            backgroundColor: Colors.blue.shade50,
            radius: 36,
            child: Icon(Icons.pets, color: Colors.blue, size: 32),
          ),
        ),
        SizedBox(width: 16),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                pet.name,
                style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
              SizedBox(height: 4),
              Text(
                '${pet.species.displayName} • ${pet.breed}',
                style: TextStyle(color: Colors.grey.shade700),
              ),
              SizedBox(height: 8),
              SagaStatusChip(
                status: pet.deletionStatus,
                rejectionReason: pet.deletionRejectionReason,
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _DetailsCard extends StatelessWidget {
  final PetModel pet;
  _DetailsCard({required this.pet});

  @override
  Widget build(BuildContext context) {
    final d = pet.birthDate;
    final birthStr =
        '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
    return Card(
      child: Padding(
        padding: EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _row(Icons.cake, 'Birth date', birthStr),
            Divider(),
            _row(
              pet.isActive ? Icons.check_circle : Icons.cancel,
              'Status',
              pet.isActive ? 'Active' : 'Inactive',
            ),
          ],
        ),
      ),
    );
  }

  Widget _row(IconData icon, String label, String value) => Padding(
        padding: EdgeInsets.symmetric(vertical: 6),
        child: Row(
          children: [
            Icon(icon, size: 18, color: Colors.blue.shade700),
            SizedBox(width: 8),
            Text(label, style: TextStyle(fontWeight: FontWeight.w500)),
            Spacer(),
            Text(value, style: TextStyle(color: Colors.grey.shade800)),
          ],
        ),
      );
}

class _CoOwnersList extends ConsumerWidget {
  final PetModel pet;
  final AsyncValue<List<OwnerModel>> allOwnersAsync;
  final ValueChanged<String> onRemove;

  _CoOwnersList({
    required this.pet,
    required this.allOwnersAsync,
    required this.onRemove,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final coOwnerIds = pet.ownerships
        .where((o) => !o.isPrimaryOwner)
        .map((o) => o.ownerId)
        .toList();

    if (coOwnerIds.isEmpty) {
      return Padding(
        padding: EdgeInsets.symmetric(vertical: 8),
        child: Text(
          'No co-owners yet.',
          style: TextStyle(color: Colors.grey.shade600),
        ),
      );
    }

    return allOwnersAsync.when(
      loading: () => Padding(
        padding: EdgeInsets.symmetric(vertical: 8),
        child: LinearProgressIndicator(),
      ),
      error: (e, _) => Text(e.toString()),
      data: (allOwners) => Column(
        children: coOwnerIds.map((id) {
          final owner = allOwners.firstWhere(
            (o) => o.id == id,
            orElse: () => OwnerModel(
              id: id,
              name: 'Unknown owner',
              cpf: '',
              phone: '',
              email: '—',
            ),
          );
          return OwnerBadge(
            name: owner.name,
            email: owner.email,
            isPrimary: false,
            onRemove: () => onRemove(id),
          );
        }).toList(),
      ),
    );
  }
}
