class VetModel {
  final String? id;
  final String name;
  final String crmv;
  final String phone;
  final String email;

  VetModel({
    this.id,
    required this.name,
    required this.crmv,
    required this.phone,
    required this.email,
  });

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'crmv': crmv,
      'phone': phone,
      'email': email,
    };
  }

  factory VetModel.fromJson(Map<String, dynamic> json) {
    return VetModel(
      id: json['id'],
      name: json['name'],
      crmv: json['crmv'],
      phone: json['phone'],
      email: json['email'],
    );
  }
}