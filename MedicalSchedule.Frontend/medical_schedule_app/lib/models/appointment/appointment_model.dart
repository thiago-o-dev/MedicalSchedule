import 'consultation_status_enum.dart';

class AppointmentModel {
  final String id;
  final String petId;
  final String vetId;
  final String ownerId;
  final DateTime scheduledAt;
  final ConsultationStatus status;
  final String? notes;

  AppointmentModel({
    required this.id,
    required this.petId,
    required this.vetId,
    required this.ownerId,
    required this.scheduledAt,
    required this.status,
    this.notes,
  });

  factory AppointmentModel.fromJson(Map<String, dynamic> json) {
    return AppointmentModel(
      id: json['id'] as String,
      petId: json['petId'] as String,
      vetId: json['vetId'] as String,
      ownerId: json['ownerId'] as String,
      scheduledAt: DateTime.parse(json['scheduledAt'] as String),
      status: ConsultationStatus.fromBackendValue(json['status'] as int),
      notes: json['notes'] as String?,
    );
  }
}
