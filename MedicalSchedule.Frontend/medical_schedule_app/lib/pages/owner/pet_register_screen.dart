import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../models/pet/pet_model.dart';
import '../../models/pet/pet_species_enum.dart';
import '../../state/owner_provider.dart';
import '../../state/pet_provider.dart';
import '../../widgets/api_error_snackbar.dart';
import '../../widgets/pet_card.dart';
import '../../widgets/pet_deletion_dialog.dart';

class PetRegisterScreen extends ConsumerWidget {
  const PetRegisterScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final currentOwnerAsync = ref.watch(currentOwnerProvider);
    final ownerId = currentOwnerAsync.valueOrNull?.id;
    final petsAsync = ref.watch(petsProvider(ownerId));

    return Scaffold(
      floatingActionButton: currentOwnerAsync.when(
        loading: () => null,
        error: (_, __) => null,
        data: (owner) => owner == null
            ? null
            : FloatingActionButton(
                onPressed: () => _showAddPetDialog(context, ref, owner.id!),
                child: Icon(Icons.add),
              ),
      ),
      body: petsAsync.when(
        loading: () => Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text(e.toString())),
        data: (pets) {
          if (pets.isEmpty) {
            return Center(
              child: Text('No pets registered. Tap + to add one!'),
            );
          }
          return RefreshIndicator(
            onRefresh: () async => ref.invalidate(petsProvider(ownerId)),
            child: ListView.builder(
              itemCount: pets.length,
              itemBuilder: (_, i) {
                final pet = pets[i];
                return PetCard(
                  pet: pet,
                  onTap: () => context.push('${Routes.petDetail}/${pet.id}'),
                  onDelete: () => _requestDeletion(context, ref, pet, ownerId),
                );
              },
            ),
          );
        },
      ),
    );
  }

  Future<void> _requestDeletion(
    BuildContext context,
    WidgetRef ref,
    PetModel pet,
    String? ownerId,
  ) async {
    final deleted = await showPetDeletionDialog(context, pet);
    if (!context.mounted) return;

    ref.invalidate(petsProvider(ownerId));

    if (deleted) {
      showSuccessSnackBar(context, 'Pet deleted.');
    }
  }

  void _showAddPetDialog(BuildContext context, WidgetRef ref, String ownerId) {
    showDialog(
      context: context,
      builder: (ctx) => _AddPetDialog(
        ownerId: ownerId,
        onSaved: () => ref.invalidate(petsProvider(ownerId)),
      ),
    );
  }
}

class _AddPetDialog extends ConsumerStatefulWidget {
  final String ownerId;
  final VoidCallback onSaved;

  const _AddPetDialog({required this.ownerId, required this.onSaved});

  @override
  ConsumerState<_AddPetDialog> createState() => _AddPetDialogState();
}

class _AddPetDialogState extends ConsumerState<_AddPetDialog> {
  final nameCtrl = TextEditingController();
  final breedCtrl = TextEditingController();
  PetSpecies selectedSpecies = PetSpecies.dog;
  DateTime? selectedBirthDate;
  bool loading = false;

  @override
  void dispose() {
    nameCtrl.dispose();
    breedCtrl.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (nameCtrl.text.isEmpty ||
        breedCtrl.text.isEmpty ||
        selectedBirthDate == null) {
      showApiErrorSnackBar(context, 'Please fill in all fields.');
      return;
    }

    setState(() => loading = true);
    try {
      await ref.read(petRepositoryProvider).createPet(
            PetModel(
              name: nameCtrl.text,
              species: selectedSpecies,
              breed: breedCtrl.text,
              birthDate: selectedBirthDate!,
              primaryOwnerId: widget.ownerId,
            ),
          );
      widget.onSaved();
      if (mounted) {
        Navigator.pop(context);
        showSuccessSnackBar(context, 'Pet registered!');
      }
    } catch (e) {
      setState(() => loading = false);
      if (mounted) showApiErrorSnackBar(context, e);
    }
  }

  @override
  Widget build(BuildContext context) {
    final d = selectedBirthDate;
    final birthLabel = d == null
        ? 'Pick birth date *'
        : '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';

    return AlertDialog(
      title: Text('Add Pet'),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameCtrl,
              decoration: InputDecoration(labelText: 'Name *'),
            ),
            TextField(
              controller: breedCtrl,
              decoration: InputDecoration(labelText: 'Breed *'),
            ),
            SizedBox(height: 12),
            DropdownButtonFormField<PetSpecies>(
              value: selectedSpecies,
              decoration: InputDecoration(labelText: 'Species'),
              items: PetSpecies.values
                  .map(
                    (s) => DropdownMenuItem(
                      value: s,
                      child: Text(s.displayName),
                    ),
                  )
                  .toList(),
              onChanged: (v) => setState(() => selectedSpecies = v!),
            ),
            SizedBox(height: 8),
            ListTile(
              contentPadding: EdgeInsets.zero,
              title: Text(birthLabel),
              trailing: Icon(Icons.calendar_today),
              onTap: () async {
                final picked = await showDatePicker(
                  context: context,
                  initialDate: DateTime(2020),
                  firstDate: DateTime(2000),
                  lastDate: DateTime.now(),
                );
                if (picked != null) {
                  setState(() => selectedBirthDate = picked);
                }
              },
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: loading ? null : () => Navigator.pop(context),
          child: Text('Cancel'),
        ),
        TextButton(
          onPressed: loading ? null : _save,
          child: loading
              ? SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Text('Save'),
        ),
      ],
    );
  }
}
