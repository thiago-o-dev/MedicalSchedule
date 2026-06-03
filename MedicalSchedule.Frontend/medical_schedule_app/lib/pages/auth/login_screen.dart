import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/widgets/custom_button.dart';
import '../../core/widgets/custom_input.dart';
import '../../repositories/auth_repository.dart';

class LoginScreen extends ConsumerStatefulWidget {
  const LoginScreen({super.key});

  @override
  ConsumerState<LoginScreen> createState() =>
      _LoginScreenState();
}

class _LoginScreenState
    extends ConsumerState<LoginScreen> {
  final emailController =
      TextEditingController();

  final passwordController =
      TextEditingController();

  bool loading = false;

  Future<void> login() async {
    setState(() {
      loading = true;
    });

    try {
      final repository = AuthRepository();

      await repository.login(
        email: emailController.text,
        password: passwordController.text,
      );

      if (mounted) {
        context.go('/owner');
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
      body: Padding(
        padding:
            const EdgeInsets.all(24),
        child: Center(
          child: Column(
            mainAxisAlignment:
                MainAxisAlignment.center,
            children: [
              const Text(
                'Medical Schedule',
                style: TextStyle(
                  fontSize: 32,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 24),

              CustomInput(
                label: 'Email',
                controller: emailController,
              ),

              CustomInput(
                label: 'Password',
                controller:
                    passwordController,
                obscureText: true,
              ),

              const SizedBox(height: 24),

              CustomButton(
                text:
                    loading
                        ? 'Loading...'
                        : 'Login',
                onPressed: login,
              ),

              TextButton(
                onPressed: () {
                  context.push('/signup');
                },
                child: const Text(
                  'Create account',
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}