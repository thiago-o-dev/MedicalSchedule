enum ConsultationStatus {
  scheduled,
  completed,
  cancelled;

  static ConsultationStatus fromBackendValue(int value) {
    switch (value) {
      case 1:
        return ConsultationStatus.scheduled;
      case 2:
        return ConsultationStatus.completed;
      case 3:
        return ConsultationStatus.cancelled;
      default:
        return ConsultationStatus.scheduled;
    }
  }

  String get displayName {
    switch (this) {
      case ConsultationStatus.scheduled:
        return 'Scheduled';
      case ConsultationStatus.completed:
        return 'Completed';
      case ConsultationStatus.cancelled:
        return 'Cancelled';
    }
  }
}
