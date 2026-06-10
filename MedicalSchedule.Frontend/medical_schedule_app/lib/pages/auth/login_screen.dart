import 'dart:math';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:medical_schedule_app/widgets/owner_vet_toggle_button.dart';
import 'package:pattern_box/pattern_box.dart';

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
  final List<int> seeds = [0,8, 22, 25];
  final int selectedSeed = Random().nextInt(3);
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: CustomPaint(
        painter: KaleidoscopePainter(color: const Color.fromARGB(255, 176, 162, 255), seed: seeds[selectedSeed]),
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      'Medical ',
                      style: TextStyle(fontSize: 32),
                    ),
                    Text(
                      'Schedule',
                      style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold, color: Color.fromARGB(255, 56, 0, 109)),
                    ),
                  ],
                ),
                
                SizedBox(height: 24),
        
                OwnerVetToggleButton(
                  isOwner: _isOwner, 
                  onPressed: (i) => setState(() => _isOwner = i == 0)
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
                SizedBox(height: 20,),
                TextButton(
                  onPressed: () => context.push('/signup'),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text('Register'),
                      Icon(Icons.person, color: Theme.of(context).primaryColor,)
                    ]
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
