import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../state/appointment_provider.dart';
import '../../state/owner_provider.dart';
import '../../widgets/api_error_snackbar.dart';
import '../../widgets/appointment_card.dart';
import '../../widgets/confirm_dialog.dart';

class AppointmentsScreen extends ConsumerWidget {
  const AppointmentsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final ownerId = ref.watch(currentOwnerProvider).valueOrNull?.id;
    if (ownerId == null) {
      return Center(child: CircularProgressIndicator());
    }
    final appointmentsAsync = ref.watch(ownerAppointmentsProvider(ownerId));

    return Scaffold(
      body: appointmentsAsync.when(
        loading: () => Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text(e.toString())),
        data: (appointments) {
          if (appointments.isEmpty) {
            return Center(child: Text('No appointments.'));
          }
          return RefreshIndicator(
            onRefresh: () async =>
                ref.invalidate(ownerAppointmentsProvider(ownerId)),
            child: ListView.builder(
              itemCount: appointments.length,
              itemBuilder: (_, i) {
                final a = appointments[i];
                return AppointmentCard(
                  appointment: a,
                  onTap: () =>
                      context.push('${Routes.appointmentDetail}/${a.id}'),
                  onCancel: () => _cancel(context, ref, a.id, ownerId),
                  onReschedule: () => _reschedule(context, ref, a.id, ownerId),
                );
              },
            ),
          );
        },
      ),
    );
  }

  Future<void> _cancel(
    BuildContext context,
    WidgetRef ref,
    String id,
    String ownerId,
  ) async {
    final confirmed = await showConfirmDialog(
      context,
      title: 'Cancel appointment?',
      message: 'This action cannot be undone.',
      destructive: true,
      confirmLabel: 'Yes, cancel',
      cancelLabel: 'No',
    );
    if (!confirmed) return;

    try {
      await ref.read(appointmentRepositoryProvider).cancelAppointment(id);
      ref.invalidate(ownerAppointmentsProvider(ownerId));
      if (context.mounted) showSuccessSnackBar(context, 'Appointment cancelled.');
    } catch (e) {
      if (context.mounted) showApiErrorSnackBar(context, e);
    }
  }

  Future<void> _reschedule(
    BuildContext context,
    WidgetRef ref,
    String id,
    String ownerId,
  ) async {
    final date = await showDatePicker(
      context: context,
      initialDate: DateTime.now().add(Duration(days: 1)),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(Duration(days: 365)),
    );
    if (date == null || !context.mounted) return;

    final time = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );
    if (time == null) return;

    final newDate = DateTime(
      date.year,
      date.month,
      date.day,
      time.hour,
      time.minute,
    );

    try {
      await ref
          .read(appointmentRepositoryProvider)
          .rescheduleAppointment(id, newDate);
      ref.invalidate(ownerAppointmentsProvider(ownerId));
      if (context.mounted) showSuccessSnackBar(context, 'Appointment rescheduled.');
    } catch (e) {
      if (context.mounted) showApiErrorSnackBar(context, e);
    }
  }
}
