import 'package:dio/dio.dart';

import '../config/env.dart';
import '../storage/token_storage.dart';
import 'api_exception.dart';

class ApiClient {
  static final Dio dio = Dio(
    BaseOptions(baseUrl: Env.baseUrl),
  )..interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final token = await TokenStorage.getToken();
          if (token != null) {
            print('Bearer $token');
            options.headers['Authorization'] = 'Bearer $token';
          }
          return handler.next(options);
        },
        onError: (error, handler) async {
          if (error.response?.statusCode == 401) {
            //await TokenStorage.clear();
          }

          final apiException = ApiException.fromDioError(error);
          return handler.reject(
            DioException(
              requestOptions: error.requestOptions,
              response: error.response,
              type: error.type,
              error: apiException,
              message: apiException.detail,
            ),
          );
        },
      ),
    );
}
