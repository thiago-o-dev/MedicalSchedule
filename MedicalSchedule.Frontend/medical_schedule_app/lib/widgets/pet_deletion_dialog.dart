import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/pet/pet_deletion_status_enum.dart';
import '../models/pet/pet_model.dart';
import '../state/pet_provider.dart';

enum _Phase { confirm, processing, rejected, error }

class PetDeletionDialog extends ConsumerStatefulWidget {
  final PetModel pet;

  const PetDeletionDialog({super.key, required this.pet});

  @override
  ConsumerState<PetDeletionDialog> createState() => _PetDeletionDialogState();
}

class _PetDeletionDialogState extends ConsumerState<PetDeletionDialog> {
  static const _pollInterval = Duration(milliseconds: 500);
  static const _maxAttempts = 20; // 10s total

  _Phase _phase = _Phase.confirm;
  String? _message;

  Future<void> _process() async {
    setState(() {
      _phase = _Phase.processing;
      _message = null;
    });

    try {
      await ref.read(petRepositoryProvider).requestPetDeletion(widget.pet.id!);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _phase = _Phase.error;
        _message = e.toString();
      });
      return;
    }

    for (var i = 0; i < _maxAttempts; i++) {
      await Future.delayed(_pollInterval);
      if (!mounted) return;

      ref.invalidate(petByIdProvider(widget.pet.id!));
      try {
        final updated =
            await ref.read(petByIdProvider(widget.pet.id!).future);

        if (updated.deletionStatus == PetDeletionStatus.deleted) {
          if (mounted) Navigator.of(context).pop(true);
          return;
        }
        if (updated.deletionStatus == PetDeletionStatus.none &&
            updated.deletionRejectionReason != null) {
          if (!mounted) return;
          setState(() {
            _phase = _Phase.rejected;
            _message = updated.deletionRejectionReason;
          });
          return;
        }
      } catch (_) {
        // Treat fetch failure as "deleted" — entity is gone.
        if (mounted) Navigator.of(context).pop(true);
        return;
      }
    }

    if (!mounted) return;
    setState(() {
      _phase = _Phase.error;
      _message = 'Timed out waiting for Scheduling review. '
          'The request is still in progress — try again in a moment.';
    });
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: _phase != _Phase.processing,
      child: AlertDialog(
        title: Text(_title()),
        content: _content(),
        actions: _actions(),
      ),
    );
  }

  String _title() => switch (_phase) {
        _Phase.confirm => 'Request deletion of "${widget.pet.name}"?',
        _Phase.processing => 'Checking schedule…',
        _Phase.rejected => 'Cannot delete pet',
        _Phase.error => 'Something went wrong',
      };

  Widget _content() => switch (_phase) {
        _Phase.confirm => const Text(
            'The Scheduling service will check for future appointments. '
            'If any exist, deletion will be rejected.',
          ),
        _Phase.processing => const Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              SizedBox(
                width: 20,
                height: 20,
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
              SizedBox(width: 12),
              Expanded(child: Text('Awaiting Scheduling review…')),
            ],
          ),
        _Phase.rejected => Text(
            _message ?? 'Deletion was rejected.',
            style: TextStyle(color: Colors.red.shade700),
          ),
        _Phase.error => Text(_message ?? 'An unexpected error occurred.'),
      };

  List<Widget> _actions() => switch (_phase) {
        _Phase.confirm => [
            TextButton(
              onPressed: () => Navigator.of(context).pop(false),
              child: const Text('Cancel'),
            ),
            FilledButton(
              style: FilledButton.styleFrom(
                backgroundColor: Colors.red.shade600,
              ),
              onPressed: _process,
              child: const Text('Request'),
            ),
          ],
        _Phase.processing => const <Widget>[],
        _Phase.rejected || _Phase.error => [
            FilledButton(
              onPressed: () => Navigator.of(context).pop(false),
              child: const Text('OK'),
            ),
          ],
      };
}

/// Shows the deletion dialog. Returns `true` if the pet was deleted.
Future<bool> showPetDeletionDialog(BuildContext context, PetModel pet) async {
  final result = await showDialog<bool>(
    context: context,
    barrierDismissible: false,
    builder: (_) => PetDeletionDialog(pet: pet),
  );
  return result ?? false;
}
