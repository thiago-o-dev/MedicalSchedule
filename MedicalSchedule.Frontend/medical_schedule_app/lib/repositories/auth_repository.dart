import '../core/storage/token_storage.dart';
import '../models/auth/login_request_model.dart';
import '../models/auth/login_response_model.dart';
import '../models/auth/register_request_model.dart';
import '../services/auth_service.dart';

class AuthRepository {
  final AuthService _service = AuthService();

  Future<LoginResponseModel> login({
    required String email,
    required String password,
  }) async {
    final response = await _service.login(
      LoginRequestModel(email: email, password: password),
    );
    final model = LoginResponseModel.fromJson(
      response.data as Map<String, dynamic>,
    );
    await TokenStorage.saveToken(model.token);
    await TokenStorage.saveEmail(email);
    return model;
  }

  Future<LoginResponseModel> register({
    required String name,
    required String document,
    required String phone,
    required String email,
    required String password,
    required bool isOwner,
    String? specialty,
  }) async {
    final response = await _service.register(
      RegisterRequestModel(
        name: name,
        document: document,
        phone: phone,
        email: email,
        password: password,
        isOwner: isOwner,
        specialty: specialty,
      ),
    );
    final model = LoginResponseModel.fromJson(
      response.data as Map<String, dynamic>,
    );
    await TokenStorage.saveToken(model.token);
    await TokenStorage.saveEmail(email);
    return model;
  }

  Future<void> logout() async {
    await TokenStorage.clear();
  }

  Future<bool> isAuthenticated() async {
    return await TokenStorage.getToken() != null;
  }
}
