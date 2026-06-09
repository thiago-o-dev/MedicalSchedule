import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/appointment/appointment_model.dart';
import '../repositories/appointment_repository.dart';

final appointmentRepositoryProvider = Provider<AppointmentRepository>(
  (ref) => AppointmentRepository(),
);

final ownerAppointmentsProvider = FutureProvider.autoDispose
    .family<List<AppointmentModel>, String>((ref, ownerId) {
  return ref
      .read(appointmentRepositoryProvider)
      .getAppointments(ownerId: ownerId);
});

final vetAppointmentsProvider = FutureProvider.autoDispose
    .family<List<AppointmentModel>, String>((ref, vetId) {
  return ref
      .read(appointmentRepositoryProvider)
      .getAppointments(vetId: vetId);
});

final appointmentByIdProvider =
    FutureProvider.autoDispose.family<AppointmentModel, String>((ref, id) {
  return ref.read(appointmentRepositoryProvider).getAppointmentById(id);
});
