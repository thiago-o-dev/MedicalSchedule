import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../models/pet/pet_model.dart';

class PetService {
  final Dio _dio = ApiClient.dio;

  Future<Response> createPet(PetModel pet) =>
      _dio.post('/api/pets', data: pet.toJson());

  Future<Response> getPets({String? ownerId}) => _dio.get(
        '/api/pets',
        queryParameters: {if (ownerId != null) 'ownerId': ownerId},
      );

  Future<Response> getPetById(String id) => _dio.get('/api/pets/$id');

  Future<Response> getPetOwner(String petId) =>
      _dio.get('/api/pets/$petId/owner');

  Future<Response> requestPetDeletion(String petId) =>
      _dio.delete('/api/pets/$petId');

  Future<Response> addPetOwner(
    String petId, {
    required String ownerId,
    bool isPrimaryOwner = false,
  }) =>
      _dio.post(
        '/api/pets/$petId/owners',
        data: {'ownerId': ownerId, 'isPrimaryOwner': isPrimaryOwner},
      );

  Future<Response> removePetOwner(String petId, String ownerId) =>
      _dio.delete('/api/pets/$petId/owners/$ownerId');
}
