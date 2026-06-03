import 'consultation_status_enum.dart';

class AppointmentModel {
  final String id;
  final String petId;
  final String vetId;
  final DateTime date;
  final ConsultationStatus status;

  AppointmentModel({
    required this.id,
    required this.petId,
    required this.vetId,
    required this.date,
    required this.status,
  });

  factory AppointmentModel.fromJson(Map<String, dynamic> json) {
    return AppointmentModel(
      id: json['id'],
      petId: json['petId'],
      vetId: json['vetId'],
      date: DateTime.parse(json['date']),
      status: ConsultationStatus.values[json['status']],
    );
  }
}