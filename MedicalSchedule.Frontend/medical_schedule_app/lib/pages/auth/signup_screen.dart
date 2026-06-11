import 'dart:math';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:medical_schedule_app/widgets/owner_vet_toggle_button.dart';
import 'package:pattern_box/pattern_box.dart';

import '../../core/widgets/custom_button.dart';
import '../../core/widgets/custom_input.dart';
import '../../repositories/auth_repository.dart';
import '../../widgets/api_error_snackbar.dart';

class SignupScreen extends StatefulWidget {
  const SignupScreen({super.key});

  @override
  State<SignupScreen> createState() => _SignupScreenState();
}

class _SignupScreenState extends State<SignupScreen> {
  final _formKey = GlobalKey<FormState>();
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
    if (!_formKey.currentState!.validate()) return;

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

  final List<int> seeds = [0,8, 22, 25];
  final int selectedSeed = Random().nextInt(4);
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Sign Up')),
      body: CustomPaint(
        painter: KaleidoscopePainter(color: const Color.fromARGB(255, 197, 217, 255), seed: seeds[selectedSeed]),
        child: Center(
          child: SingleChildScrollView(
            padding: EdgeInsets.all(24),
            child: ConstrainedBox(
              constraints: BoxConstraints(
                maxWidth: 400.0
              ),
              child: Form(
                key: _formKey,
                child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                        Text('Medical ', style: TextStyle(fontSize: 32)),
                        Text(
                          'Schedule',
                          style: TextStyle(
                            fontSize: 32,
                            fontWeight: FontWeight.bold,
                            color: Color.fromARGB(255, 56, 0, 109),
                          ),
                        ),
                      ],
                    ),

                    SizedBox(height: 24),

                    OwnerVetToggleButton(
                      isOwner: _isOwner,
                      onPressed: (i) => setState(() => _isOwner = i == 0),
                    ),

                    SizedBox(height: 24),

                    CustomInput(
                      label: 'Name',
                      controller: _nameCtrl,
                      textCapitalization: TextCapitalization.words,
                      validator: (v) =>
                          v == null || v.isEmpty ? 'Informe o nome' : null,
                    ),
                    CustomInput(
                      label: _isOwner ? 'CPF' : 'CRM',
                      controller: _documentCtrl,
                      keyboardType: TextInputType.number,
                      validator: (v) => v == null || v.isEmpty
                          ? 'Informe o ${_isOwner ? 'CPF' : 'CRM'}'
                          : null,
                    ),
                    CustomInput(
                      label: 'Phone',
                      controller: _phoneCtrl,
                      keyboardType: TextInputType.phone,
                      validator: (v) =>
                          v == null || v.isEmpty ? 'Informe o telefone' : null,
                    ),

                    if (!_isOwner)
                      CustomInput(
                        label: 'Specialty',
                        controller: _specialtyCtrl,
                        validator: (v) => v == null || v.isEmpty
                            ? 'Informe a especialidade'
                            : null,
                      ),

                    CustomInput(
                      label: 'Email',
                      controller: _emailCtrl,
                      keyboardType: TextInputType.emailAddress,
                      validator: (v) =>
                          v == null || v.isEmpty ? 'Informe o email' : null,
                    ),
                    CustomInput(
                      label: 'Password',
                      controller: _passwordCtrl,
                      obscureText: true,
                      validator: (v) =>
                          v == null || v.isEmpty ? 'Informe a senha' : null,
                    ),

                    SizedBox(height: 24),

                    CustomButton(
                      text: _loading ? 'Loading...' : 'Register',
                      onPressed: _loading ? () {} : _register,
                    ),

                    SizedBox(height: 20),
                    TextButton(
                      onPressed: () => context.pop(),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text('Return'),
                          Icon(Icons.arrow_back, color: Theme.of(context).primaryColor),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
