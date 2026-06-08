import 'package:go_router/go_router.dart';

import '../config/routes.dart';
import '../../pages/auth/login_screen.dart';
import '../../pages/auth/signup_screen.dart';
import '../../pages/owner/owner_home_screen.dart';
import '../../pages/vet/vet_home_screen.dart';

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
    GoRoute(
      path: Routes.ownerHome,
      builder: (_, _) => const OwnerHomeScreen(),
    ),
    GoRoute(
      path: Routes.vetHome,
      builder: (_, _) => const VetHomeScreen(),
    ),
  ],
);
