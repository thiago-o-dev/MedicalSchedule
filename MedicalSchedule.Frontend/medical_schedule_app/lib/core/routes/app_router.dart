import 'package:go_router/go_router.dart';

import '../config/routes.dart';
import '../../pages/auth/login_screen.dart';
import '../../pages/auth/signup_screen.dart';
import '../../pages/owner/owner_home_screen.dart';
import '../../pages/owner/pet_detail_screen.dart';
import '../../pages/owner/appointment_detail_screen.dart';
import '../../pages/shared/profile_screen.dart';
import '../../pages/vet/vet_home_screen.dart';

final GoRouter appRouter = GoRouter(
  routes: [
    GoRoute(path: Routes.login, builder: (_, __) => LoginScreen()),
    GoRoute(path: Routes.signup, builder: (_, __) => SignupScreen()),
    GoRoute(path: Routes.ownerHome, builder: (_, __) => OwnerHomeScreen()),
    GoRoute(path: Routes.vetHome, builder: (_, __) => VetHomeScreen()),

    GoRoute(
      path: '${Routes.petDetail}/:petId',
      builder: (_, state) =>
          PetDetailScreen(petId: state.pathParameters['petId']!),
    ),
    GoRoute(
      path: '${Routes.appointmentDetail}/:appointmentId',
      builder: (_, state) => AppointmentDetailScreen(
        appointmentId: state.pathParameters['appointmentId']!,
      ),
    ),
    GoRoute(
      path: '${Routes.profile}/:role/:userId',
      builder: (_, state) => ProfileScreen(
        role: state.pathParameters['role']!,
        userId: state.pathParameters['userId']!,
      ),
    ),
  ],
);
