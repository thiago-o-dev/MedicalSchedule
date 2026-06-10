import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../models/pet/pet_model.dart';
import '../../models/vet/vet_model.dart';
import '../../state/appointment_provider.dart';
import '../../state/owner_provider.dart';
import '../../state/pet_provider.dart';
import '../../state/vet_provider.dart';
import '../../widgets/api_error_snackbar.dart';
import '../../widgets/vet_carousel_card.dart';

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
  final carouselController = CarouselController();
  bool loading = false;

  @override
  void dispose() {
    notesCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now().add(Duration(days: 1)),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(Duration(days: 365)),
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
      showApiErrorSnackBar(context, 'Please fill in all required fields.');
      return;
    }

    final scheduledAt = DateTime(
      selectedDate!.year,
      selectedDate!.month,
      selectedDate!.day,
      selectedTime!.hour,
      selectedTime!.minute,
    );

    final ownerId = ref.read(currentOwnerProvider).valueOrNull?.id;
    if (ownerId == null) {
      showApiErrorSnackBar(context, 'Owner not loaded yet.');
      return;
    }

    setState(() => loading = true);
    try {
      await ref.read(appointmentRepositoryProvider).scheduleAppointment(
            petId: selectedPet!.id!,
            vetId: selectedVet!.id!,
            ownerId: ownerId,
            scheduledAt: scheduledAt,
            notes: notesCtrl.text.isNotEmpty ? notesCtrl.text : null,
          );
      ref.invalidate(ownerAppointmentsProvider(ownerId));
      if (mounted) {
        showSuccessSnackBar(context, 'Appointment scheduled!');
        setState(() {
          selectedVet = null;
          selectedPet = null;
          selectedDate = null;
          selectedTime = null;
          notesCtrl.clear();
        });
      }
    } catch (e) {
      if (mounted) showApiErrorSnackBar(context, e);
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
    
    return SingleChildScrollView(
      padding: EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            'Veterinarian *',
            style: Theme.of(context).textTheme.titleSmall,
          ),
          SizedBox(height: 8),
          SizedBox(
            height: 160,
            child: vetsAsync.when(
              loading: () =>
                  Center(child: CircularProgressIndicator()),
              error: (e, _) => Text(e.toString()),
              data: (vets) {
                if (vets.isEmpty) {
                  return Center(child: Text('No veterinarians available.'));
                }
                return CarouselView.weighted(
                  flexWeights: [1,2,1],
                  controller: carouselController,
                  shrinkExtent: 140,
                  scrollDirection: Axis.horizontal,
                  onTap: (i) => 
                    setState(() {
                      carouselController.animateToItem(i, duration: const Duration(milliseconds: 500), curve: Curves.easeInOut);
                      selectedVet = vets[i];
                    }),
                  children: vets
                      .map(
                        (v) => VetCarouselCard(
                          vet: v,
                          selected: selectedVet?.id == v.id,
                        ),
                      )
                      .toList(),
                );
              },
            ),
          ),
          if (selectedVet != null)
            Padding(
              padding: EdgeInsets.only(top: 6),
              child: Text(
                'Selected: ${selectedVet!.name}',
                style: TextStyle(
                  color: Colors.blue.shade700,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          SizedBox(height: 20),
          petsAsync.when(
            loading: () =>
                Center(child: CircularProgressIndicator()),
            error: (e, _) => Text(e.toString()),
            data: (pets) => DropdownButtonFormField<PetModel>(
              value: selectedPet,
              decoration: InputDecoration(labelText: 'Pet *'),
              items: pets
                  .where((p) => p.isActive)
                  .map(
                    (p) => DropdownMenuItem(
                      value: p,
                      child: Text('${p.name} (${p.species.displayName})'),
                    ),
                  )
                  .toList(),
              onChanged: (p) => setState(() => selectedPet = p),
            ),
          ),
          SizedBox(height: 8),
          ListTile(
            contentPadding: EdgeInsets.zero,
            title: Text(dateLabel),
            trailing: Icon(Icons.calendar_today),
            onTap: _pickDate,
          ),
          ListTile(
            contentPadding: EdgeInsets.zero,
            title: Text(timeLabel),
            trailing: Icon(Icons.access_time),
            onTap: _pickTime,
          ),
          SizedBox(height: 8),
          TextField(
            controller: notesCtrl,
            decoration: InputDecoration(labelText: 'Notes (optional)'),
            maxLines: 3,
          ),
          SizedBox(height: 24),
          ElevatedButton(
            onPressed: loading ? null : _submit,
            child: loading
                ? SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : Text('Schedule'),
          ),
        ],
      ),
    );
  }
}
