enum PetDeletionStatus {
  none,
  pendingDeletion,
  deleted;

  static PetDeletionStatus fromBackendValue(int? value) {
    switch (value) {
      case 1:
        return PetDeletionStatus.pendingDeletion;
      case 2:
        return PetDeletionStatus.deleted;
      case 0:
      case null:
      default:
        return PetDeletionStatus.none;
    }
  }

  String get displayName {
    switch (this) {
      case PetDeletionStatus.none:
        return 'Active';
      case PetDeletionStatus.pendingDeletion:
        return 'Awaiting review';
      case PetDeletionStatus.deleted:
        return 'Deleted';
    }
  }
}
