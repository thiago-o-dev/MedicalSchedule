import 'package:flutter/material.dart';

class RelatedPetsScreen
    extends StatelessWidget {
  const RelatedPetsScreen({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar:
          AppBar(title: const Text('Pets')),
      body: ListView.builder(
        itemCount: 5,
        itemBuilder: (_, index) {
          return const Card(
            child: ListTile(
              title: Text('Cat'),
            ),
          );
        },
      ),
    );
  }
}