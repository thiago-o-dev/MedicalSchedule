import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../models/appointment/consultation_status_enum.dart';
import '../../state/appointment_provider.dart';
import '../../state/pet_provider.dart';
import '../../state/vet_provider.dart';

class AppointmentDetailScreen extends ConsumerWidget {
  final String appointmentId;

  AppointmentDetailScreen({super.key, required this.appointmentId});

  String _format(DateTime dt) =>
      '${dt.day.toString().padLeft(2, '0')}/${dt.month.toString().padLeft(2, '0')}/${dt.year} '
      '${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final appointmentAsync = ref.watch(appointmentByIdProvider(appointmentId));

    return Scaffold(
      appBar: AppBar(title: Text('Appointment Details')),
      body: appointmentAsync.when(
        loading: () => Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text(e.toString())),
        data: (appointment) {
          final petAsync = ref.watch(petByIdProvider(appointment.petId));
          final vetAsync = ref.watch(vetByIdProvider(appointment.vetId));

          final statusColor = switch (appointment.status) {
            ConsultationStatus.scheduled => Colors.blue.shade700,
            ConsultationStatus.completed => Colors.green.shade700,
            ConsultationStatus.cancelled => Colors.grey.shade600,
          };

          return ListView(
            padding: EdgeInsets.all(16),
            children: [
              Center(
                child: Container(
                  padding: EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    color: statusColor.withValues(alpha: 0.1),
                    shape: BoxShape.circle,
                  ),
                  child: Icon(Icons.event, size: 48, color: statusColor),
                ),
              ),
              SizedBox(height: 16),
              Center(
                child: Text(
                  _format(appointment.scheduledAt),
                  style: TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              SizedBox(height: 4),
              Center(
                child: Chip(
                  label: Text(appointment.status.displayName),
                  backgroundColor: statusColor.withValues(alpha: 0.15),
                  labelStyle: TextStyle(color: statusColor),
                ),
              ),
              SizedBox(height: 24),
              Card(
                child: Padding(
                  padding: EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      _row(
                        Icons.pets,
                        'Pet',
                        petAsync.when(
                          loading: () => '...',
                          error: (e, _) => 'Error',
                          data: (p) => '${p.name} (${p.species.displayName})',
                        ),
                      ),
                      Divider(),
                      _row(
                        Icons.medical_services,
                        'Veterinarian',
                        vetAsync.when(
                          loading: () => '...',
                          error: (e, _) => 'Error',
                          data: (v) => '${v.name} • ${v.specialty}',
                        ),
                      ),
                      if (appointment.notes != null &&
                          appointment.notes!.isNotEmpty) ...[
                        Divider(),
                        _row(Icons.notes, 'Notes', appointment.notes!),
                      ],
                    ],
                  ),
                ),
              ),
            ],
          );
        },
      ),
    );
  }

  Widget _row(IconData icon, String label, String value) => Padding(
        padding: EdgeInsets.symmetric(vertical: 8),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, size: 18, color: Colors.blue.shade700),
            SizedBox(width: 8),
            SizedBox(
              width: 110,
              child: Text(
                label,
                style: TextStyle(fontWeight: FontWeight.w500),
              ),
            ),
            Expanded(
              child: Text(value, style: TextStyle(color: Colors.grey.shade800)),
            ),
          ],
        ),
      );
}
