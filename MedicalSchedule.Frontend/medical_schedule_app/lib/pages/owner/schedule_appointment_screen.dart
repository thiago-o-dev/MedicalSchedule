import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../core/storage/token_storage.dart';
import '../../models/pet/pet_model.dart';
import '../../models/vet/vet_model.dart';
import '../../state/appointment_provider.dart';
import '../../state/owner_provider.dart';
import '../../state/pet_provider.dart';
import '../../state/vet_provider.dart';

class ScheduleAppointmentScreen extends ConsumerStatefulWidget {
  const ScheduleAppointmentScreen({super.key});

  @override
  ConsumerState<ScheduleAppointmentScreen> createState() =>
      _ScheduleAppointmentScreenState();
}

class _ScheduleAppointmentScreenState
    extends ConsumerState<ScheduleAppointmentScreen> {
  VetModel? selectedVet;
  PetModel? selectedPet;
  DateTime? selectedDate;
  TimeOfDay? selectedTime;
  final notesCtrl = TextEditingController();
  bool loading = false;

  @override
  void dispose() {
    notesCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now().add(const Duration(days: 1)),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (picked != null) setState(() => selectedDate = picked);
  }

  Future<void> _pickTime() async {
    final picked = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );
    if (picked != null) setState(() => selectedTime = picked);
  }

  Future<void> _submit() async {
    if (selectedVet == null ||
        selectedPet == null ||
        selectedDate == null ||
        selectedTime == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please fill in all required fields.')),
      );
      return;
    }

    final scheduledAt = DateTime(
      selectedDate!.year,
      selectedDate!.month,
      selectedDate!.day,
      selectedTime!.hour,
      selectedTime!.minute,
    );

    setState(() => loading = true);
    try {
      await ref.read(appointmentRepositoryProvider).scheduleAppointment(
            petId: selectedPet!.id!,
            vetId: selectedVet!.id!,
            scheduledAt: scheduledAt,
            notes: notesCtrl.text.isNotEmpty ? notesCtrl.text : null,
          );
      ref.invalidate(appointmentsProvider(null));
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Appointment scheduled!')),
        );
        setState(() {
          selectedVet = null;
          selectedPet = null;
          selectedDate = null;
          selectedTime = null;
          notesCtrl.clear();
        });
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error: $e')),
        );
      }
    }
    if (mounted) setState(() => loading = false);
  }

  @override
  Widget build(BuildContext context) {
    final vetsAsync = ref.watch(vetsProvider);
    final ownerId = ref.watch(currentOwnerProvider).valueOrNull?.id;
    final petsAsync = ref.watch(petsProvider(ownerId));

    final d = selectedDate;
    final t = selectedTime;
    final dateLabel = d == null
        ? 'Pick date *'
        : '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
    final timeLabel = t == null ? 'Pick time *' : t.format(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Schedule Appointment'),
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
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            vetsAsync.when(
              loading: () =>
                  const Center(child: CircularProgressIndicator()),
              error: (e, _) => Text('Error loading vets: $e'),
              data: (vets) => DropdownButtonFormField<VetModel>(
                value: selectedVet,
                decoration: const InputDecoration(
                  labelText: 'Veterinarian *',
                ),
                items: vets
                    .map(
                      (v) => DropdownMenuItem(
                        value: v,
                        child: Text('${v.name} (${v.specialty})'),
                      ),
                    )
                    .toList(),
                onChanged: (v) => setState(() => selectedVet = v),
              ),
            ),
            const SizedBox(height: 16),
            petsAsync.when(
              loading: () =>
                  const Center(child: CircularProgressIndicator()),
              error: (e, _) => Text('Error loading pets: $e'),
              data: (pets) => DropdownButtonFormField<PetModel>(
                value: selectedPet,
                decoration:
                    const InputDecoration(labelText: 'Pet *'),
                items: pets
                    .map(
                      (p) => DropdownMenuItem(
                        value: p,
                        child: Text(
                          '${p.name} (${p.species.displayName})',
                        ),
                      ),
                    )
                    .toList(),
                onChanged: (p) => setState(() => selectedPet = p),
              ),
            ),
            const SizedBox(height: 8),
            ListTile(
              contentPadding: EdgeInsets.zero,
              title: Text(dateLabel),
              trailing: const Icon(Icons.calendar_today),
              onTap: _pickDate,
            ),
            ListTile(
              contentPadding: EdgeInsets.zero,
              title: Text(timeLabel),
              trailing: const Icon(Icons.access_time),
              onTap: _pickTime,
            ),
            const SizedBox(height: 8),
            TextField(
              controller: notesCtrl,
              decoration: const InputDecoration(
                labelText: 'Notes (optional)',
              ),
              maxLines: 3,
            ),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: loading ? null : _submit,
              child: loading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Schedule'),
            ),
          ],
        ),
      ),
    );
  }
}
