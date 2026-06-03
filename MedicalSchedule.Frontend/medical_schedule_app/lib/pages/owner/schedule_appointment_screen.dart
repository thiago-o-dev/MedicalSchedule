import 'package:flutter/material.dart';

class ScheduleAppointmentScreen
    extends StatelessWidget {
  const ScheduleAppointmentScreen({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          'Schedule Appointment',
        ),
      ),
      body: const Center(
        child: Text(
          'Appointment Scheduler',
        ),
      ),
    );
  }
}