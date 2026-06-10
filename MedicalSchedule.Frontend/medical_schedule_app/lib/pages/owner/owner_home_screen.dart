import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../state/owner_provider.dart';
import '../../widgets/app_drawer.dart';
import 'appointments_screen.dart';
import 'pet_register_screen.dart';
import 'schedule_appointment_screen.dart';

class OwnerHomeScreen extends ConsumerStatefulWidget {
  const OwnerHomeScreen({super.key});

  @override
  ConsumerState<OwnerHomeScreen> createState() => _OwnerHomeScreenState();
}

class _OwnerHomeScreenState extends ConsumerState<OwnerHomeScreen> {
  int index = 0;

  final _titles = ['My Pets', 'Schedule', 'Appointments'];
  final _pages = [
    PetRegisterScreen(),
    ScheduleAppointmentScreen(),
    AppointmentsScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    final ownerAsync = ref.watch(currentOwnerProvider);
    final owner = ownerAsync.valueOrNull;

    return Scaffold(
      appBar: AppBar(title: Text(_titles[index])),
      drawer: AppDrawer(
        role: 'owner',
        userName: owner?.name,
        userEmail: owner?.email,
        userId: owner?.id,
      ),
      body: _pages[index],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: index,
        onTap: (value) => setState(() => index = value),
        items: [
          BottomNavigationBarItem(icon: Icon(Icons.pets), label: 'Pets'),
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
