import 'package:flutter/material.dart';

import '../models/vet/vet_model.dart';

class VetCarouselCard extends StatelessWidget {
  final VetModel vet;
  final bool selected;

  const VetCarouselCard({
    super.key,
    required this.vet,
    this.selected = false,
  });

  @override
  Widget build(BuildContext context) {
    return AnimatedContainer(
      duration: Duration(milliseconds: 200),
      margin: EdgeInsets.symmetric(vertical: 4),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        color: selected ? Colors.blue.shade600 : Colors.white,
        border: Border.all(
          color: selected ? Colors.blue.shade700 : Colors.grey.shade300,
          width: selected ? 2 : 1,
        ),
        boxShadow: selected
            ? [
                BoxShadow(
                  color: Colors.blue.shade100,
                  blurRadius: 8,
                  offset: Offset(0, 4),
                ),
              ]
            : null,
      ),
      padding: EdgeInsets.all(14),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(
            Icons.medical_services,
            size: 28,
            color: selected ? Colors.white : Colors.blue.shade700,
          ),
          SizedBox(height: 8),
          Text(
            vet.name,
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.bold,
              color: selected ? Colors.white : Colors.black87,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
          SizedBox(height: 4),
          Text(
            vet.specialty,
            style: TextStyle(
              fontSize: 11,
              color: selected ? Colors.white70 : Colors.grey.shade700,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
          SizedBox(height: 6),
          Text(
            'CRM ${vet.crm}',
            style: TextStyle(
              fontSize: 10,
              color: selected ? Colors.white60 : Colors.grey.shade500,
            ),
          ),
        ],
      ),
    );
  }
}
