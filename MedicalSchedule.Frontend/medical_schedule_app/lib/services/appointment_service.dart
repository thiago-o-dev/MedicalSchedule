import 'package:dio/dio.dart';

import '../core/network/api_client.dart';

class AppointmentService {
  final Dio _dio = ApiClient.dio;

  Future<Response> scheduleAppointment({
    required String petId,
    required String vetId,
    required DateTime scheduledAt,
    String? notes,
  }) async {
    return await _dio.post(
      '/api/consultations',
      data: {
        'petId': petId,
        'vetId': vetId,
        'scheduledAt': scheduledAt.toIso8601String(),
        'notes': notes,
      },
    );
  }

  Future<Response> getAppointments({
    String? petId,
    String? vetId,
    int? status,
  }) async {
    return await _dio.get(
      '/api/consultations',
      queryParameters: {
        if (petId != null) 'petId': petId,
        if (vetId != null) 'vetId': vetId,
        if (status != null) 'status': status,
      },
    );
  }

  Future<Response> getAppointmentById(String id) async {
    return await _dio.get('/api/consultations/$id');
  }

  Future<Response> cancelAppointment(String id) async {
    return await _dio.delete('/api/consultations/$id');
  }

  Future<Response> rescheduleAppointment(
    String id,
    DateTime newScheduledAt,
  ) async {
    return await _dio.patch(
      '/api/consultations/$id/reschedule',
      data: {'newScheduledAt': newScheduledAt.toIso8601String()},
    );
  }
}
