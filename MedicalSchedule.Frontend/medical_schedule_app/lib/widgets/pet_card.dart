import 'package:flutter/material.dart';

import '../models/pet/pet_model.dart';
import 'saga_status_chip.dart';

class PetCard extends StatelessWidget {
  final PetModel pet;
  final VoidCallback? onTap;
  final VoidCallback? onDelete;
  final bool showActions;

  const PetCard({
    super.key,
    required this.pet,
    this.onTap,
    this.onDelete,
    this.showActions = true,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: EdgeInsets.all(12),
          child: Row(
            children: [
              Hero(
                tag: 'pet-${pet.id ?? pet.name}',
                child: CircleAvatar(
                  backgroundColor: Colors.blue.shade50,
                  radius: 24,
                  child: Icon(Icons.pets, color: Colors.blue),
                ),
              ),
              SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      pet.name,
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    SizedBox(height: 2),
                    Text(
                      '${pet.species.displayName} • ${pet.breed}',
                      style: TextStyle(color: Colors.grey.shade600, fontSize: 13),
                    ),
                    SizedBox(height: 6),
                    SagaStatusChip(
                      status: pet.deletionStatus,
                      rejectionReason: pet.deletionRejectionReason,
                    ),
                  ],
                ),
              ),
              if (showActions && onDelete != null && pet.isActive)
                IconButton(
                  icon: Icon(Icons.delete_outline),
                  color: Colors.red.shade700,
                  tooltip: 'Request deletion',
                  onPressed: onDelete,
                ),
            ],
          ),
        ),
      ),
    );
  }
}
