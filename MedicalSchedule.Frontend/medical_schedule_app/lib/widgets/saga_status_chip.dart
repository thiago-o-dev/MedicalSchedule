import 'package:flutter/material.dart';

import '../models/pet/pet_deletion_status_enum.dart';

class SagaStatusChip extends StatefulWidget {
  final PetDeletionStatus status;
  final String? rejectionReason;

  const SagaStatusChip({
    super.key,
    required this.status,
    this.rejectionReason,
  });

  @override
  State<SagaStatusChip> createState() => _SagaStatusChipState();
}

class _SagaStatusChipState extends State<SagaStatusChip>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;
  late final Animation<double> _pulse;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: Duration(milliseconds: 900),
    )..repeat(reverse: true);
    _pulse = Tween<double>(begin: 0.85, end: 1.0).animate(
      CurvedAnimation(parent: _controller, curve: Curves.easeInOut),
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.status == PetDeletionStatus.none) {
      return SizedBox.shrink();
    }

    final (label, color, icon) = switch (widget.status) {
      PetDeletionStatus.pendingDeletion => (
          'Awaiting review',
          Colors.orange.shade700,
          Icons.hourglass_top,
        ),
      PetDeletionStatus.deleted => (
          'Deleted',
          Colors.grey.shade700,
          Icons.delete_outline,
        ),
      PetDeletionStatus.none => throw StateError('unreachable'),
    };

    final shouldPulse = widget.status == PetDeletionStatus.pendingDeletion;

    final chip = AnimatedContainer(
      duration: Duration(milliseconds: 250),
      padding: EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        border: Border.all(color: color, width: 1),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: color),
          SizedBox(width: 6),
          Text(
            label,
            style: TextStyle(
              color: color,
              fontSize: 12,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );

    return shouldPulse ? ScaleTransition(scale: _pulse, child: chip) : chip;
  }
}
