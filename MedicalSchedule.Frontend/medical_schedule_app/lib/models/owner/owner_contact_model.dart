class OwnerContactModel {
  final String name;
  final String email;

  OwnerContactModel({required this.name, required this.email});

  factory OwnerContactModel.fromJson(Map<String, dynamic> json) {
    return OwnerContactModel(
      name: json['name'] as String,
      email: json['email'] as String,
    );
  }
}
