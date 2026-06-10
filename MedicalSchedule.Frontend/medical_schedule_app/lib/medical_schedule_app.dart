import 'package:flutter/material.dart';

import 'core/routes/app_router.dart';

class MedicalScheduleApp extends StatelessWidget {
  const MedicalScheduleApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      routerConfig: appRouter,
      debugShowCheckedModeBanner: false,
      scrollBehavior: ScrollBehavior(),
    );
  }
}
