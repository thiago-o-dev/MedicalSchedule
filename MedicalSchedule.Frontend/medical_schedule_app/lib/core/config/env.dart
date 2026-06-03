import 'package:flutter/foundation.dart';

class Env {
  static String get baseUrl {
    // Flutter Web
    if (kIsWeb) {
      return 'https://localhost:7001';
    }

    // Android Emulator
    return 'https://10.0.2.2:7001';
  }
}