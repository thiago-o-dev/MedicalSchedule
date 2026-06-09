import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'medical_schedule_app.dart';

void main() {
  runApp(
    ProviderScope(
      child: MedicalScheduleApp(),
    ),
  );
}
