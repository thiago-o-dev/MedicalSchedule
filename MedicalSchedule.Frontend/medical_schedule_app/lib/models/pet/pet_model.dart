import 'package:intl/intl.dart';

import 'pet_deletion_status_enum.dart';
import 'pet_ownership_model.dart';
import 'pet_species_enum.dart';

class PetModel {
  final String? id;
  final String name;
  final PetSpecies species;
  final String breed;
  final DateTime birthDate;
  final String? primaryOwnerId;
  final bool isActive;
  final PetDeletionStatus deletionStatus;
  final String? deletionRejectionReason;
  final List<PetOwnershipModel> ownerships;

  PetModel({
    this.id,
    required this.name,
    required this.species,
    required this.breed,
    required this.birthDate,
    this.primaryOwnerId,
    this.isActive = true,
    this.deletionStatus = PetDeletionStatus.none,
    this.deletionRejectionReason,
    this.ownerships = const [],
  });

  Map<String, dynamic> toJson() => {
        'name': name,
        'species': species.toBackendValue(),
        'breed': breed,
        'birthDate': DateFormat('yyyy-MM-dd').format(birthDate),
        'primaryOwnerId': primaryOwnerId,
      };

  factory PetModel.fromJson(Map<String, dynamic> json) {
    final ownershipsRaw = json['ownerships'] as List?;
    return PetModel(
      id: json['id'] as String?,
      name: json['name'] as String,
      species: PetSpecies.fromBackendValue(json['species'] as int),
      breed: json['breed'] as String,
      birthDate: DateTime.parse(json['birthDate'] as String),
      isActive: json['isActive'] as bool? ?? true,
      deletionStatus:
          PetDeletionStatus.fromBackendValue(json['deletionStatus'] as int?),
      deletionRejectionReason: json['deletionRejectionReason'] as String?,
      ownerships: ownershipsRaw == null
          ? const []
          : ownershipsRaw
              .map((e) => PetOwnershipModel.fromJson(e as Map<String, dynamic>))
              .toList(),
    );
  }
}
