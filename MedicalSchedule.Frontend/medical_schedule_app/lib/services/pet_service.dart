import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../models/pet/pet_model.dart';

class PetService {
  final Dio _dio = ApiClient.dio;

  Future<Response> createPet(
    PetModel pet,
  ) async {
    return await _dio.post(
      '/api/registration/pets',
      data: pet.toJson(),
    );
  }

  Future<Response> getPets(
    String ownerId,
  ) async {
    return await _dio.get(
      '/api/registration/pets',
      queryParameters: {
        'ownerId': ownerId,
      },
    );
  }
}