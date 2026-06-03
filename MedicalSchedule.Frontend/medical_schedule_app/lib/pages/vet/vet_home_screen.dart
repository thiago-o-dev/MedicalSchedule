import 'package:flutter/material.dart';

import 'related_pets_screen.dart';
import 'vet_appointments_screen.dart';

class VetHomeScreen
    extends StatefulWidget {
  const VetHomeScreen({super.key});

  @override
  State<VetHomeScreen> createState() =>
      _VetHomeScreenState();
}

class _VetHomeScreenState
    extends State<VetHomeScreen> {
  int index = 0;

  final pages = [
    const VetAppointmentsScreen(),
    const RelatedPetsScreen(),
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
            icon: Icon(Icons.list),
            label: 'Appointments',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.pets),
            label: 'Pets',
          ),
        ],
      ),
    );
  }
}