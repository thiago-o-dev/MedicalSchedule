import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/routes.dart';
import '../../core/storage/token_storage.dart';
import '../../state/owner_provider.dart';
import '../../state/vet_provider.dart';
import '../../widgets/api_error_snackbar.dart';
import '../../widgets/confirm_dialog.dart';

class ProfileScreen extends ConsumerWidget {
  final String role;
  final String userId;

  ProfileScreen({
    super.key,
    required this.role,
    required this.userId,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Scaffold(
      appBar: AppBar(title: Text('Profile')),
      body: role == 'vet'
          ? _VetProfile(userId: userId)
          : _OwnerProfile(userId: userId),
    );
  }
}

class _OwnerProfile extends ConsumerWidget {
  final String userId;
  _OwnerProfile({required this.userId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final ownerAsync = ref.watch(ownerByIdProvider(userId));
    return ownerAsync.when(
      loading: () => Center(child: CircularProgressIndicator()),
      error: (e, _) => Center(child: Text(e.toString())),
      data: (owner) => ListView(
        padding: EdgeInsets.all(16),
        children: [
          _Avatar(icon: Icons.person, color: Colors.blue),
          SizedBox(height: 16),
          _row(Icons.badge, 'Name', owner.name),
          _row(Icons.credit_card, 'CPF', owner.cpf),
          _row(Icons.email, 'Email', owner.email),
          _row(Icons.phone, 'Phone', owner.phone),
          _row(
            owner.isActive ? Icons.check_circle : Icons.cancel,
            'Status',
            owner.isActive ? 'Active' : 'Inactive',
          ),
        ],
      ),
    );
  }
}

class _VetProfile extends ConsumerWidget {
  final String userId;
  _VetProfile({required this.userId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final vetAsync = ref.watch(vetByIdProvider(userId));
    return vetAsync.when(
      loading: () => Center(child: CircularProgressIndicator()),
      error: (e, _) => Center(child: Text(e.toString())),
      data: (vet) => ListView(
        padding: EdgeInsets.all(16),
        children: [
          _Avatar(icon: Icons.medical_services, color: Colors.teal),
          SizedBox(height: 16),
          _row(Icons.badge, 'Name', vet.name),
          _row(Icons.medical_information, 'CRM', vet.crm),
          _row(Icons.local_hospital, 'Specialty', vet.specialty),
          _row(Icons.email, 'Email', vet.email),
          _row(
            vet.isActive ? Icons.check_circle : Icons.cancel,
            'Status',
            vet.isActive ? 'Active' : 'Inactive',
          ),
          SizedBox(height: 24),
          if (vet.isActive)
            ElevatedButton.icon(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.red.shade600,
                foregroundColor: Colors.white,
              ),
              icon: Icon(Icons.power_settings_new),
              label: Text('Deactivate account'),
              onPressed: () async {
                final ok = await showConfirmDialog(
                  context,
                  title: 'Deactivate account?',
                  message:
                      'You will no longer receive new appointments. '
                      'Existing appointments stay scheduled.',
                  destructive: true,
                  confirmLabel: 'Deactivate',
                );
                if (!ok || !context.mounted) return;
                try {
                  await ref.read(vetRepositoryProvider).deactivateVet(userId);
                  await TokenStorage.clear();
                  if (context.mounted) context.go(Routes.login);
                } catch (e) {
                  if (context.mounted) showApiErrorSnackBar(context, e);
                }
              },
            ),
        ],
      ),
    );
  }
}

class _Avatar extends StatelessWidget {
  final IconData icon;
  final Color color;
  _Avatar({required this.icon, required this.color});

  @override
  Widget build(BuildContext context) => Center(
        child: CircleAvatar(
          radius: 48,
          backgroundColor: color.withValues(alpha: 0.15),
          child: Icon(icon, size: 48, color: color),
        ),
      );
}

Widget _row(IconData icon, String label, String value) => Card(
      elevation: 0,
      child: ListTile(
        leading: Icon(icon, color: Colors.blue.shade700),
        title: Text(label, style: TextStyle(fontSize: 12)),
        subtitle: Text(
          value,
          style: TextStyle(fontSize: 16, color: Colors.black87),
        ),
      ),
    );
