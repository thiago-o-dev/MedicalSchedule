import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../models/vet/vet_model.dart';

class VetService {
  final Dio _dio = ApiClient.dio;

  Future<Response> createVet(VetModel vet) =>
      _dio.post('/registry/api/vets', data: vet.toJson());

  Future<Response> getVets() => _dio.get('/registry/api/vets');

  Future<Response> getMe() => _dio.get('/registry/api/vets/me');

  Future<Response> getVetById(String id) => _dio.get('/registry/api/vets/$id');

  Future<Response> deactivateVet(String id) => _dio.delete('/registry/api/vets/$id');
}
