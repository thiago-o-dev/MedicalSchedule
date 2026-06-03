import 'package:flutter/material.dart';

class VetAppointmentsScreen
    extends StatelessWidget {
  const VetAppointmentsScreen({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title:
            const Text('Vet Appointments'),
      ),
      body: ListView.builder(
        itemCount: 5,
        itemBuilder: (_, index) {
          return const Card(
            child: ListTile(
              title: Text('Dog'),
              subtitle: Text('Owner'),
            ),
          );
        },
      ),
    );
  }
}