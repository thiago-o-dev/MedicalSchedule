import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../core/storage/token_storage.dart';
import '../../state/appointment_provider.dart';
import '../../state/vet_provider.dart';

class VetAppointmentsScreen extends ConsumerWidget {
  const VetAppointmentsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final vetId = ref.watch(currentVetProvider).valueOrNull?.id;
    final appointmentsAsync = ref.watch(appointmentsProvider(vetId));

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Appointments'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Logout',
            onPressed: () async {
              await TokenStorage.clear();
              if (context.mounted) context.go(Routes.login);
            },
          ),
        ],
      ),
      body: appointmentsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text('Error: $e')),
        data: (appointments) {
          if (appointments.isEmpty) {
            return const Center(child: Text('No appointments.'));
          }
          return ListView.builder(
            itemCount: appointments.length,
            itemBuilder: (_, i) {
              final a = appointments[i];
              final dt = a.scheduledAt;
              final dateStr =
                  '${dt.day.toString().padLeft(2, '0')}/${dt.month.toString().padLeft(2, '0')}/${dt.year} '
                  '${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';
              return Card(
                margin: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 6,
                ),
                child: ListTile(
                  leading: const Icon(Icons.calendar_month),
                  title: Text(dateStr),
                  subtitle: Text(a.status.displayName),
                  trailing: a.notes != null && a.notes!.isNotEmpty
                      ? const Icon(Icons.notes)
                      : null,
                ),
              );
            },
          );
        },
      ),
    );
  }
}
