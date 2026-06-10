import 'package:flutter/material.dart';

import '../models/appointment/appointment_model.dart';
import '../models/appointment/consultation_status_enum.dart';

class AppointmentCard extends StatelessWidget {
  final AppointmentModel appointment;
  final VoidCallback? onTap;
  final VoidCallback? onCancel;
  final VoidCallback? onReschedule;

  const AppointmentCard({
    super.key,
    required this.appointment,
    this.onTap,
    this.onCancel,
    this.onReschedule,
  });

  String _format(DateTime dt) =>
      '${dt.day.toString().padLeft(2, '0')}/${dt.month.toString().padLeft(2, '0')}/${dt.year} '
      '${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';

  @override
  Widget build(BuildContext context) {
    final isScheduled = appointment.status == ConsultationStatus.scheduled;
    final statusColor = switch (appointment.status) {
      ConsultationStatus.scheduled => Colors.blue.shade700,
      ConsultationStatus.completed => Colors.green.shade700,
      ConsultationStatus.cancelled => Colors.grey.shade600,
    };

    return Card(
      margin: EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          child: Row(
            children: [
              Container(
                padding: EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha: 0.1),
                  shape: BoxShape.circle,
                ),
                child: Icon(Icons.event, color: statusColor),
              ),
              SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      _format(appointment.scheduledAt),
                      style: TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    SizedBox(height: 2),
                    Text(
                      appointment.status.displayName,
                      style: TextStyle(color: statusColor, fontSize: 12),
                    ),
                    if (appointment.notes != null &&
                        appointment.notes!.isNotEmpty) ...[
                      SizedBox(height: 4),
                      Text(
                        appointment.notes!,
                        style: TextStyle(
                          fontStyle: FontStyle.italic,
                          fontSize: 12,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                  ],
                ),
              ),
              if (isScheduled && (onReschedule != null || onCancel != null))
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    if (onReschedule != null)
                      IconButton(
                        icon: Icon(Icons.edit_calendar),
                        tooltip: 'Reschedule',
                        onPressed: onReschedule,
                      ),
                    if (onCancel != null)
                      IconButton(
                        icon: Icon(
                          Icons.cancel_outlined,
                          color: Colors.red.shade700,
                        ),
                        tooltip: 'Cancel',
                        onPressed: onCancel,
                      ),
                  ],
                ),
            ],
          ),
        ),
      ),
    );
  }
}
