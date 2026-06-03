import 'package:flutter/material.dart';

import '../../core/widgets/custom_button.dart';
import '../../core/widgets/custom_input.dart';

class SignupScreen extends StatefulWidget {
  const SignupScreen({super.key});

  @override
  State<SignupScreen> createState() =>
      _SignupScreenState();
}

class _SignupScreenState
    extends State<SignupScreen> {
  bool isOwner = true;

  final nameController =
      TextEditingController();

  final documentController =
      TextEditingController();

  final phoneController =
      TextEditingController();

  final emailController =
      TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar:
          AppBar(title: const Text('Sign Up')),
      body: Padding(
        padding:
            const EdgeInsets.all(24),
        child: Column(
          children: [
            ToggleButtons(
              isSelected: [
                isOwner,
                !isOwner,
              ],
              onPressed: (index) {
                setState(() {
                  isOwner = index == 0;
                });
              },
              children: const [
                Padding(
                  padding:
                      EdgeInsets.all(12),
                  child: Text('Owner'),
                ),
                Padding(
                  padding:
                      EdgeInsets.all(12),
                  child: Text('Vet'),
                ),
              ],
            ),

            const SizedBox(height: 24),

            CustomInput(
              label: 'Name',
              controller: nameController,
            ),

            CustomInput(
              label:
                  isOwner ? 'CPF' : 'CRMV',
              controller:
                  documentController,
            ),

            CustomInput(
              label: 'Phone',
              controller: phoneController,
            ),

            CustomInput(
              label: 'Email',
              controller: emailController,
            ),

            const SizedBox(height: 24),

            CustomButton(
              text: 'Register',
              onPressed: () {},
            ),
          ],
        ),
      ),
    );
  }
}