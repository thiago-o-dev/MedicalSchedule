import 'package:flutter/material.dart';
import 'package:medical_schedule_app/medical_schedule_screen.dart';

class MedicalScheduleApp extends StatelessWidget {
  const MedicalScheduleApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: "Medical Schedule",
      home: MedicalScheduleScreen(),
      debugShowCheckedModeBanner: false,
    );
  }
}