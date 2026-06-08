import 'pet_species_enum.dart';

class PetModel {
  final String? id;
  final String name;
  final PetSpecies species;
  final String breed;
  final DateTime birthDate;
  final String? primaryOwnerId;
  final bool isActive;

  PetModel({
    this.id,
    required this.name,
    required this.species,
    required this.breed,
    required this.birthDate,
    this.primaryOwnerId,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() {
    final d = birthDate;
    final dateStr =
        '${d.year.toString().padLeft(4, '0')}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';
    return {
      'name': name,
      'species': species.toBackendValue(),
      'breed': breed,
      'birthDate': dateStr,
      'primaryOwnerId': primaryOwnerId,
    };
  }

  factory PetModel.fromJson(Map<String, dynamic> json) {
    return PetModel(
      id: json['id'] as String?,
      name: json['name'] as String,
      species: PetSpecies.fromBackendValue(json['species'] as int),
      breed: json['breed'] as String,
      birthDate: DateTime.parse(json['birthDate'] as String),
      isActive: json['isActive'] as bool? ?? true,
    );
  }
}
