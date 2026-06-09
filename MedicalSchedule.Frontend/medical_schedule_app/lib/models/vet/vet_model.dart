class VetModel {
  final String? id;
  final String name;
  final String crm;
  final String specialty;
  final String email;
  final bool isActive;

  VetModel({
    this.id,
    required this.name,
    required this.crm,
    required this.specialty,
    required this.email,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'crm': crm,
      'specialty': specialty,
      'email': email,
    };
  }

  factory VetModel.fromJson(Map<String, dynamic> json) {
    return VetModel(
      id: json['id'] as String?,
      name: json['name'] as String,
      crm: json['crm'] as String,
      specialty: json['specialty'] as String,
      email: json['email'] as String,
      isActive: json['isActive'] as bool? ?? true,
    );
  }
}
