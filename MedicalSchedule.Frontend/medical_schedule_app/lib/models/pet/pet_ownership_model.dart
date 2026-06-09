class PetOwnershipModel {
  final String ownerId;
  final bool isPrimaryOwner;

  PetOwnershipModel({
    required this.ownerId,
    required this.isPrimaryOwner,
  });

  factory PetOwnershipModel.fromJson(Map<String, dynamic> json) {
    return PetOwnershipModel(
      ownerId: json['ownerId'] as String,
      isPrimaryOwner: json['isPrimaryOwner'] as bool? ?? false,
    );
  }
}
