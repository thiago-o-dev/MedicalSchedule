import 'package:flutter/material.dart';

class AppointmentsScreen
    extends StatelessWidget {
  const AppointmentsScreen({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          'Appointments',
        ),
      ),
      body: ListView.builder(
        itemCount: 10,
        itemBuilder: (_, index) {
          return const Card(
            child: ListTile(
              title: Text('Dog Appointment'),
              subtitle: Text(
                'Scheduled',
              ),
            ),
          );
        },
      ),
    );
  }
}