import 'package:dio/dio.dart';
import 'package:flutter/material.dart';

import '../core/network/api_exception.dart';

void showApiErrorSnackBar(BuildContext context, Object error) {
  final messenger = ScaffoldMessenger.of(context);

  String message;
  Color color = Colors.red.shade700;

  if (error is DioException && error.error is ApiException) {
    final apiEx = error.error as ApiException;
    message = '${apiEx.title}: ${apiEx.detail}';
    if (apiEx.statusCode == 409) color = Colors.orange.shade800;
    if (apiEx.statusCode == 404) color = Colors.blueGrey;
  } else if (error is ApiException) {
    message = '${error.title}: ${error.detail}';
  } else {
    message = error.toString();
  }

  messenger.showSnackBar(
    SnackBar(
      content: Text(message),
      backgroundColor: color,
      behavior: SnackBarBehavior.floating,
    ),
  );
}

void showSuccessSnackBar(BuildContext context, String message) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      backgroundColor: Colors.green.shade700,
      behavior: SnackBarBehavior.floating,
    ),
  );
}
