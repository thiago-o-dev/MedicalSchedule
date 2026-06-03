import 'pet_species_enum.dart';

class PetModel {
  final String id;
  final String name;
  final PetSpecies species;
  final String breed;
  final DateTime birthDate;
  final String ownerId;

  PetModel({
    required this.id,
    required this.name,
    required this.species,
    required this.breed,
    required this.birthDate,
    required this.ownerId,
  });

  factory PetModel.fromJson(Map<String, dynamic> json) {
    return PetModel(
      id: json['id'],
      name: json['name'],
      species: PetSpecies.values[json['species']],
      breed: json['breed'],
      birthDate: DateTime.parse(json['birthDate']),
      ownerId: json['ownerId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'species': species.index,
      'breed': breed,
      'birthDate': birthDate.toIso8601String(),
      'ownerId': ownerId,
    };
  }
}