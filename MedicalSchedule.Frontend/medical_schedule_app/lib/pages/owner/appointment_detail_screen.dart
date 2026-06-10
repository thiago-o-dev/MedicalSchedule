import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:medical_schedule_app/widgets/appointment_details.dart';

import '../../state/appointment_provider.dart';
import '../../state/pet_provider.dart';
import '../../state/vet_provider.dart';

class AppointmentDetailScreen extends ConsumerWidget {
  final String appointmentId;

  const AppointmentDetailScreen({super.key, required this.appointmentId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final appointmentAsync = ref.watch(appointmentByIdProvider(appointmentId));

    return Scaffold(
      appBar: AppBar(title: Text('Appointment Details')),
      body: appointmentAsync.when(
        loading: () => Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text(e.toString())),
        data: (appointment) {
          final petAsync = ref.watch(petByIdProvider(appointment.petId));
          final vetAsync = ref.watch(vetByIdProvider(appointment.vetId));

          return AppointmentDetails(appointment: appointment, petAsync: petAsync, vetAsync: vetAsync);
        },
      ),
    );
  }
}
