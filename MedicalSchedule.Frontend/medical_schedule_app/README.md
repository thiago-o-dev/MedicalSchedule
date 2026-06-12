# Medical Schedule  - some of our notes about the frotnend project

This is the mobile frontend for our college project: a veterinary scheduling app built with Flutter. The idea is simple - pet owners can register their pets, browse vets, and schedule/manage appointments. Vets have their own view to see their schedule and related patients.

We used this project as an opportunity to learn some Flutter libraries that are widely used in production apps, which is why this README also serves as a bit of a study log for us.

---

## How to run

Make sure you have Flutter 3.x installed and the backend running locally (see the backend README for setup).

```bash
flutter pub get
flutter run
```

The base URL is configured in `lib/core/config/env.dart`. If you're running the backend on a different port, change it there.

---

## Project structure

```
lib/
  core/          # app-wide config: router, network client, storage, theme
  models/        # data classes with fromJson/toJson
  repositories/  # abstracts data access (used by providers)
  services/      # raw API calls using Dio
  state/         # Riverpod providers
  pages/         # screens (owner, vet, auth, shared)
  widgets/       # reusable UI components
```

We tried to keep a separation between "where data comes from" (services) and "how it's used" (repositories/providers), even if it added a bit of boilerplate :). It made the code easier to follow once we got used to it.

---

## Libraries

### flutter_riverpod 
**Docs:** https://riverpod.dev/docs/introduction/getting_started  
**pub.dev:** https://pub.dev/packages/flutter_riverpod

State management. We considered `Provider` (the older package) and even just using `setState` everywhere, but Riverpod made async state - like fetching the list of pets or appointments from the API - much cleaner. The `AsyncValue` type (which can be `loading`, `error`, or `data`) saved us from writing a ton of if-else checks in the UI.

For example, instead of managing a `bool isLoading` and a `String? errorMessage` manually, we just do:

```dart
final petsAsync = ref.watch(petsProvider(ownerId));

petsAsync.when(
  loading: () => CircularProgressIndicator(),
  error: (e, _) => Text(e.toString()),
  data: (pets) => ListView(...),
);
```

It also handles cache and refresh automatically, which was useful for invalidating the pet list after adding or deleting one.

---

### go_router  
**pub.dev:** https://pub.dev/packages/go_router

Navigation/routing. Flutter's built-in `Navigator.push` works fine for small apps but gets messy fast when you have multiple user roles (owner vs vet) with different home screens. `go_router` lets us define all routes in one place (`lib/core/routes/app_router.dart`) and use named paths like `/owner/pets` or `/vet/appointments`.

The redirect feature was also useful for auth - if the user isn't logged in, the router redirects them to `/login` automatically instead of us having to check that in every screen.

---

### dio  
**pub.dev:** https://pub.dev/packages/dio

HTTP client. We used `dio` instead of the built-in `http` package because it has interceptors, which let you add the jwt token to every request automatically without repeating that logic in every API call. Our interceptor lives in `lib/core/network/api_client.dart` - it reads the token from secure storage (another library) and attaches it to the `Authorization` header.

It also has better error handling: you can catch `DioException` and extract the status code and response body cleanly.

---

### flutter_secure_storage 
**pub.dev:** https://pub.dev/packages/flutter_secure_storage

Stores the JWT token securely on the device. On Android it uses the Android Keystore, on iOS it uses the Keychain. We initially just stored the token in shared preferences (which is essentially a plain text file), then realized that was a bad idea for auth tokens. This package was a straightforward drop in fix

---

### intl 
**pub.dev:** https://pub.dev/packages/intl

Date formatting. Flutter doesn't have a great built in way to format dates, so we used this. Mostly used for displaying dates in `dd/MM/yyyy` format and serializing them to `yyyy-MM-dd` for the API.

---

### pattern_box 
**pub.dev:** https://pub.dev/packages/pattern_box

Used for the decorative background on the auth screens (login and signup). It's a small package that generates geometric patterns using Flutter's `CustomPainter`. We mostly used it because the plain white background looked too bare and we didn't want to rely on a static image just for decoration. The `KaleidoscopePainter` with a random seed gives the screens a slightly different look each time you open them.

---

## Notes

- The app targets Android and iOS but was mainly tested on browser during development.
- There's no automated test suite yet - something we'd add with more time probably.
- The backend needs to be reachable at the URL set in `env.dart`. If you're testing on a physical device, `localhost` won't work - use your machine's local IP.
