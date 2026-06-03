import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'core/routes/app_router.dart';

// A gente usa o Go Router pra fazer nossa tela, ele é o ProviderScope,
// nele nos temos as telas definidas em core/config/routes.dart.
// Fiz assim pq desejei emular o funcionamento do Next.js nesse sistema.
void main() {
  runApp(
    const ProviderScope(
      child: MedicalScheduleApp(),
    ),
  );
}

class MedicalScheduleApp extends StatelessWidget {
  const MedicalScheduleApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      routerConfig: appRouter,
      debugShowCheckedModeBanner: false,
    );
  }
}