import 'package:dio/dio.dart';

import '../config/env.dart';
import '../storage/token_storage.dart';

// precisamos disso pra alimentar os tokens pros nossos requests usando o dio
class ApiClient {
  static final Dio dio = Dio(
    BaseOptions(
      baseUrl: Env.baseUrl,
    ),
  )..interceptors.add(
      InterceptorsWrapper(
        onRequest: (
          options,
          handler,
        ) async {
          final token =
              await TokenStorage.getToken();

          if (token != null) {
            options.headers['Authorization'] =
                'Bearer $token';
          }

          return handler.next(options);
        },

        onError: (
          error,
          handler,
        ) async {
          if (error.response?.statusCode ==
              401) {
            await TokenStorage.clear();
          }

          return handler.next(error);
        },
      ),
    );
}