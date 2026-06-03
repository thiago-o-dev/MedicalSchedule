import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../models/auth/login_request_model.dart';

class AuthService {
  final Dio _dio = ApiClient.dio;

  Future<Response> login(
    LoginRequestModel request,
  ) async {
    return await _dio.post(
      '/api/auth/token',
      data: request.toJson(),
    );
  }
}