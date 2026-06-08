class LoginResponseModel {
  final String token;
  final int expiresIn;

  LoginResponseModel({required this.token, required this.expiresIn});

  factory LoginResponseModel.fromJson(Map<String, dynamic> json) {
    return LoginResponseModel(
      token: json['token'] as String,
      expiresIn: json['expiresIn'] as int,
    );
  }
}
