import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../state/vet_provider.dart';
import '../../widgets/app_drawer.dart';
import 'related_pets_screen.dart';
import 'vet_appointments_screen.dart';

class VetHomeScreen extends ConsumerStatefulWidget {
  VetHomeScreen({super.key});

  @override
  ConsumerState<VetHomeScreen> createState() => _VetHomeScreenState();
}

class _VetHomeScreenState extends ConsumerState<VetHomeScreen> {
  int index = 0;

  final _titles = ['My Appointments', 'Pets'];
  final _pages = [VetAppointmentsScreen(), RelatedPetsScreen()];

  @override
  Widget build(BuildContext context) {
    final vetAsync = ref.watch(currentVetProvider);
    final vet = vetAsync.valueOrNull;

    return Scaffold(
      appBar: AppBar(title: Text(_titles[index])),
      drawer: AppDrawer(
        role: 'vet',
        userName: vet?.name,
        userEmail: vet?.email,
        userId: vet?.id,
      ),
      body: _pages[index],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: index,
        onTap: (value) => setState(() => index = value),
        items: [
          BottomNavigationBarItem(
            icon: Icon(Icons.list),
            label: 'Appointments',
          ),
          BottomNavigationBarItem(icon: Icon(Icons.pets), label: 'Pets'),
        ],
      ),
    );
  }
}
