class OwnerModel {
  final String? id;
  final String name;
  final String cpf;
  final String phone;
  final String email;
  final bool isActive;

  OwnerModel({
    this.id,
    required this.name,
    required this.cpf,
    required this.phone,
    required this.email,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'cpf': cpf,
      'phone': phone,
      'email': email,
    };
  }

  factory OwnerModel.fromJson(Map<String, dynamic> json) {
    return OwnerModel(
      id: json['id'] as String?,
      name: json['name'] as String,
      cpf: json['cpf'] as String,
      phone: json['phone'] as String,
      email: json['email'] as String,
      isActive: json['isActive'] as bool? ?? true,
    );
  }
}
