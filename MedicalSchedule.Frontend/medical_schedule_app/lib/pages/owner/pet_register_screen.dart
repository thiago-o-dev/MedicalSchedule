import 'package:flutter/material.dart';

class PetRegisterScreen
    extends StatelessWidget {
  const PetRegisterScreen({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    final pets = [
      'Dog',
      'Cat',
      'Axolotl',
      'Dinosaur',
    ];

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Pets'),
      ),
      body: Column(
        children: [
          SizedBox(
            height: 220,
            child: CarouselView(
              itemExtent: 250,
              children:
                  pets.map((pet) {
                    return Card(
                      child: Center(
                        child: Text(
                          pet,
                          style:
                              const TextStyle(
                                fontSize: 32,
                              ),
                        ),
                      ),
                    );
                  }).toList(),
            ),
          ),
        ],
      ),
    );
  }
}