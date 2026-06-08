class RegisterRequestModel {
  final String name;
  final String document;
  final String phone;
  final String email;
  final String password;
  final bool isOwner;
  final String? specialty;

  RegisterRequestModel({
    required this.name,
    required this.document,
    required this.phone,
    required this.email,
    required this.password,
    required this.isOwner,
    this.specialty,
  });

  Map<String, dynamic> toJson() => {
        'name': name,
        'document': document,
        'phone': phone,
        'email': email,
        'password': password,
        'isOwner': isOwner,
        'specialty': specialty,
      };
}
