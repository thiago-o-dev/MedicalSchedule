import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/appointment/appointment_model.dart';
import '../repositories/appointment_repository.dart';

final appointmentRepositoryProvider = Provider<AppointmentRepository>(
  (ref) => AppointmentRepository(),
);

final appointmentsProvider =
    FutureProvider.family<List<AppointmentModel>, String?>((ref, vetId) {
  return ref
      .read(appointmentRepositoryProvider)
      .getAppointments(vetId: vetId);
});
