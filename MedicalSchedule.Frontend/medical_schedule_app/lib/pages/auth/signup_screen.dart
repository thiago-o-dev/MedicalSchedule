import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/widgets/custom_button.dart';
import '../../core/widgets/custom_input.dart';
import '../../repositories/auth_repository.dart';
import '../../widgets/api_error_snackbar.dart';

class SignupScreen extends StatefulWidget {
  SignupScreen({super.key});

  @override
  State<SignupScreen> createState() => _SignupScreenState();
}

class _SignupScreenState extends State<SignupScreen> {
  bool _isOwner = true;
  bool _loading = false;

  final _nameCtrl = TextEditingController();
  final _documentCtrl = TextEditingController();
  final _phoneCtrl = TextEditingController();
  final _specialtyCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();

  @override
  void dispose() {
    _nameCtrl.dispose();
    _documentCtrl.dispose();
    _phoneCtrl.dispose();
    _specialtyCtrl.dispose();
    _emailCtrl.dispose();
    _passwordCtrl.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    if (_nameCtrl.text.isEmpty ||
        _documentCtrl.text.isEmpty ||
        _phoneCtrl.text.isEmpty ||
        _emailCtrl.text.isEmpty ||
        _passwordCtrl.text.isEmpty ||
        (!_isOwner && _specialtyCtrl.text.isEmpty)) {
      showApiErrorSnackBar(context, 'Fill in all required fields.');
      return;
    }

    setState(() => _loading = true);

    try {
      await AuthRepository().register(
        name: _nameCtrl.text,
        document: _documentCtrl.text,
        phone: _phoneCtrl.text,
        email: _emailCtrl.text,
        password: _passwordCtrl.text,
        isOwner: _isOwner,
        specialty: _isOwner ? null : _specialtyCtrl.text,
      );

      if (mounted) context.go(_isOwner ? '/owner' : '/vet');
    } catch (e) {
      if (mounted) showApiErrorSnackBar(context, e);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Registrar')),
      body: Center(
        child: SingleChildScrollView(
          padding: EdgeInsets.all(24),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
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
        
              CustomInput(label: 'Name', controller: _nameCtrl),
              CustomInput(
                label: _isOwner ? 'CPF' : 'CRM',
                controller: _documentCtrl,
              ),
              CustomInput(label: 'Phone', controller: _phoneCtrl),
        
              if (!_isOwner)
                CustomInput(label: 'Specialty', controller: _specialtyCtrl),
        
              CustomInput(label: 'Email', controller: _emailCtrl),
              CustomInput(
                label: 'Password',
                controller: _passwordCtrl,
                obscureText: true,
              ),
        
              SizedBox(height: 24),
        
              CustomButton(
                text: _loading ? 'Loading...' : 'Register',
                onPressed: _loading ? () {} : _register,
              ),
        
              TextButton(
                onPressed: () => context.pop(),
                child: Text('Return'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
