import 'package:dio/dio.dart';

import '../core/network/api_client.dart';

class AppointmentService {
  final Dio _dio = ApiClient.dio;

  Future<Response> scheduleAppointment({
    required String petId,
    required String vetId,
    required DateTime date,
  }) async {
    return await _dio.post(
      '/api/consultations/appointments',
      data: {
        'petId': petId,
        'vetId': vetId,
        'date': date.toIso8601String(),
      },
    );
  }

  Future<Response> getAppointments({
    String? petId,
    String? vetId,
  }) async {
    return await _dio.get(
      '/api/consultations/appointments',
      queryParameters: {
        'petId': petId,
        'vetId': vetId,
      },
    );
  }
}