import '../core/storage/token_storage.dart';
import '../models/auth/login_request_model.dart';
import '../models/auth/login_response_model.dart';
import '../services/auth_service.dart';

class AuthRepository {
  final AuthService _service = AuthService();

  Future<LoginResponseModel> login({
    required String email,
    required String password,
  }) async {
    final request = LoginRequestModel(
      email: email,
      password: password,
    );

    final response = await _service.login(request);

    final model = LoginResponseModel.fromJson(
      response.data,
    );

    await TokenStorage.saveToken(model.token);

    return model;
  }

  Future<void> logout() async {
    await TokenStorage.clear();
  }

  Future<bool> isAuthenticated() async {
    final token = await TokenStorage.getToken();

    return token != null;
  }
}