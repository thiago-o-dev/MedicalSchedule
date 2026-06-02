import 'dart:io';

class Env {
  static String get baseUrl {
    // para rodar no android emulator
    if (Platform.isAndroid) {
      return 'http://10.0.2.2:5000';
    }

    return 'http://localhost:5000';
  }
}