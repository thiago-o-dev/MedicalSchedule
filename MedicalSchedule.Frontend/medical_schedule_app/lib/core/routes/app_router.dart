import 'package:go_router/go_router.dart';

import '../../pages/auth/login_screen.dart';
import '../../pages/auth/signup_screen.dart';

final GoRouter appRouter = GoRouter(
  routes: [
    GoRoute(
      path: '/',
      builder: (_, _) => const LoginScreen(),
    ),
    GoRoute(
      path: '/signup',
      builder: (_, _) => const SignupScreen(),
    ),
  ],
);