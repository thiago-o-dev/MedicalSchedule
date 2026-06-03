import '../models/appointment/appointment_model.dart';
import '../services/appointment_service.dart';

class AppointmentRepository {
  final AppointmentService _service =
      AppointmentService();

  Future<void> scheduleAppointment({
    required String petId,
    required String vetId,
    required DateTime date,
  }) async {
    await _service.scheduleAppointment(
      petId: petId,
      vetId: vetId,
      date: date,
    );
  }

  Future<List<AppointmentModel>>
      getAppointments({
    String? petId,
    String? vetId,
  }) async {
    final response = await _service.getAppointments(
      petId: petId,
      vetId: vetId,
    );

    final List data = response.data;

    return data
        .map(
          (e) => AppointmentModel.fromJson(e),
        )
        .toList();
  }
}