import 'package:dio/dio.dart';
import '../core/network/api_client.dart';

class AuthService {
  final Dio _dio = ApiClient.dio;

  Future<Response> login({
    required String email,
    required String password,
  }) async {
    return await _dio.post(
      '/login',
      data: {
        'email': email,
        'password': password,
      },
    );
  }
}