import 'package:go_router/go_router.dart';

import '../config/routes.dart';

import '../../pages/auth/login_screen.dart';
import '../../pages/auth/signup_screen.dart';

final GoRouter appRouter = GoRouter(
  routes: [
    GoRoute(
      path: Routes.login,
      builder: (_, _) => const LoginScreen(),
    ),
    GoRoute(
      path: Routes.signup,
      builder: (_, _) => const SignupScreen(),
    ),
  ],
);