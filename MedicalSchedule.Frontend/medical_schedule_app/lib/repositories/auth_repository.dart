// import '../models/user_model.dart';
import '../services/auth_service.dart';

class AuthRepository {
  final AuthService _service = AuthService();

  // Future<UserModel> login(
  //   String email,
  //   String password,
  // ) async {
  //   final response = await _service.login(
  //     email: email,
  //     password: password,
  //   );

  //   return UserModel.fromJson(response.data);
  // }
}