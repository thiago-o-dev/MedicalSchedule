import 'package:flutter/material.dart';

import 'appointments_screen.dart';
import 'pet_register_screen.dart';
import 'schedule_appointment_screen.dart';

class OwnerHomeScreen extends StatefulWidget {
  const OwnerHomeScreen({super.key});

  @override
  State<OwnerHomeScreen> createState() =>
      _OwnerHomeScreenState();
}

class _OwnerHomeScreenState
    extends State<OwnerHomeScreen> {
  int index = 0;

  final pages = [
    const PetRegisterScreen(),
    const ScheduleAppointmentScreen(),
    const AppointmentsScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: pages[index],

      bottomNavigationBar:
          BottomNavigationBar(
        currentIndex: index,
        onTap: (value) {
          setState(() {
            index = value;
          });
        },
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.pets),
            label: 'Pets',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.calendar_month),
            label: 'Schedule',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.list),
            label: 'Appointments',
          ),
        ],
      ),
    );
  }
}