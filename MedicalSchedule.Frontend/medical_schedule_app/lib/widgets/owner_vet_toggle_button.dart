import 'package:flutter/material.dart';

class OwnerVetToggleButton extends StatelessWidget {
  final bool isOwner;
  final ValueChanged<int> onPressed;

  const OwnerVetToggleButton({
    super.key,
    required this.isOwner,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return ToggleButtons(
      borderRadius: BorderRadius.circular(10),
      isSelected: [isOwner, !isOwner],
      onPressed: onPressed,
      children: const [
        Padding(
          padding: EdgeInsets.symmetric(horizontal: 20),
          child: Text('Owner'),
        ),
        Padding(
          padding: EdgeInsets.symmetric(horizontal: 20),
          child: Text('Vet'),
        ),
      ],
    );
  }
}