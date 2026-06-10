import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../core/config/routes.dart';
import '../core/storage/token_storage.dart';

class AppDrawer extends StatelessWidget {
  final String role;
  final String? userName;
  final String? userEmail;
  final String? userId;

  const AppDrawer({
    super.key,
    required this.role,
    this.userName,
    this.userEmail,
    this.userId,
  });

  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: SafeArea(
        child: Column(
          children: [
            UserAccountsDrawerHeader(
              decoration: BoxDecoration(color: Colors.blue.shade600),
              accountName: Text(userName ?? 'Welcome'),
              accountEmail: Text(userEmail ?? role.toUpperCase()),
              currentAccountPicture: CircleAvatar(
                backgroundColor: Colors.white,
                child: Icon(
                  role == 'owner' ? Icons.person : Icons.medical_services,
                  color: Colors.blue.shade700,
                  size: 32,
                ),
              ),
            ),
            ListTile(
              leading: Icon(Icons.account_circle),
              title: Text('Profile'),
              onTap: userId == null
                  ? null
                  : () {
                      Navigator.pop(context);
                      context.push('${Routes.profile}/$role/$userId');
                    },
            ),
            Divider(),
            Spacer(),
            ListTile(
              leading: Icon(Icons.logout, color: Colors.red),
              title: Text(
                'Logout',
                style: TextStyle(color: Colors.red),
              ),
              onTap: () async {
                await TokenStorage.clear();
                if (context.mounted) context.go(Routes.login);
              },
            ),
            SizedBox(height: 8),
          ],
        ),
      ),
    );
  }
}
