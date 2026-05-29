import 'package:flutter/material.dart';

class MedicalScheduleScreen extends StatelessWidget {
  const MedicalScheduleScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Center(child: Text("Medical Schedule")),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),

      body: Center(
        // aqui vai ter o app de vdd
      ),
    );
  }
}