import 'package:dio/dio.dart';

class ApiException implements Exception {
  final int statusCode;
  final String title;
  final String detail;
  final String? exceptionType;
  final String? traceId;

  ApiException({
    required this.statusCode,
    required this.title,
    required this.detail,
    this.exceptionType,
    this.traceId,
  });

  factory ApiException.fromDioError(DioException error) {
    final response = error.response;
    if (response == null) {
      return ApiException(
        statusCode: 0,
        title: 'Network error',
        detail: 'Could not reach the server. Check your connection.',
      );
    }

    final data = response.data;
    if (data is Map<String, dynamic>) {
      // Keycloak OAuth token endpoint: { "error": "...", "error_description": "..." }
      final keycloakError = data['error'] as String?;
      final keycloakDesc = data['error_description'] as String?;
      if (keycloakError != null) {
        return ApiException(
          statusCode: response.statusCode ?? 401,
          title: keycloakError,
          detail: keycloakDesc ?? keycloakError,
        );
      }
      // Keycloak Admin API: { "errorMessage": "...", "field": "..." }
      final adminErrorMessage = data['errorMessage'] as String?;
      if (adminErrorMessage != null) {
        return ApiException(
          statusCode: response.statusCode ?? 409,
          title: 'Conflict',
          detail: adminErrorMessage,
        );
      }
      return ApiException(
        statusCode: (data['status'] as int?) ?? response.statusCode ?? 500,
        title: (data['title'] as String?) ?? 'Error',
        detail: (data['detail'] as String?) ?? response.statusMessage ?? 'Unknown error.',
        exceptionType: data['exceptionType'] as String?,
        traceId: data['traceId'] as String?,
      );
    }

    return ApiException(
      statusCode: response.statusCode ?? 500,
      title: 'Error',
      detail: response.statusMessage ?? 'Unexpected error.',
    );
  }

  @override
  String toString() => detail;
}
