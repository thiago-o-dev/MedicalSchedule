import 'package:flutter/material.dart';

class LoadingWidget extends StatelessWidget {
  LoadingWidget({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: CircularProgressIndicator(), // é o q fica girando
    );
  }
}