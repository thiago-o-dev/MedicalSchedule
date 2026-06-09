import '../models/appointment/appointment_model.dart';
import '../services/appointment_service.dart';

class AppointmentRepository {
  final AppointmentService _service = AppointmentService();

  Future<String> scheduleAppointment({
    required String petId,
    required String vetId,
    required String ownerId,
    required DateTime scheduledAt,
    String? notes,
  }) async {
    final response = await _service.scheduleAppointment(
      petId: petId,
      vetId: vetId,
      ownerId: ownerId,
      scheduledAt: scheduledAt,
      notes: notes,
    );
    return response.data['id'] as String;
  }

  Future<List<AppointmentModel>> getAppointments({
    String? petId,
    String? vetId,
    String? ownerId,
    int? status,
  }) async {
    final response = await _service.getAppointments(
      petId: petId,
      vetId: vetId,
      ownerId: ownerId,
      status: status,
    );
    final List data = response.data as List;
    return data
        .map((e) => AppointmentModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<AppointmentModel> getAppointmentById(String id) async {
    final response = await _service.getAppointmentById(id);
    return AppointmentModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> cancelAppointment(String id) async {
    await _service.cancelAppointment(id);
  }

  Future<void> rescheduleAppointment(String id, DateTime newScheduledAt) async {
    await _service.rescheduleAppointment(id, newScheduledAt);
  }
}
