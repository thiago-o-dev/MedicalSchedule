class OwnerModel {
  final String? id;
  final String name;
  final String cpf;
  final String phone;
  final String email;

  OwnerModel({
    this.id,
    required this.name,
    required this.cpf,
    required this.phone,
    required this.email,
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
      id: json['id'],
      name: json['name'],
      cpf: json['cpf'],
      phone: json['phone'],
      email: json['email'],
    );
  }
}