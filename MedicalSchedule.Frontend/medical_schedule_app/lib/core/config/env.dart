import 'package:flutter/foundation.dart';

class Env {
  // Sobrescrita em tempo de build via --dart-define=API_BASE_URL=...
  // ou via ARG API_BASE_URL no Dockerfile.
  static const _definedBaseUrl = String.fromEnvironment('API_BASE_URL');

  static String get baseUrl {
    if (_definedBaseUrl.isNotEmpty) return _definedBaseUrl;
    // Gateway HTTP — sem problemas de certificado dev
    if (kIsWeb) return 'http://localhost:5197';
    return 'http://10.0.2.2:5197';
  }
}
