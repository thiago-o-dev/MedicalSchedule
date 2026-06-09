import 'package:flutter/material.dart';

class OwnerBadge extends StatelessWidget {
  final String name;
  final String email;
  final bool isPrimary;
  final VoidCallback? onRemove;

  OwnerBadge({
    super.key,
    required this.name,
    required this.email,
    this.isPrimary = false,
    this.onRemove,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: BorderSide(color: Colors.grey.shade300),
      ),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: isPrimary ? Colors.blue.shade100 : Colors.grey.shade200,
          child: Icon(
            isPrimary ? Icons.star : Icons.person,
            color: isPrimary ? Colors.blue.shade700 : Colors.grey.shade700,
            size: 18,
          ),
        ),
        title: Text(name, style: TextStyle(fontWeight: FontWeight.w600)),
        subtitle: Text(email, style: TextStyle(fontSize: 12)),
        trailing: onRemove == null
            ? (isPrimary
                ? Chip(
                    label: Text('Primary', style: TextStyle(fontSize: 11)),
                    visualDensity: VisualDensity.compact,
                  )
                : null)
            : IconButton(
                icon: Icon(Icons.remove_circle_outline),
                color: Colors.red.shade700,
                tooltip: 'Remove owner',
                onPressed: onRemove,
              ),
      ),
    );
  }
}
