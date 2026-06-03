import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:medical_schedule_app/repositories/auth_repository.dart';

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

  // do owner
  final nameController = TextEditingController();
  final documentController = TextEditingController();
  final phoneController = TextEditingController();
  // pro keycloak
  final emailController = TextEditingController();
  final passwordController = TextEditingController();

  bool loading = false;

  Future<void> register() async {
    setState(() {
      loading = true;
    });

    try {
      final repository = AuthRepository();

      await repository.register(
        name: nameController.text,
        document: documentController.text,
        phone: phoneController.text,
        email: emailController.text,
        password: passwordController.text,
      );

      if (mounted) {
        if (isOwner){
          context.go('/owner');
        }else{
          context.go('/vet');
        }
        
      }
    } catch (e) {
      ScaffoldMessenger.of(context)
          .showSnackBar(
        SnackBar(
          content: Text(e.toString()),
        ),
      );
    }

    setState(() {
      loading = false;
    });
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Sign Up')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
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
                  padding: EdgeInsets.all(12),
                  child: Text('Owner'),
                ),
                Padding(
                  padding: EdgeInsets.all(12),
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
              label: isOwner ? 'CPF' : 'CRMV',
              controller: documentController,
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
              text: loading ? 'Loading...' : 'Register',
              onPressed: register,
            ),
            
            TextButton(
                onPressed: () {
                  context.pop();
                },
                child: const Text(
                  'Return',
                ),
              ),
          ],
        ),
      ),
    );
  }
}