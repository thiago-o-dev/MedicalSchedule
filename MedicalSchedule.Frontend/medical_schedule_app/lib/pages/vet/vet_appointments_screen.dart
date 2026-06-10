import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../state/appointment_provider.dart';
import '../../state/vet_provider.dart';
import '../../widgets/appointment_card.dart';

class VetAppointmentsScreen extends ConsumerWidget {
  const VetAppointmentsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final vetId = ref.watch(currentVetProvider).valueOrNull?.id;
    if (vetId == null) {
      return Center(child: CircularProgressIndicator());
    }
    final appointmentsAsync = ref.watch(vetAppointmentsProvider(vetId));

    return appointmentsAsync.when(
      loading: () => Center(child: CircularProgressIndicator()),
      error: (e, _) => Center(child: Text(e.toString())),
      data: (appointments) {
        if (appointments.isEmpty) {
          return Center(child: Text('No appointments.'));
        }
        return RefreshIndicator(
          onRefresh: () async => ref.invalidate(vetAppointmentsProvider(vetId)),
          child: ListView.builder(
            itemCount: appointments.length,
            itemBuilder: (_, i) {
              final a = appointments[i];
              return AppointmentCard(
                appointment: a,
                onTap: () =>
                    context.push('${Routes.appointmentDetail}/${a.id}'),
              );
            },
          ),
        );
      },
    );
  }
}
