import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/widgets/custom_button.dart';
import '../../core/widgets/custom_input.dart';
import '../../repositories/auth_repository.dart';

class LoginScreen extends ConsumerStatefulWidget {
  LoginScreen({super.key});

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _emailCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();
  bool _isOwner = true;
  bool _loading = false;

  @override
  void dispose() {
    _emailCtrl.dispose();
    _passwordCtrl.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    if (_emailCtrl.text.isEmpty || _passwordCtrl.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Fill in email and password.')),
      );
      return;
    }

    setState(() => _loading = true);

    try {
      await AuthRepository().login(
        email: _emailCtrl.text,
        password: _passwordCtrl.text,
      );

      if (mounted) context.go(_isOwner ? '/owner' : '/vet');
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(e.toString())),
        );
      }
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Padding(
        padding: EdgeInsets.all(24),
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                'Medical Schedule',
                style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold),
              ),
              SizedBox(height: 24),

              ToggleButtons(
                isSelected: [_isOwner, !_isOwner],
                onPressed: (i) => setState(() => _isOwner = i == 0),
                children: [
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 20),
                    child: Text('Owner'),
                  ),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 20),
                    child: Text('Vet'),
                  ),
                ],
              ),

              SizedBox(height: 24),

              CustomInput(label: 'Email', controller: _emailCtrl),
              CustomInput(
                label: 'Password',
                controller: _passwordCtrl,
                obscureText: true,
              ),

              SizedBox(height: 24),

              CustomButton(
                text: _loading ? 'Loading...' : 'Login',
                onPressed: _loading ? () {} : _login,
              ),

              TextButton(
                onPressed: () => context.push('/signup'),
                child: Text('Create account'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
